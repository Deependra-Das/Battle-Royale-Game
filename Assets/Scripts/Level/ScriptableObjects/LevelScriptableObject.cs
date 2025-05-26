using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelScriptableObject", menuName = "ScriptableObjects/LevelScriptableObject")]
public class LevelScriptableObject : ScriptableObject
{
    public int floorCount = 1;
    public int mapFloorRadius = 10;
    public int spawnFloorRadius = 40;
    public int spawnClusterRadius = 1;
    public float tileOffset_X = 4.0f;
    public float tileOffset_Z = 3.45f;
    public float floorHeightIncrement;
    public GameObject levelContainerPrefab;
    public GameObject floorPrefab;
    public GameObject spawnContainerPrefab;
    public GameObject gameOverTrigger;
    public GameObject basePlane;
    public List<GameObject> hexTilePrefabList;
}
