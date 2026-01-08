using DG.Tweening;
using Fusion;
using System;
using System.Threading.Tasks;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerRunner : Player, IDamageable
{
    [Header("Statistics")]
    [Networked] public float HP { get; set; } = 300f;
    [Networked] public float Stamina { get; set; } = 100f;
    [Networked] public float RunningPower { get; set; } = 30f;
    [Networked] public float DamageReduction { get; set; } = 0f;
    [Networked] public float WeaponDamageScaler { get; set; } = 1f;
    [Networked] public NetworkBool IsDead { get; set; }

    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 6f; // 이동 속도

    private Rigidbody _rigidbody; // 리지드바디
    private RunnerItemConsumer _itemConsumer; // 아이템 소비자
    private RunnerSkillCaster _skillCaster; // 스킬 시전자
    private bool _isSliding = false; // 슬라이드 상태
    private bool _isTumbling = false; // 텀블 상태
    private bool _isOutOfBody = false; // 영혼 상태
    private Sequence _outOfBodySequence;
    private GameObject _outOfBodySpiritObject;
    private bool _isSwiftness = false; // 스위프트니스 상태
    private int _swiftnessSlideCount = 0; // 스위프트니스 슬라이드 횟수
    private bool _isInvincible = false; // 무적 상태
    private float _elapsedTime = 0f; // 경과 시간

    public event Action<PlayerRunner> OnPositionChanged; // 영역 관련 이벤트
    public event Action<PlayerRunner> OnPositionChangedInLocal; // 로컬 영역 관련 이벤트

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _itemConsumer = new RunnerItemConsumer();
        _skillCaster = new RunnerSkillCaster();
    }

    public override void Spawned()
    {
        InitializePlayerRunner();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // 러너 이동
            if (_isSliding == false && _isTumbling == false)
            {
                float speed = data.DashInput.IsSet(NetworkInputData.DASH_INPUT) ? _moveSpeed * 2f : _moveSpeed;
                data.PlayerRunnerDirection.Normalize();
                _rigidbody.linearVelocity = speed * data.PlayerRunnerDirection;
                transform.LookAt(transform.position + data.PlayerRunnerDirection);
            }

            // 러너 슬라이드
            var slideUsing = data.SlideInput.IsSet(NetworkInputData.SLIDE_INPUT);
            if (slideUsing)
            {
                _ = StartSlide();
            }
                
            // 러너 아이템 사용
            var itemUsing = data.ItemInput.IsSet(NetworkInputData.ITEM_INPUT);
            if (itemUsing)
                UseItem(data.SelectedItem);

            // 러너 스킬 사용
            var skillUsing = data.SkillInput.IsSet(NetworkInputData.SKILL_INPUT);
            if (skillUsing)
            {
                if (_isOutOfBody)
                    EndOutOfBody(true); // 영혼 상태일 때 스킬 사용 시 영혼 상태 종료
                else
                    CastSkill(data.SelectedSkill);
            }

            // 러너 상호작용
            var interactUsing = data.InteractInput.IsSet(NetworkInputData.INTERACT_INPUT);
            if (interactUsing)
                Debug.Log("상호작용 사용");

            // 러너 연구소 상호작용
            var laboratoryUsing = data.LaboratoryInput.IsSet(NetworkInputData.LABORATORY_INPUT);
            if (laboratoryUsing)
            {
                var lab = StageManager.Instance.Laboratory;
                StageManager.Instance.CinemachineSystem.SetTrackingTarget(lab.transform);
                Debug.Log("연구소 보기");
            }
            else
            {
                StageManager.Instance.CinemachineSystem.SetTrackingTarget(transform);
            }

            var weaponUsing = data.WeaponInput.IsSet(NetworkInputData.WEAPON_INPUT);
            if (weaponUsing)
            {
                Debug.Log("무기 사용"); // 빌더가 구매해서 러너에게 장착시킴
                // Debug.Log(data.SelectedWeapon);
            }
        }

        // TakeDamage(Time.fixedDeltaTime * 0.5f);

        // 영역에 관한 로직 이벤트 수행 - 러너만
        OnPositionChanged?.Invoke(this);
    }

    private void Update()
    {
        _elapsedTime += Time.deltaTime;
        var minutes = Mathf.FloorToInt(_elapsedTime / 60f);
        var seconds = Mathf.FloorToInt(_elapsedTime % 60f);
        StageManager.Instance.UIController.RunnerUI.Display.ElapsedTime.SetElapsedTimeText($"{minutes:D2}:{seconds:D2}");

        // 영역에 관한 로직 이벤트 수행(로컬) - Host, Client 구분 없이 매 프레임
        // OnPositionChangedInLocal?.Invoke(this);
    }

    // 플레이어 러너 초기화
    private void InitializePlayerRunner()
    {
        if (HasStateAuthority)
        {
            HP = 100f;
            IsDead = false;
            _isSliding = false;
            _isInvincible = false;
        }
    }

    private async Task StartSlide()
    {
        if (_isSliding) return; // 이미 슬라이드 상태면 무시
        Debug.Log("슬라이드 시작");
        _isSliding = true;
        _rigidbody.linearVelocity = Vector3.zero; // 슬라이드 시작 시 현재 속도 초기화
        _rigidbody.AddForce(3f * _moveSpeed * transform.forward, ForceMode.Impulse);
        var originalDrag = _rigidbody.linearDamping;
        _rigidbody.linearDamping = 2f; // 슬라이드 시 마찰력 증가
        Stamina -= 10f;
        StageManager.Instance.UIController.RunnerUI.Display.Player.SetStaminaBarRatio(Stamina / 100f); // UI 기력바 갱신
        await Task.Delay(1000); // 1초 동안 슬라이드 상태 유지
        _rigidbody.linearDamping = originalDrag;
        _isSliding = false;
        Debug.Log("슬라이드 종료");
    }

    private void UseItem(int itemIndex)
    {
        Debug.Log($"아이템 {itemIndex} 사용");

        var runnerItemType = (RunnerItemType)itemIndex;
        _itemConsumer.Use(runnerItemType, this);
    }

    private void CastSkill(int skillIndex)
    {
        Debug.Log($"스킬 {skillIndex} 사용");

        var skillType = (RunnerSkillType)skillIndex;
        _skillCaster.Cast(skillType, this);
    }

    public void StartTumble()
    {
        if (_isTumbling) return; // 이미 텀블 상태면 무시
        _isTumbling = true;
        _rigidbody.linearVelocity = _moveSpeed * transform.forward;
        DOTween.Sequence()
            .AppendInterval(1.0f)
            .AppendCallback(() => _isTumbling = false);
    }

    public void StartInvincibility(float duration)
    {
        if (!HasStateAuthority) return; // 상태 권한이 없으면 무시 - 호스트만 변수값 변경 가능
        if (_isInvincible) return; // 이미 무적 상태면 무시 ... 처음부터 다시 무적 상태 시작 | 지속시간 추가 | 무시

        Debug.Log("무적 상태 시작");
        _isInvincible = true; // 무적 상태 시작
        _ = EndInvincibilityAfterDelay(duration); // 일정 시간 후 무적 상태 종료
    }

    private async Task EndInvincibilityAfterDelay(float duration)
    {
        await Task.Delay(TimeSpan.FromSeconds(duration));
        if (HasStateAuthority)
            _isInvincible = false; // 무적 상태 종료
        Debug.Log("무적 상태 종료");
    }

    public void StartOutOfBody()
    {
        if (_isOutOfBody) return; // 이미 영혼 상태면 무시
        _isOutOfBody = true;
        _outOfBodySpiritObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        _outOfBodySpiritObject.transform.position = transform.position + transform.forward * 1f;
        _outOfBodySequence = DOTween.Sequence()
            .Append(_outOfBodySpiritObject.transform.DOMove(transform.position + transform.forward * 50f, 5.0f).SetEase(Ease.Linear))
            .AppendCallback(() => EndOutOfBody());
    }

    private void EndOutOfBody(bool forceEnd = false)
    {
        if (!_isOutOfBody) return; // 영혼 상태가 아니면 무시
        _outOfBodySequence.Kill();
        _outOfBodySequence = null;
        _isOutOfBody = false;
        if (forceEnd)
            transform.position = _outOfBodySpiritObject.transform.position;
        Destroy(_outOfBodySpiritObject);
        _outOfBodySpiritObject = null;
    }

    public void StartSwiftness()
    {
        if (_isSwiftness) return; // 이미 스위프트니스 상태면 무시
        _isSwiftness = true;
        // 무기 공격 속도
        // 장전 속도 대폭 증가
        _swiftnessSlideCount = 3; // 슬라이드 3회 획득
        DOTween.Sequence()
            .AppendInterval(10.0f) // 10초 지속
            .AppendCallback(() =>
            {
                _isSwiftness = false;
                _swiftnessSlideCount = 0;
                Debug.Log("스위프트니스 상태 종료");
            });
    }

    public void TakeDamage(float damage)
    {
        if (!HasStateAuthority) return; // 상태 권한이 없으면 무시 - 호스트만 변수값 변경 가능
        if (IsDead) return; // 이미 죽었으면 무시
        if (_isInvincible) return; // 무적 상태면 무시

        HP -= damage; // 체력 감소
        StageManager.Instance.UIController.RunnerUI.Display.Player.SetHealthBarRatio(HP / 100f); // UI 체력바 갱신

        if (HP <= 0f) // 체력이 0 이하라면
            IsDead = true; // 죽음 처리
    }
}
