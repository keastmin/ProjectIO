using System;
using UnityEngine;

public class TerritoryTestPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;

    public event Action<TerritoryTestPlayer> OnPositionChanged;

    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movement = moveSpeed * Time.deltaTime * new Vector3(horizontal, vertical, 0);
        transform.Translate(movement, Space.World);

        OnPositionChanged?.Invoke(this);

        // var result = Physics.Raycast(new Ray(transform.position + Vector3.back * 3, Vector3.forward * 20f), out RaycastHit hit, 20f);
        // if (result)
        // {
        //     Debug.Log($"Hit: {hit.collider.name} at {hit.point}");
        // }
        // else
        // {
        //     Debug.Log("No hit detected.");
        // }
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