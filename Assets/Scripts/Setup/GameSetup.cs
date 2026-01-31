using UnityEngine;

public class GameSetup : MonoBehaviour
{
    [Header("Level")]
    public int sections = 5;
    public float sectionLength = 12f;
    public float laneDistance = 1.5f;

    [Header("Prefabs (Optional)")]
    public GameObject playerPrefab;
    public GameObject groundPrefab;
    public GameObject platformPrefab;
    public GameObject enemyPrefab;

    [Header("Enemies")]
    public int enemiesPerSection = 2;

    void Start()
    {
        SetupPlayer();
        SetupCamera();
        GenerateLevel();

        // Asegurar follow después de todo
        Invoke(nameof(AssignCameraTarget), 0.1f);
    }


    // ================= PLAYER =================

    GameObject playerInstance;

    void SetupPlayer()
    {
        if (playerPrefab != null)
        {
            playerInstance = Instantiate(playerPrefab, new Vector3(0, 2, 0), Quaternion.identity);
        }
        else
        {
            playerInstance = new GameObject("Player");

            playerInstance.transform.position = new Vector3(0, 2, 0);

            var rb = playerInstance.AddComponent<Rigidbody2D>();
            rb.gravityScale = 2;
            rb.freezeRotation = true;

            playerInstance.AddComponent<BoxCollider2D>();
            playerInstance.AddComponent<PlayerController>();
            playerInstance.AddComponent<PlayerCombat>();
            playerInstance.AddComponent<PlayerMeleeCombo>();
        }

        playerInstance.tag = "Player";
    }


    // ================= CAMERA =================

    void SetupCamera()
    {
        GameObject camObj;

        if (Camera.main != null)
        {
            camObj = Camera.main.gameObject;
        }
        else
        {
            camObj = new GameObject("Main Camera");
            Camera cam = camObj.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = 5;
            camObj.tag = "MainCamera";
        }

        SimpleCameraFollow follow = camObj.GetComponent<SimpleCameraFollow>();
        if (!follow)
            follow = camObj.AddComponent<SimpleCameraFollow>();

        if (playerInstance != null)
            follow.target = playerInstance.transform;
        else
            Debug.LogError("Player instance not created!");
    }



    // ================= LEVEL =================

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
        // ---------- Ground per lane ----------
        for (int lane = -1; lane <= 1; lane++)
        {
            GameObject ground;

            if (groundPrefab != null)
            {
                ground = Instantiate(groundPrefab);
                Debug.Log("Spawned prefab: " + ground.name);


                // Reset seguro (MUY importante con prefabs)
                ground.transform.position = Vector3.zero;
                ground.transform.rotation = Quaternion.identity;

                // Forzar física correcta si tiene Rigidbody
                Rigidbody2D rb = ground.GetComponent<Rigidbody2D>();
                if (rb != null)
                    rb.bodyType = RigidbodyType2D.Static;
            }
            else
            {
                ground = new GameObject($"Ground_Lane_{lane}");

                BoxCollider2D col = ground.AddComponent<BoxCollider2D>();
                col.size = new Vector2(sectionLength, 0.5f);

                Rigidbody2D rb = ground.AddComponent<Rigidbody2D>();
                rb.bodyType = RigidbodyType2D.Static;
            }

            // Colocación FINAL (para ambos casos)
            ground.transform.localScale = new Vector3(sectionLength, 0.5f, 1);
            ground.transform.position = new Vector3(
                startX + sectionLength / 2,
                0,
                lane * laneDistance
            );

        }

        // ---------- Upper platform ----------
        GameObject platform;

        if (platformPrefab != null)
{
    platform = Instantiate(platformPrefab);
            Debug.Log("Spawned prefab: " + platform.name);


            // Reset seguro de transform
            platform.transform.position = Vector3.zero;
    platform.transform.rotation = Quaternion.identity;

    // Forzar física correcta si tiene Rigidbody
    Rigidbody2D rb = platform.GetComponent<Rigidbody2D>();
    if (rb != null)
        rb.bodyType = RigidbodyType2D.Static;
}
else
{
    platform = new GameObject("Platform");

    BoxCollider2D pCol = platform.AddComponent<BoxCollider2D>();
    pCol.size = new Vector2(3, 0.4f);

    Rigidbody2D rb = platform.AddComponent<Rigidbody2D>();
    rb.bodyType = RigidbodyType2D.Static;
}

// Colocación FINAL
platform.transform.localScale = new Vector3(3, 0.4f, 1);
platform.transform.position = new Vector3(
    startX + Random.Range(3, sectionLength - 3),
    2.5f,
    0
);

    }

    // ================= ENEMIES =================

    void SpawnEnemies(float sectionStart)
    {
        for (int i = 0; i < enemiesPerSection; i++)
        {
            int lane = Random.Range(-1, 2);

            GameObject enemy;

            if (enemyPrefab != null)
            {
                enemy = Instantiate(enemyPrefab);
                Debug.Log("Spawned enemy prefab");


                // Reset seguro de transform
                enemy.transform.position = Vector3.zero;
                enemy.transform.rotation = Quaternion.identity;

                // Forzar física correcta si tiene Rigidbody
                Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.gravityScale = 2;
                    rb.freezeRotation = true;
                }

                // Validar EnemyBase
                EnemyBase eb = enemy.GetComponent<EnemyBase>();
                if (eb == null)
                    eb = enemy.AddComponent<EnemyBase>();

                // Validar puntos de patrulla
                if (eb.leftPoint == null || eb.rightPoint == null)
                {
                    GameObject left = new GameObject("LeftPoint");
                    GameObject right = new GameObject("RightPoint");

                    left.transform.SetParent(enemy.transform);
                    right.transform.SetParent(enemy.transform);

                    left.transform.localPosition = Vector3.left * 2;
                    right.transform.localPosition = Vector3.right * 2;

                    eb.leftPoint = left.transform;
                    eb.rightPoint = right.transform;
                }
            }
            else
            {
                enemy = new GameObject("Enemy");

                var rb = enemy.AddComponent<Rigidbody2D>();
                rb.gravityScale = 2;
                rb.freezeRotation = true;

                enemy.AddComponent<BoxCollider2D>();

                EnemyBase eb = enemy.AddComponent<EnemyBase>();

                GameObject left = new GameObject("LeftPoint");
                GameObject right = new GameObject("RightPoint");

                left.transform.SetParent(enemy.transform);
                right.transform.SetParent(enemy.transform);

                left.transform.localPosition = Vector3.left * 2;
                right.transform.localPosition = Vector3.right * 2;

                eb.leftPoint = left.transform;
                eb.rightPoint = right.transform;
            }

            // Posición FINAL (para ambos casos)
            enemy.transform.position = new Vector3(
                sectionStart + Random.Range(2f, sectionLength - 2f),
                1,
                lane * laneDistance
            );
        }
    }

    void AssignCameraTarget()
    {
        SimpleCameraFollow follow = Camera.main.GetComponent<SimpleCameraFollow>();

        if (follow == null)
        {
            Debug.LogError("SimpleCameraFollow missing on camera!");
            return;
        }

        GameObject player = GameObject.Find("Player");

        if (player != null)
        {
            follow.target = player.transform;
        }
        else
        {
            Debug.LogError("Player not found for camera follow!");
        }
    }


}
