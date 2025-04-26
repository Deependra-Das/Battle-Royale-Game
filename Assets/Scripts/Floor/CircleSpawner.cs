using UnityEngine;

public class CircleSpawner : MonoBehaviour
{
    public GameObject objectToSpawn;
    public float radius = 5f;
    public int numberOfObjects = 10;

    void Start()
    {
        SpawnObjectsOnCircle();
    }

    void SpawnObjectsOnCircle()
    {
        float angleStep = 360f / numberOfObjects;

        for (int i = 0; i < numberOfObjects; i++)
        {
            float angleInRadians = Mathf.Deg2Rad * angleStep * i;
            float x = Mathf.Cos(angleInRadians) * radius;
            float z = Mathf.Sin(angleInRadians) * radius;
            Vector3 spawnPosition = new Vector3(x, 0f, z);
            Instantiate(objectToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}
