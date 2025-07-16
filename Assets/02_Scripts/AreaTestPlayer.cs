using System;
using UnityEngine;

public class AreaTestPlayer : MonoBehaviour
{
    public float moveSpeed = 5f;
    public bool isInside;
    public Vector2 previousPosition;
    public bool isExtending;

    [SerializeField] AreaTestController testController;

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

        if (testController.go == null)
        {
            return;
        }

        isInside = testController.IsInside(new Vector2(transform.position.x, transform.position.y));
        if (isInside)
        {
            if (isExtending)
            {
                testController.RefreshPolygonPoints();
                isExtending = false;
                Debug.Log("다시 들어옴");
            }
        }
        else
        {
            if (!isExtending)
            {
                Debug.Log("나감");
            }
            isExtending = true;
            var currentPosition = new Vector2(transform.position.x, transform.position.y);

            if (Vector2.SqrMagnitude(currentPosition - previousPosition) > 0.01f)
            {
                testController.MemorizePosition(currentPosition);
                previousPosition = currentPosition;
            }
        }
    }
}