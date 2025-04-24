using UnityEngine;
using System.Collections.Generic;

namespace BattleRoyale.Floor
{
    [DefaultExecutionOrder(-15)]
    public class FloorGeneratorService : MonoBehaviour
    {
        [SerializeField] private int _numberOfFloors;
        [SerializeField] private float _heightIncrement;

        public List<GameObject> floorsList = new List<GameObject>();

        void Start()
        {
            GenerateFloors();
        }

        void GenerateFloors()
        {
            for (int i = 1; i <= _numberOfFloors; i++)
            {
                Vector3 position = new Vector3(0, i * _heightIncrement, 0);
                GameObject newFloor = new GameObject("Floor_" + i);

                newFloor.transform.position = position;
                floorsList.Add(newFloor);
            }
        }

        public void DestroyAllFloors()
        {
            foreach (GameObject floor in floorsList)
            {
                Destroy(floor);
            }
            floorsList.Clear();
        }

        public List<GameObject> GetFloors()
        {
            return floorsList;
        }
    }
}
