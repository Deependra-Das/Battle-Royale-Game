using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelScriptableObject", menuName = "ScriptableObjects/LevelScriptableObject")]
public class LevelScriptableObject : ScriptableObject
{
    public List<GameObject> _floorPrefabList;
    public GameObject _spawnTileCluster;
    public GameObject _gameOverTrigger;

    public float _floorHeightIncrement;
    public float _radius;
    public int _numberOfPlayers;
}
