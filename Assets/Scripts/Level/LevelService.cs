using BattleRoyale.Tile;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.Level
{
    public class LevelService
    {
        private GameObject _levelContainerPrefab;
        private List<GameObject> _hexTilePrefabList;
        private GameObject _floorPrefab;
        private GameObject _spawnContainerPrefab;
        private GameObject _gameOverTrigger;
        private GameObject _basePlane;
        private float _floorHeightIncrement;
        private int _mapFloorRadius;
        private int _spawnFloorRadius;
        private int _spawnClusterRadius;
        private float _tileOffset_X;
        private float _tileOffset_Z;
        private int _floorCount;
        private GameObject _activeLevel;
        private Transform _parentTransform;
        private Transform _gameOverTriggerTransform;
        private Transform _basePlaneTransform;

        private List<GameObject> _floorsList = new List<GameObject>();
        private List<GameObject> _hexTileList = new List<GameObject>();
        private List<Vector3> _playerSpawnPointsList = new List<Vector3>();

        public int ServerSpawnedTileCount => _hexTileList.Count;
        public bool IsLevelReady { get; private set; } = false;

        public LevelService(LevelScriptableObject level_SO)
        {
            _hexTilePrefabList = level_SO.hexTilePrefabList;
            _levelContainerPrefab = level_SO.levelContainerPrefab;
            _floorPrefab = level_SO.floorPrefab;
            _spawnContainerPrefab = level_SO.spawnContainerPrefab;
            _gameOverTrigger = level_SO.gameOverTrigger;
            _basePlane = level_SO.basePlane;
            _floorHeightIncrement = level_SO.floorHeightIncrement;
            _mapFloorRadius = level_SO.mapFloorRadius;
            _spawnFloorRadius = level_SO.spawnFloorRadius;
            _spawnClusterRadius = level_SO.spawnClusterRadius;
            _tileOffset_X = level_SO.tileOffset_X;
            _tileOffset_Z = level_SO.tileOffset_Z;
            _floorCount = level_SO.floorCount;
        }

        public IEnumerator StartLevelCoroutine(int numberOfPlayers)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                GenerateLevelContainer();
                GenerateBasePlane();
                yield return GenerateFloorsCoroutine();
                yield return GenerateSpawnTileClusterCoroutine(numberOfPlayers);
                GenerateGameOverTrigger();
                IsLevelReady = true;
            }
        }

        private void GenerateLevelContainer()
        {
            _activeLevel = Object.Instantiate(_levelContainerPrefab);
            NetworkObject networkObject = _activeLevel.GetComponent<NetworkObject>();
            _parentTransform = _activeLevel.transform;

            if (networkObject != null)
            {
                networkObject.Spawn();
            }
        }

        private void GenerateBasePlane()
        {
            GameObject basePlaneObject = Object.Instantiate(_basePlane);
            NetworkObject networkObject = basePlaneObject.GetComponent<NetworkObject>();

            if (networkObject != null)
            {
                networkObject.Spawn();
            }
            basePlaneObject.transform.parent = _parentTransform;
            _basePlaneTransform = basePlaneObject.transform;
        }

        private IEnumerator GenerateFloorsCoroutine()
        {
            for (int i = 1; i <= _floorCount; i++)
            {
                Vector3 position = new Vector3(0, i * _floorHeightIncrement, 0);
                GameObject newFloor = Object.Instantiate(_floorPrefab, position, Quaternion.identity);
                newFloor.name = "Floor_" + i;
                _floorsList.Add(newFloor);

                NetworkObject networkObject = newFloor.GetComponent<NetworkObject>();
                if (networkObject != null) networkObject.Spawn();
                newFloor.transform.parent = _parentTransform;
                yield return GenerateHexTileMapCoroutine(_mapFloorRadius, newFloor.transform, _hexTilePrefabList[i]);
            }
        }

     
    private IEnumerator GenerateSpawnTileClusterCoroutine(int numberOfPlayers)
    {
        float angleIncrement = 360f / numberOfPlayers;
        for (int i = 0; i < numberOfPlayers; i++)
        {
            float angle = i * angleIncrement;
            float radian = angle * Mathf.Deg2Rad;
            Vector3 position = new Vector3(Mathf.Cos(radian) * _spawnFloorRadius, (_floorCount + 1) * _floorHeightIncrement, Mathf.Sin(radian) * _spawnFloorRadius);
            GameObject newSpawnTileCluster = Object.Instantiate(_spawnContainerPrefab, position, Quaternion.identity);
            newSpawnTileCluster.name = "SpawnTileCluster_" + (i + 1);

            _playerSpawnPointsList.Add(position + new Vector3(0,0.5f,0));
            _floorsList.Add(newSpawnTileCluster);

            NetworkObject networkObject = newSpawnTileCluster.GetComponent<NetworkObject>();
            if (networkObject != null) networkObject.Spawn();
            newSpawnTileCluster.transform.parent = _parentTransform;

            yield return GenerateHexTileMapCoroutine(_spawnClusterRadius, newSpawnTileCluster.transform, _hexTilePrefabList[0]);
        }
    }

        private IEnumerator GenerateHexTileMapCoroutine(int radius, Transform container, GameObject hexTilePrefab)
        {
            int tilesThisFrame = 0;
            int tileSpawnCapPerFrame = 15;
            for (int q = -radius; q <= radius; q++)
            {
                int radiusMin = Mathf.Max(-radius, -q - radius);
                int radiusMax = Mathf.Min(radius, -q + radius);
                for (int r = radiusMin; r <= radiusMax; r++)
                {
                    Vector3 localPosition = HexToLocal(q, r);
                    GameObject newTile = Object.Instantiate(hexTilePrefab, container);
                    newTile.transform.localPosition = localPosition;
                    newTile.name = $"Hex_{q},{r}_" + container.name;

                    NetworkObject networkObject = newTile.GetComponent<NetworkObject>();
                    if (networkObject != null) networkObject.Spawn();
                    newTile.transform.parent = container;
                    _hexTileList.Add(newTile);
                    tilesThisFrame++;

                    if (tilesThisFrame >= tileSpawnCapPerFrame)
                    {
                        tilesThisFrame = 0;
                        yield return null;
                    }
                }
            }
        }

        private Vector3 HexToLocal(int q, int r)
        {
            float x = _tileOffset_X * (q + r / 2f);
            float z = _tileOffset_Z * r;
            return new Vector3(x, 0f, z);
        }

        private void GenerateGameOverTrigger()
        {
            GameObject trigger = Object.Instantiate(_gameOverTrigger, new Vector3(_parentTransform.position.x, _parentTransform.position.y, _parentTransform.position.z), Quaternion.identity);

            NetworkObject networkObject = trigger.GetComponent<NetworkObject>();
            if (networkObject != null)
            {
                networkObject.Spawn();
            }
            _gameOverTriggerTransform = trigger.transform;
            trigger.transform.parent = _parentTransform;
        }

        public List<Vector3> GetPlayerSpawnPoints()
        {
            return _playerSpawnPointsList;
        }

        public List<GameObject> GetFloors()
        {
            return _floorsList;
        }

        private void DestroyAllHexTiles()
        {
            foreach (GameObject hexTile in _hexTileList)
            {
                if (hexTile != null)
                {
                    NetworkObject networkObject = hexTile.GetComponent<NetworkObject>();
                    if (networkObject != null && networkObject.IsSpawned)
                    {
                        networkObject.Despawn();
                    }
                }
            }
            _hexTileList.Clear();
        }

        private void DestroyAllFloors()
        {
            foreach (GameObject floor in _floorsList)
            {
                if (floor != null)
                {
                    NetworkObject networkObject = floor.GetComponent<NetworkObject>();
                    if (networkObject != null && networkObject.IsSpawned)
                    {
                        networkObject.Despawn();
                    }
                }
            }
            _floorsList.Clear();
        }

        private void DestroyAllSpawnClusters()
        {
            foreach (GameObject spawnCluster in _floorsList)
            {
                if (spawnCluster != null)
                {
                    NetworkObject networkObject = spawnCluster.GetComponent<NetworkObject>();
                    if (networkObject != null && networkObject.IsSpawned)
                    {
                        networkObject.Despawn();
                    }
                }
            }
            _floorsList.Clear();
        }

        private void DestroyGameOverTrigger()
        {
            if (_gameOverTriggerTransform.gameObject != null)
            {
                NetworkObject networkObject = _gameOverTriggerTransform.gameObject.GetComponent<NetworkObject>();
                if (networkObject != null && networkObject.IsSpawned)
                {
                    networkObject.Despawn();
                }
            }
        }

        private void DestroyBasePlane()
        {
            if (_basePlaneTransform.gameObject != null)
            {
                NetworkObject networkObject = _basePlaneTransform.gameObject.GetComponent<NetworkObject>();
                if (networkObject != null && networkObject.IsSpawned)
                {
                    networkObject.Despawn();
                }
            }
        }

        private void DestroyLevelContainer()
        {
            if (_parentTransform.gameObject != null)
            {
                NetworkObject networkObject = _parentTransform.gameObject.GetComponent<NetworkObject>();
                if (networkObject != null && networkObject.IsSpawned)
                {
                    networkObject.Despawn();
                }
            }
        }

        public void Dispose()
        {
            if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsServer)
            {
                DestroyAllHexTiles();
                DestroyAllFloors();
                DestroyAllSpawnClusters();
                DestroyGameOverTrigger();
                DestroyBasePlane();
                DestroyLevelContainer();
            }
        }
    }
}

