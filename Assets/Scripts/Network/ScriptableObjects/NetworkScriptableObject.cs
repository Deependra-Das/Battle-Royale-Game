using UnityEngine;

[CreateAssetMenu(fileName = "NetworkScriptableObject", menuName = "ScriptableObjects/NetworkScriptableObject")]
public class NetworkScriptableObject : ScriptableObject
{
    public GameplayManager gameplayManagerPrefab;
    public PlayerSessionManager playerSessionManagerPrefab;
    public TileRegistry tileRegistryPrefab;
}
