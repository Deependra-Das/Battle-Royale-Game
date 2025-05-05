using UnityEngine;
using Fusion;

public struct NetworkInputData : INetworkInput
{
    public Vector2 movementInput;
    public Vector3 lookInput;
    public NetworkBool isJumpPressed;
}
