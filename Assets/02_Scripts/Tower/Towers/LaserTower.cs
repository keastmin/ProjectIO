using Fusion;
using System.Collections;
using UnityEngine;

public class LaserTower : AttackTower
{
    [Header("레이저")]
    [SerializeField] private LineRenderer _laser;
    [SerializeField] private float _laserWidth;
    [SerializeField] private float _laserLength;
    [SerializeField] private float _laserDamage;
    [SerializeField] private float _laserWidthDecreaseRate;

    [Header("시각")]
    [SerializeField] private Transform _head;

    private bool _isFire = false;
    private float _currLaserWidth = 0;

    public override void Spawned()
    {
        if (HasStateAuthority)
        {
            InitHostLaserTower();
        }

        InitLocalLaserTower();
    }

    public override void FixedUpdateNetwork()
    {
        if (HasStateAuthority)
        {
            SetTarget();
            LookAtTarget();
            Fire();
        }
    }

    public override void Render()
    {
        FireLaser();
    }

    protected override void LookAtTarget()
    {
        // Y축만 회전
        if (_currTarget != null)
        {
            Vector3 dir = _currTarget.transform.position - _head.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.0001f)
            {
                var look = Quaternion.LookRotation(dir);
                _head.rotation = Quaternion.Lerp(
                    _head.rotation, look, _rotateSpeed * Runner.DeltaTime);
            }
        }
    }

    protected override void Fire()
    {
        if (_attackTick.ExpiredOrNotRunning(Runner) && _currTarget != null)
        {
            _attackTick = TickTimer.CreateFromSeconds(Runner, _attackSpeed);

            RaycastHit[] hits;
            hits = Physics.SphereCastAll(_attackPosition.position, _laserWidth / 2f, _attackPosition.forward, _laserLength - (_laserWidth / 2f), _enemyLayer);

            foreach(var hit in hits)
            {
                hit.collider.TryGetComponent(out Monster monster);
                monster.TakeDamage(_laserDamage);
            }

            RPC_LocalFire();
        }
    }

    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    private void RPC_LocalFire()
    {
        _laser.enabled = true;
        _laser.positionCount = 2;
        _currLaserWidth = _laserWidth;
        _isFire = true;
    }

    private void FireLaser()
    {
        if (_isFire)
        {
            _laser.SetPosition(0, _attackPosition.position);
            _laser.SetPosition(1, _attackPosition.position + (_attackPosition.forward * _laserLength));

            _laser.widthMultiplier = _currLaserWidth;
            _currLaserWidth -= (Time.deltaTime * _laserWidthDecreaseRate);

            if (_currLaserWidth <= 0)
            {
                _currLaserWidth = 0;
                _laser.enabled = false;
                _isFire = false;
            }
        }
    }

    private void InitHostLaserTower()
    {
        // 공격 타이머 초기화
        _attackTick = TickTimer.CreateFromSeconds(Runner, _attackSpeed);
    }

    private void InitLocalLaserTower()
    {
        TryGetComponent(out _laser);
        _laser.enabled = false;
    }
}
