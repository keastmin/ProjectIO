using Unity.Cinemachine;
using UnityEngine;

public class PlayerBuilderMover : MonoBehaviour
{
    [SerializeField] private CinemachineCamera _builderCam;
    [SerializeField] private float _moveSpeed = 10f;
    [SerializeField] private float _movePixel = 5f;
}
