using UnityEngine;

public enum ResourceType
{
    Mineral,
    Gas,
}

public class Resource : MonoBehaviour
{
    public ResourceType Type;
    public int Amount = 5;

    public void Obtain()
    {
        // 자원 획득 로직
        Debug.Log($"Obtained {Amount} resources from {gameObject.name}");
        Destroy(gameObject); // 자원 획득 후 제거
    }
}