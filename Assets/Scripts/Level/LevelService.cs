using BattleRoyale.Floor;
using BattleRoyale.Tile;
using UnityEngine;
using System.Collections.Generic;

namespace BattleRoyale.Level
{
    public class LevelService : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _floorPrefabList;
        [SerializeField] private float _floorHeightIncrement;
        
        private List<GameObject> _floorsList = new List<GameObject>();

        void Start()
        {
            GenerateFloors();
        }

        public void GenerateFloors()
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
    }
}

