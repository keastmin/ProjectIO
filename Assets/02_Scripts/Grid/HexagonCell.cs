using UnityEngine;

public enum GridState
{
    None,
    Tower
}

public class HexagonCell
{
    // 인덱스
    public int IndexX;
    public int IndexY;

    // 위치
    public float PosX;
    public float PosZ;

    // 상태
    public GridState State;

    public HexagonCell(int x, int y, float posX, float posZ)
    {
        IndexX = x;
        IndexY = y;
        PosX = posX;
        PosZ = posZ;
        State = GridState.None;
    }

    public Vector3 WorldPosition => new Vector3(PosX, 0f, PosZ);
}
