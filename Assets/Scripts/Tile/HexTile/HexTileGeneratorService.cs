using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

namespace BattleRoyale.Tile
{
    public class HexTileGeneratorService : MonoBehaviour
    {
        [SerializeField] private Transform containerTransform;
        [SerializeField] private GameObject _hexTilePrefab;
        [SerializeField] private int _mapWidth = 25;
        [SerializeField] private int _mapHeight = 12;
        [SerializeField] private float _tileOffset_X = 1.8f;
        [SerializeField] private float _tileOffset_Z = 1.565f;

        private void Start()
        {
            GenerateHexTileMap(containerTransform);
        }

        public void GenerateHexTileMap(Transform containerTransform)
        {
            float mapMin_X = -_mapWidth / 2;
            float mapMax_X = _mapWidth / 2;
            float mapMin_Z = -_mapHeight / 2;
            float mapMax_Z = _mapHeight / 2;

            for (float i = mapMin_X; i <= mapMax_X; i++)
            {
                for (float j = mapMin_Z; j <= mapMax_Z; j++)
                {
                    GameObject newTile = Instantiate(_hexTilePrefab);
                    Vector3 tilePosition;

                    if (j % 2 == 0)
                    {
                        tilePosition = new Vector3(i * _tileOffset_X, containerTransform.position.y, j * _tileOffset_Z);
                    }
                    else
                    {
                        tilePosition = new Vector3(i * _tileOffset_X + _tileOffset_X / 2, containerTransform.position.y, j * _tileOffset_Z);
                    }

                    StartCoroutine(SetTileData(newTile, i, j, tilePosition, containerTransform));
                }
            }
        }

        IEnumerator SetTileData(GameObject hexTileView,float i, float j, Vector3 tilePosition, Transform containerTransform)
        {
            yield return new WaitForSeconds(0.00001f);
            hexTileView.transform.position = tilePosition;
            hexTileView.transform.parent = containerTransform;
            hexTileView.name = i.ToString()+","+j.ToString();
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
