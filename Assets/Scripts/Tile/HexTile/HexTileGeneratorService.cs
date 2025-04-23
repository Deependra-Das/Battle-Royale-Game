using System.Collections.Generic;
using System.Collections;
using UnityEngine;

namespace BattleRoyale.Tile
{
    [DefaultExecutionOrder(-10)]
    public class HexTileGeneratorService : MonoBehaviour
    {
        [SerializeField] private HexTileView hexTilePrefab;
        [SerializeField] private Transform hexTileContainerTransform;
        [SerializeField] private int mapWidth = 25;
        [SerializeField] private int mapHeight = 12;
        [SerializeField] private float tileOffset_X = 1.8f;
        [SerializeField] private float tileOffset_Z = 1.565f;
        

        private List<HexTileView> _tileViewList;

        void Start()
        {
            _tileViewList = new List<HexTileView>();
            CreateHexTileMap();
        }

        void CreateHexTileMap()
        {
            float mapMin_X = -mapWidth / 2;
            float mapMax_X = mapWidth / 2;
            float mapMin_Z = -mapHeight / 2;
            float mapMax_Z = mapHeight / 2;

            for (float i = mapMin_X; i <= mapMax_X; i++)
            {
                for (float j = mapMin_Z; j <= mapMax_Z; j++)
                {
                    HexTileView newTile = Object.Instantiate(hexTilePrefab);
                    Vector3 tilePosition;

                    if (j % 2 == 0)
                    {
                        tilePosition = new Vector3(i * tileOffset_X, 0, j * tileOffset_Z);
                    }
                    else
                    {
                        tilePosition = new Vector3(i * tileOffset_X + tileOffset_X / 2, 0, j * tileOffset_Z);
                    }

                    StartCoroutine(SetTileData(newTile, i, j, tilePosition));
                    _tileViewList.Add(newTile);
                }
            }
        }

        IEnumerator SetTileData(HexTileView hexTileView,float i, float j, Vector3 tilePosition)
        {
            yield return new WaitForSeconds(0.00001f);
            hexTileView.gameObject.transform.position = tilePosition;
            hexTileView.gameObject.transform.parent = hexTileContainerTransform;
            hexTileView.gameObject.name = i.ToString()+","+j.ToString();
        }

        void OnTriggerExit(Collider other)
        {
            HexTileView hexTileViewObj = other.GetComponent<HexTileView>();

            if(hexTileViewObj != null)
            {
                Destroy(other.gameObject);
            }
        }
    }
}
