using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;
    public float spawnRate = 1.2f;
    public float minX = -6f;
    public float maxX = 6f;

    float timer;

    void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = spawnRate;
            SpawnObstacle();
        }
    }

    void SpawnObstacle()
    {
        float x = Random.Range(minX, maxX);

        Instantiate(
            obstaclePrefab,
            new Vector3(x, transform.position.y, 0),
            Quaternion.identity
        );
    }
}
