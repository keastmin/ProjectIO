using Fusion;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerRunner : Player, IDamageable
{
    [Header("플레이어 상태")]
    [Networked] public float HP { get; set; } = 300f;
    [Networked] public float Stamina { get; set; } = 100f;
    [Networked] public float RunningPower { get; set; } = 30f;
    [Networked] public float DamageReduction { get; set; } = 0f;
    [Networked] public float WeaponDamageScaler { get; set; } = 1f;
    [Networked] public NetworkBool IsDead { get; set; }

    [Header("이동")]
    [SerializeField] private float _moveSpeed = 6f; // 이동 속도

    private Rigidbody _rigidbody; // 리지드바디
    private bool _isSliding = false; // 슬라이드 상태

    public event Action<PlayerRunner> OnPositionChanged; // 영역 관련 이벤트


    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>(); // 리지드바디 받아오기
    }

    public override void Spawned()
    {
        InitPlayerRunner();
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            // 러너 이동
            if (_isSliding == false)
            {
                float speed = data.DashInput.IsSet(NetworkInputData.DASH_INPUT) ? _moveSpeed * 2f : _moveSpeed;
                data.PlayerRunnerDirection.Normalize();
                _rigidbody.linearVelocity = speed * data.PlayerRunnerDirection;
                transform.LookAt(transform.position + data.PlayerRunnerDirection);
            }

            // 러너 슬라이드
            var slideUsing = data.SlideInput.IsSet(NetworkInputData.SLIDE_INPUT);
            if (slideUsing)
                _ = StartSlide();
                

            // 러너 아이템 사용
            var itemUsing = data.ItemInput.IsSet(NetworkInputData.ITEM_INPUT);
            if (itemUsing)
            {
                Debug.Log("아이템 사용");
                UseItem(data.SelectedItem);
            }

            // 러너 스킬 사용
            var skillUsing = data.SkillInput.IsSet(NetworkInputData.SKILL_INPUT);
            if (skillUsing)
                Debug.Log("스킬 사용");

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

        // 영역에 관한 로직 이벤트 수행
        OnPositionChanged?.Invoke(this);
    }

    // 플레이어 러너 초기화
    private void InitPlayerRunner()
    {
        if (HasStateAuthority)
        {
            HP = 100f;
            IsDead = false;
        }
    }

    private async Task StartSlide()
    {
        _isSliding = true;
        _rigidbody.linearVelocity = Vector3.zero; // 슬라이드 시작 시 현재 속도 초기화
        _rigidbody.AddForce(3f * _moveSpeed * transform.forward, ForceMode.Impulse);
        var originalDrag = _rigidbody.linearDamping;
        _rigidbody.linearDamping = 2f; // 슬라이드 시 마찰력 증가
        await Task.Delay(1000); // 1초 동안 슬라이드 상태 유지
        _rigidbody.linearDamping = originalDrag;
        _isSliding = false;
    }

    enum ItemType
    {
        None,
        Return,
        SpawnShield,
        SpawnDrone,
        ElectricGrenade,
        GasMine,
    }

    private void UseItem(int itemIndex)
    {
        Debug.Log($"아이템 {itemIndex} 사용");
        switch ((ItemType)itemIndex)
        {
            case ItemType.Return: ReturnToLaboratory(); break;
            case ItemType.SpawnShield: SpawnShield(); break;
            case ItemType.SpawnDrone: SpawnDrone(); break;
            case ItemType.ElectricGrenade: ThrowElectricGrenade(); break;
        }
    }

    private void ReturnToLaboratory()
    {
        // 1. 연구소 위치 가져오기
        // 2. 위치 근처로 이동
        // 연구소 바로 아래에 이동
        var targetPosition = Vector3.zero;
        var angle = UnityEngine.Random.Range(0f, 2f * Mathf.PI);
        var x = Mathf.Cos(angle) * 2f;
        var z = Mathf.Sin(angle) * 2f;
        var randomOffset = new Vector3(x, transform.position.y, z);
        transform.position = targetPosition + randomOffset;
    }

    private void SpawnShield()
    {
        // Shield 오브젝트 생성
        // Shield 내부 로직: 점차 커지면서 최대 크기로 커지면 사라짐
    }

    private void SpawnDrone()
    {
        // Drone 오브젝트 생성 및 초기화
        // Drone 내부 로직: 일정 시간 동안 플레이어 주변을 맴돌며 적 공격, 일정 시간 후 터지면서 사라짐
    }

    private void ThrowElectricGrenade()
    {
        // 전기 수류탄 생성 및 투척
        // 수류탄 내부 로직: 던지면 물리 적용해서 움직임, 오브젝트에 닿으면 폭발하면서 주변 적에게 데미지 입힘
    }
    private void InstallGasMine()
    {
        // 가스 지뢰 생성
        // 지뢰 내부 로직: 적과 닿으면 터지고 적이 밀림
    }

    // 데미지 입기
    public void TakeDamage(float damage)
    {
        if (!HasStateAuthority) return; // 상태 권한이 없으면 무시 - 호스트만 변수값 변경 가능
        if (IsDead) return; // 이미 죽었으면 무시

        HP -= damage; // 체력 감소

        if (HP <= 0f) // 체력이 0 이하라면
            IsDead = true; // 죽음 처리
    }
}
