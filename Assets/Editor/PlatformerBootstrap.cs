using UnityEditor;
using UnityEngine;
using System.IO;

public class PlatformerBootstrap
{
    [MenuItem("Tools/Platformer/Create Full Project Setup")]
    public static void CreateProject()
    {
        CreateFolders();
        CreateScripts();

        AssetDatabase.Refresh();
        Debug.Log("✅ Platformer project fully bootstrapped!");
    }

    static void CreateFolders()
    {
        string[] folders =
        {
            "Assets/Scripts",
            "Assets/Scripts/Core",
            "Assets/Scripts/Player",
            "Assets/Scripts/Enemies",
            "Assets/Scripts/Camera",
            "Assets/Scripts/Setup",
            "Assets/Prefabs",
            "Assets/Scenes",
            "Assets/Art",
            "Assets/Audio"
        };

        foreach (var folder in folders)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }
    }

    static void CreateScripts()
    {
        CreateScript("Assets/Scripts/Player/PlayerController.cs", PlayerControllerCode());
        CreateScript("Assets/Scripts/Player/PlayerCombat.cs", PlayerCombatCode());
        CreateScript("Assets/Scripts/Player/PlayerMeleeCombo.cs", PlayerMeleeComboCode());
        CreateScript("Assets/Scripts/Enemies/EnemyBase.cs", EnemyBaseCode());
        CreateScript("Assets/Scripts/Camera/SimpleCameraFollow.cs", CameraCode());
        CreateScript("Assets/Scripts/Setup/GameSetup.cs", GameSetupCode());
    }

    static void CreateScript(string path, string content)
    {
        if (File.Exists(path)) return;
        File.WriteAllText(path, content);
    }

    // ================= CODE TEMPLATES =================

    static string PlayerControllerCode() => @"
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 8f;
    public float jumpForce = 14f;

    public float laneDistance = 1.5f;
    int currentLane = 0;

    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float h = Input.GetAxisRaw(""Horizontal"");
        rb.velocity = new Vector2(h * speed, rb.velocity.y);

        if (Input.GetButtonDown(""Jump""))
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        if (Input.GetKeyDown(KeyCode.UpArrow))
            ChangeLane(1);

        if (Input.GetKeyDown(KeyCode.DownArrow))
            ChangeLane(-1);
    }

    void ChangeLane(int dir)
    {
        currentLane = Mathf.Clamp(currentLane + dir, -1, 1);
        Vector3 pos = transform.position;
        pos.z = currentLane * laneDistance;
        transform.position = pos;
    }
}
";

    static string PlayerCombatCode() => @"
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float bulletSpeed = 15f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
            Shoot();
    }

    void Shoot()
    {
        if (!bulletPrefab || !firePoint) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        float dir = transform.localScale.x > 0 ? 1 : -1;
        rb.velocity = Vector2.right * bulletSpeed * dir;
    }
}
";

    static string PlayerMeleeComboCode() => @"
using UnityEngine;

public class PlayerMeleeCombo : MonoBehaviour
{
    public Transform meleePoint;
    public float radius = 0.6f;
    public LayerMask enemyLayer;

    int combo = 0;
    float timer;
    public float resetTime = 0.6f;

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0) combo = 0;

        if (Input.GetKeyDown(KeyCode.C))
            Attack();
    }

    void Attack()
    {
        timer = resetTime;
        combo = (combo + 1) % 3;

        Collider2D[] hits = Physics2D.OverlapCircleAll(meleePoint.position, radius, enemyLayer);

        foreach (var hit in hits)
        {
            EnemyBase e = hit.GetComponent<EnemyBase>();
            if (e) e.TakeDamage(1);
        }
    }
}
";

    static string EnemyBaseCode() => @"
using UnityEngine;

public class EnemyBase : MonoBehaviour
{
    public int hp = 3;
    public float speed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;

    bool right = true;
    Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        float dir = right ? 1 : -1;
        rb.velocity = new Vector2(dir * speed, rb.velocity.y);

        if (right && transform.position.x >= rightPoint.position.x)
            right = false;

        if (!right && transform.position.x <= leftPoint.position.x)
            right = true;
    }

    public void TakeDamage(int dmg)
    {
        hp -= dmg;
        if (hp <= 0) Destroy(gameObject);
    }
}
";

    static string CameraCode() => @"
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(3,1,-10);
    public float smooth = 5f;

    void LateUpdate()
    {
        if (!target) return;
        transform.position = Vector3.Lerp(transform.position, target.position + offset, smooth * Time.deltaTime);
    }
}
";

    static string GameSetupCode() => @"
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public int sections = 5;
    public float sectionLength = 12f;
    public float laneDistance = 1.5f;
    public int enemiesPerSection = 2;

    void Start()
    {
        CreatePlayer();
        SetupCamera();
        GenerateLevel();
    }

    void CreatePlayer()
    {
        GameObject p = new GameObject(""Player"");
        p.transform.position = new Vector3(0,2,0);

        var rb = p.AddComponent<Rigidbody2D>();
        rb.gravityScale = 2;
        rb.freezeRotation = true;

        p.AddComponent<BoxCollider2D>();
        p.AddComponent<PlayerController>();
        p.AddComponent<PlayerCombat>();
        p.AddComponent<PlayerMeleeCombo>();
    }

    void SetupCamera()
    {
        Camera cam = Camera.main ?? new GameObject(""Main Camera"").AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = 5;

        var follow = cam.gameObject.AddComponent<SimpleCameraFollow>();
        follow.target = GameObject.Find(""Player"").transform;
    }

    void GenerateLevel()
    {
        float x = 0;

        for(int i=0;i<sections;i++)
        {
            for(int lane=-1; lane<=1; lane++)
            {
                GameObject g = GameObject.CreatePrimitive(PrimitiveType.Cube);
                g.transform.localScale = new Vector3(sectionLength, .5f, 1);
                g.transform.position = new Vector3(x + sectionLength/2, 0, lane * laneDistance);

                Destroy(g.GetComponent<BoxCollider>());
                g.AddComponent<BoxCollider2D>();
            }

            SpawnEnemies(x);
            x += sectionLength;
        }
    }

    void SpawnEnemies(float startX)
    {
        for(int i=0;i<enemiesPerSection;i++)
        {
            GameObject e = new GameObject(""Enemy"");

            e.transform.position = new Vector3(
                startX + Random.Range(2, sectionLength-2),
                1,
                Random.Range(-1,2) * laneDistance
            );

            var rb = e.AddComponent<Rigidbody2D>();
            rb.gravityScale = 2;
            rb.freezeRotation = true;

            e.AddComponent<BoxCollider2D>();

            EnemyBase eb = e.AddComponent<EnemyBase>();

            GameObject l = new GameObject(""LeftPoint"");
            GameObject r = new GameObject(""RightPoint"");

            l.transform.position = e.transform.position + Vector3.left * 2;
            r.transform.position = e.transform.position + Vector3.right * 2;

            eb.leftPoint = l.transform;
            eb.rightPoint = r.transform;
        }
    }
}
";
}
