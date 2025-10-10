using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace BattleRoyale.UtilitiesModule
{
    public class HexTileGenerator : MonoBehaviour
    {
        //public GameObject[] objectToSpawn;
        //public int[] numberOfObjects;
        //public float[] radi;

        //void Start()
        //{
        //    for (int i = 0; i < objectToSpawn.Length; i++)
        //    {
        //        SpawnObjectsAround(objectToSpawn[i], radi[i], numberOfObjects[i]);
        //    }
        //}

        //void SpawnObjectsAround(GameObject pillar, float radius, int numberOfObjects)
        //{
        //    float angleStep = 360f / numberOfObjects;

        //    for (int i = 0; i < numberOfObjects; i++)
        //    {
        //        float angle = i * angleStep;
        //        float radian = angle * Mathf.Deg2Rad;

        //        Vector3 spawnPosition = new Vector3(
        //            Mathf.Cos(radian) * radius,
        //            0f,
        //            Mathf.Sin(radian) * radius
        //        );

        //        Instantiate(pillar, transform.position + spawnPosition, Quaternion.identity);
        //    }
        //}


        //public GameObject hexTilePrefab;
        //public Transform holder;

        //[SerializeField] int mapWidth = 25;
        //[SerializeField] int mapHeight = 12;

        //[SerializeField] float tileXOffset = 4.0f;
        //[SerializeField] float tileZOffset = 3.45f;

        //private void Start()
        //{
        //    CreateHexTileMap();
        //}

        //private void CreateHexTileMap()
        //{
        //    float mapxMin = -mapWidth/2;
        //    float mapXMax = mapWidth/2 ;

        //    float mapZMin = -mapHeight / 2;
        //    float mapZMax = mapHeight/2 ;

        //    for(float x = mapxMin; x <= mapXMax; x++)
        //    {
        //        for (float z = mapZMin; z <= mapZMax; z++)
        //        {
        //            GameObject tempGO = Instantiate(hexTilePrefab);

        //            Vector3 pos;

        //            if (z % 2 == 0)
        //            {
        //                pos = new Vector3(x * tileXOffset,0, z * tileZOffset);
        //            }
        //            else
        //            {
        //                pos = new Vector3(x * tileXOffset + tileXOffset / 2, 0, z * tileZOffset);
        //            }

        //            StartCoroutine(SetTileInfo(tempGO, x, z, pos));
        //        }
        //    }
        //}

        //IEnumerator SetTileInfo(GameObject obj, float x, float z, Vector3 pos)
        //{
        //    yield return new WaitForSeconds(0.0001f);
        //    obj.transform.parent = holder;
        //    obj.name = "Tile_" + x.ToString() + "_"+ z.ToString();
        //    obj.transform.position = pos;
        //}

        //private void OnTriggerExit(Collider other)
        //{
        //    Destroy(other.gameObject);
        //}

        public GameObject containerPrefab;
        public GameObject hexTilePrefab;
        public float tileOffset_X = 1f;
        public float tileOffset_Z = 1f;
        public int mapFloorRadius = 5;
        public float holeRadius = 2f;
        public float minVerticalScale = 100f;
        public float maxVerticalScale = 500f;

        private List<GameObject> _containersList = new List<GameObject>();
        private List<GameObject> _hexTileList = new List<GameObject>();

        private void Start()
        {
            StartCoroutine(GenerateLevelCoroutine());
        }

        private IEnumerator GenerateLevelCoroutine()
        {
            yield return GenerateContainerCoroutine();

            foreach (var container in _containersList)
            {
                yield return GenerateHexTilesForContainer(container.transform);
            }
        }

        private IEnumerator GenerateContainerCoroutine()
        {
            Vector3 position = Vector3.zero;
            GameObject newContainer = Instantiate(containerPrefab, position, Quaternion.identity);
            newContainer.name = "Container";
            _containersList.Add(newContainer);
            yield return null;
        }

        private IEnumerator GenerateHexTilesForContainer(Transform containerTransform)
        {
            int tilesThisFrame = 0;
            int tileSpawnCapPerFrame = 15;

            for (int q = -mapFloorRadius; q <= mapFloorRadius; q++)
            {
                int radiusMin = Mathf.Max(-mapFloorRadius, -q - mapFloorRadius);
                int radiusMax = Mathf.Min(mapFloorRadius, -q + mapFloorRadius);
                for (int r = radiusMin; r <= radiusMax; r++)
                {
                    Vector3 localPosition = HexToLocal(q, r);

                    float distanceFromCenter = Mathf.Sqrt(localPosition.x * localPosition.x + localPosition.z * localPosition.z);
                    if (distanceFromCenter < holeRadius)
                    {
                        continue;
                    }

                    GameObject newTile = Instantiate(hexTilePrefab, containerTransform);
                    newTile.transform.localPosition = localPosition;
                    newTile.name = $"Hex_Pillar({q},{r})";

                    float randomVerticalScale = Random.Range(minVerticalScale, maxVerticalScale);
                    newTile.transform.localScale = new Vector3(2f, randomVerticalScale, 2f);

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
            float x = tileOffset_X * (q + r / 2f);
            float z = tileOffset_Z * r;
            return new Vector3(x, 0f, z);
        }

        public List<GameObject> GetContainers()
        {
            return _containersList;
        }

        public List<GameObject> GetHexTiles()
        {
            return _hexTileList;
        }
    }
}