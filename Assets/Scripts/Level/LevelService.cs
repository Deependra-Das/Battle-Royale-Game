using BattleRoyale.Floor;
using BattleRoyale.Tile;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

namespace BattleRoyale.Level
{
    [DefaultExecutionOrder(-10)]
    public class LevelService : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _floorPrefabList;
        [SerializeField] private GameObject _spawnTileCluster;
        [SerializeField] private GameObject _gameOverTrigger;

        [SerializeField] private float _floorHeightIncrement;    
        [SerializeField] private float _radius;
        [SerializeField] private int _numberOfPlayers;
       

        private List<GameObject> _floorsList = new List<GameObject>();
        private List<Vector3> _playerSpawnPontsList = new List<Vector3>();

        void Start()
        {
            GenerateTileFloors();
            GenerateSpawnTileCluster();
            GenerateGameOverTrigger();
        }

        public void GenerateTileFloors()
        {
            for (int i = 1; i <= _floorPrefabList.Count; i++)
            {
                Vector3 position = new Vector3(0, i * _floorHeightIncrement, 0);
                GameObject newFloor = Instantiate(_floorPrefabList[i-1]);

                newFloor.transform.position = position;
                newFloor.transform.parent = transform;
                newFloor.name = "Floor_" + i;
                _floorsList.Add(newFloor);
            }
        }

        public void DestroyAllFloors()
        {
            foreach (GameObject floor in _floorsList)
            {
                Destroy(floor);
            }
            _floorsList.Clear();
        }

        public List<GameObject> GetFloors()
        {
            return _floorsList;
        }

        public void GenerateSpawnTileCluster()
        {
            float angleIncrement = 360f / _numberOfPlayers;

            for (int i = 0; i < _numberOfPlayers; i++)
            {
                float angle = i * angleIncrement;
                float radian = angle * Mathf.Deg2Rad;
                Vector3 position = new Vector3(Mathf.Cos(radian) * _radius, ((_floorPrefabList.Count + 1) * _floorHeightIncrement), Mathf.Sin(radian) * _radius);
                GameObject newSpawnTileCluster = Instantiate(_spawnTileCluster, position, Quaternion.identity);
                newSpawnTileCluster.transform.parent = transform;
                newSpawnTileCluster.name = "SpawnTileCluster_" + (i+1).ToString();
                _playerSpawnPontsList.Add(position);
                _floorsList.Add(newSpawnTileCluster);
            }
        }

        public List<Vector3> GetPlayerSpawnPoints()
        {
            return _playerSpawnPontsList;
        }

        public void GenerateGameOverTrigger()
        {
            GameObject trigger = Instantiate(_gameOverTrigger, new Vector3(transform.position.x, (transform.position.y), transform.position.z), Quaternion.identity);
            trigger.transform.parent = transform;
        }
    }
}

