using UnityEngine;

[CreateAssetMenu(fileName = "NetworkScriptableObject", menuName = "ScriptableObjects/NetworkScriptableObject")]
public class NetworkScriptableObject : ScriptableObject
{
    public GameplayManager gameplayManagerPrefab;
    public TileRegistry tileRegistryPrefab;
}
