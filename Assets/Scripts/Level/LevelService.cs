using BattleRoyale.Floor;
using BattleRoyale.Tile;
using UnityEngine;
using System.Collections.Generic;

namespace BattleRoyale.Level
{

    public class LevelService
    {
        private List<GameObject> _floorPrefabList;
        private GameObject _spawnTileCluster;
        private GameObject _gameOverTrigger;
        private GameObject _basePlane;
        private float _floorHeightIncrement;    
        private float _radius;
        private int _numberOfPlayers;
        private GameObject _activelevel;
        private Transform _parentTansform;

        private List<GameObject> _floorsList = new List<GameObject>();
        private List<Vector3> _playerSpawnPontsList = new List<Vector3>();

        public LevelService(LevelScriptableObject level_SO)
        {
            _floorPrefabList = level_SO.floorPrefabList;
            _spawnTileCluster = level_SO.spawnTileCluster;
            _gameOverTrigger = level_SO.gameOverTrigger;
            _basePlane = level_SO.basePlane;
            _floorHeightIncrement = level_SO.floorHeightIncrement;
            _radius = level_SO.radius;
            _numberOfPlayers = level_SO.numberOfPlayers;
        }

        public void StartLevel()
        {
            GenerateLevelContainer();
            GenerateBasePlane();
            GenerateTileFloors();
            GenerateSpawnTileCluster();
            GenerateGameOverTrigger();
        }

        private void GenerateLevelContainer()
        {
            _activelevel = new GameObject();
            _activelevel.name = "LevelContainer";
            _parentTansform = _activelevel.transform;
        }

        private void GenerateBasePlane()
        {
            GameObject basePlane = Object.Instantiate(_basePlane);
            basePlane.transform.parent = _parentTansform;
        }

        private void GenerateTileFloors()
        {
            for (int i = 1; i <= _floorPrefabList.Count; i++)
            {
                Vector3 position = new Vector3(0, i * _floorHeightIncrement, 0);
                GameObject newFloor = Object.Instantiate(_floorPrefabList[i-1]);

                newFloor.transform.position = position;
                newFloor.transform.parent = _parentTansform;
                newFloor.name = "Floor_" + i;
                _floorsList.Add(newFloor);
            }
        }

        private void GenerateSpawnTileCluster()
        {
            float angleIncrement = 360f / _numberOfPlayers;

            for (int i = 0; i < _numberOfPlayers; i++)
            {
                float angle = i * angleIncrement;
                float radian = angle * Mathf.Deg2Rad;
                Vector3 position = new Vector3(Mathf.Cos(radian) * _radius, ((_floorPrefabList.Count + 1) * _floorHeightIncrement), Mathf.Sin(radian) * _radius);
                GameObject newSpawnTileCluster = Object.Instantiate(_spawnTileCluster, position, Quaternion.identity);
                newSpawnTileCluster.transform.parent = _parentTansform;
                newSpawnTileCluster.name = "SpawnTileCluster_" + (i+1).ToString();
                _playerSpawnPontsList.Add(position);
                _floorsList.Add(newSpawnTileCluster);
            }
        }

        public List<Vector3> GetPlayerSpawnPoints()
        {
            return _playerSpawnPontsList;
        }

        private void GenerateGameOverTrigger()
        {
            GameObject trigger = Object.Instantiate(_gameOverTrigger, new Vector3(_parentTansform.position.x, (_parentTansform.position.y), _parentTansform.position.z), Quaternion.identity);
            trigger.transform.parent = _parentTansform;
        }

        public List<GameObject> GetFloors()
        {
            return _floorsList;
        }

        public void DestroyAllFloors()
        {
            foreach (GameObject floor in _floorsList)
            {
                Object.Destroy(floor);
            }
            _floorsList.Clear();
        }
    }
}

