using Fusion;
using System;
using UnityEngine;

public class PlayerRunner : Player, IDamageable
{
    [Header("플레이어 상태")]
    [Networked] public float HP { get; set; }
    [Networked] public NetworkBool IsDead { get; set; }

    [Header("이동")]
    [SerializeField] private float _moveSpeed = 6f; // 이동 속도

    private Rigidbody _rigidbody; // 리지드바디

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
            float speed = data.DashInput.IsSet(NetworkInputData.DASH_INPUT) ? _moveSpeed * 2f : _moveSpeed;
            data.PlayerRunnerDirection.Normalize();
            _rigidbody.linearVelocity = speed * data.PlayerRunnerDirection;

            // 러너 아이템 사용
            var itemUsing = data.ItemInput.IsSet(NetworkInputData.ITEM_INPUT);
            if (itemUsing)
            {
                Debug.Log("아이템 사용");
                Debug.Log(data.SelectedItem);
            }

            // 러너 스킬 사용
            var skillUsing = data.SkillInput.IsSet(NetworkInputData.SKILL_INPUT);
            if (skillUsing)
                Debug.Log("스킬 사용");

            // 러너 상호작용
            var interactUsing = data.InteractInput.IsSet(NetworkInputData.INTERACT_INPUT);
            if (interactUsing)
                Debug.Log("상호작용 사용");
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
