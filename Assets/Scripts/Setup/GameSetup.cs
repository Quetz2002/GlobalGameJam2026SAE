using UnityEngine;

public class GameSetup : MonoBehaviour
{
    [Header("Level")]
    public int sections = 5;
    public float sectionLength = 12f;
    public float laneDistance = 1.5f;

    [Header("Enemies")]
    public int enemiesPerSection = 2;

    void Start()
    {
        SetupPlayer();
        SetupCamera();
        GenerateLevel();
    }

    void SetupPlayer()
    {
        GameObject player = new GameObject("Player");

        player.transform.position = new Vector3(0, 2, 0);

        var rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 2;
        rb.freezeRotation = true;

        player.AddComponent<BoxCollider2D>();
        player.AddComponent<PlayerController>();
        player.AddComponent<PlayerCombat>();
        player.AddComponent<PlayerMeleeCombo>();

        player.tag = "Player";
    }

    void SetupCamera()
    {
        GameObject camObj = Camera.main != null
            ? Camera.main.gameObject
            : new GameObject("Main Camera");

        Camera cam = camObj.GetComponent<Camera>() ?? camObj.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5;

        var follow = camObj.AddComponent<SimpleCameraFollow>();
        follow.target = GameObject.Find("Player").transform;
    }

    void GenerateLevel()
    {
        float xPos = 0;

        for (int i = 0; i < sections; i++)
        {
            CreateSection(xPos);
            SpawnEnemies(xPos);
            xPos += sectionLength;
        }
    }

    void CreateSection(float startX)
    {
        for (int lane = -1; lane <= 1; lane++)
        {
            GameObject ground = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ground.name = $"Ground_Lane_{lane}";

            ground.transform.localScale = new Vector3(sectionLength, 0.5f, 1);
            ground.transform.position = new Vector3(
                startX + sectionLength / 2,
                0,
                lane * laneDistance
            );

            Destroy(ground.GetComponent<BoxCollider>());
            ground.AddComponent<BoxCollider2D>();
        }

        // Plataformas arriba
        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        platform.transform.localScale = new Vector3(3, 0.4f, 1);
        platform.transform.position = new Vector3(
            startX + Random.Range(3, sectionLength - 3),
            2.5f,
            0
        );

        Destroy(platform.GetComponent<BoxCollider>());
        platform.AddComponent<BoxCollider2D>();
    }

    void SpawnEnemies(float sectionStart)
    {
        for (int i = 0; i < enemiesPerSection; i++)
        {
            int lane = Random.Range(-1, 2);

            GameObject enemy = new GameObject("Enemy");

            enemy.transform.position = new Vector3(
                sectionStart + Random.Range(2f, sectionLength - 2f),
                1,
                lane * laneDistance
            );

            var rb = enemy.AddComponent<Rigidbody2D>();
            rb.gravityScale = 2;
            rb.freezeRotation = true;

            enemy.AddComponent<BoxCollider2D>();

            EnemyBase eb = enemy.AddComponent<EnemyBase>();

            GameObject left = new GameObject("LeftPoint");
            GameObject right = new GameObject("RightPoint");

            left.transform.position = enemy.transform.position + Vector3.left * 2;
            right.transform.position = enemy.transform.position + Vector3.right * 2;

            eb.leftPoint = left.transform;
            eb.rightPoint = right.transform;
        }
    }
}
