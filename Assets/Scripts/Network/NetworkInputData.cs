using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector2 movementInput;
    public float rotationInput;
    public NetworkBool isJumpPressed;
}
