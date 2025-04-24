using BattleRoyale.Floor;
using BattleRoyale.Tile;
using UnityEngine;
using System.Collections.Generic;

namespace BattleRoyale.Level
{
    public class LevelService : MonoBehaviour
    {
        [SerializeField] private FloorGeneratorService floorGeneratorService;
        [SerializeField] private HexTileGeneratorService hexTileGeneratorService;

        private List<GameObject> _floorList = new List<GameObject>();
        void Start()
        {
            CreateFloors();
            CreateFloorTiles();
        }

        void CreateFloors()
        {
            floorGeneratorService.GenerateFloors(transform);
            _floorList = floorGeneratorService.GetFloors();
        }

        void CreateFloorTiles()
        {
            foreach(GameObject floor in _floorList)
            {
                hexTileGeneratorService.GenerateHexTileMap(floor.transform);
            }
        }
    }
}

