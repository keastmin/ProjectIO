using System;
using Fusion;
using UnityEngine;

public class NetworkPlayerRunner : NetworkBehaviour
{
    public float moveSpeed = 5f;

    public event Action<Transform> OnPositionChangedEvent;

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        var isDash = Input.GetKey(KeyCode.LeftShift);
        Vector3 movement = moveSpeed * Time.deltaTime * new Vector3(horizontal, 0, vertical);
        if (isDash) { movement *= 2; }
        transform.Translate(movement, Space.World);

        OnPositionChangedEvent?.Invoke(transform);
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasInputAuthority)
        {
            
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.back * 3, 0.5f);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + Vector3.back * 3 + Vector3.forward * 20f, 0.5f);

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position + Vector3.back * 3, transform.position + Vector3.back * 3 + Vector3.forward * 20f);
    }
}