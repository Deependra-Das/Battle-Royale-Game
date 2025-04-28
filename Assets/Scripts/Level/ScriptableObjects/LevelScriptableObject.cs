using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelScriptableObject", menuName = "ScriptableObjects/LevelScriptableObject")]
public class LevelScriptableObject : ScriptableObject
{
    public List<GameObject> floorPrefabList;
    public GameObject spawnTileCluster;
    public GameObject gameOverTrigger;
    public GameObject basePlane;
    public float floorHeightIncrement;
    public float radius;
    public int numberOfPlayers;
}
