using UnityEngine;
using System.Collections.Generic;

namespace BattleRoyale.FloorModule
{
    public class FloorGeneratorService : MonoBehaviour
    {
        [SerializeField] private List<GameObject> _floorPrefabList;
        [SerializeField] private int _numberOfFloors;
        [SerializeField] private float _heightIncrement;

        private List<GameObject> _floorsList = new List<GameObject>();

        public void GenerateFloors(Transform parentTransform)
        {
            for (int i = 1; i <= _numberOfFloors; i++)
            {
                Vector3 position = new Vector3(0, i * _heightIncrement, 0);
                GameObject newFloor = new GameObject("Floor_" + i);

                newFloor.transform.position = position;
                newFloor.transform.parent = parentTransform;
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
