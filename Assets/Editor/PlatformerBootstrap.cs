using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class PlatformerBootstrap
{
    [MenuItem("Tools/Platformer/Create FULL Project (Scripts + Prefabs + Level)")]
    public static void CreateFullProject()
    {
        CreateFolders();
        CreateScripts();
        CreatePrefabs();

        AssetDatabase.Refresh();
        Debug.Log("🔥 FULL PLATFORMER PROJECT CREATED!");
    }

    // ================= FOLDERS =================

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
            "Assets/Prefabs/Player",
            "Assets/Prefabs/Enemies",
            "Assets/Prefabs/Level",
            "Assets/Prefabs/Projectiles",
            "Assets/Scenes",
            "Assets/Art",
            "Assets/Audio",
            "Assets/Editor"
        };

        foreach (var f in folders)
            if (!Directory.Exists(f))
                Directory.CreateDirectory(f);
    }

    // ================= SCRIPTS =================

    static void CreateScripts()
    {
        Create("Assets/Scripts/Core/GameManager.cs", GameManagerCode());
        Create("Assets/Scripts/Core/HealthBase.cs", HealthBaseCode());
        Create("Assets/Scripts/Core/PoolManager.cs", PoolManagerCode());

        Create("Assets/Scripts/Player/PlayerController.cs", PlayerController());
        Create("Assets/Scripts/Player/PlayerCombat.cs", PlayerCombat());
        Create("Assets/Scripts/Player/PlayerMeleeCombo.cs", PlayerCombo());

        Create("Assets/Scripts/Enemies/EnemyBase.cs", EnemyBase());

        Create("Assets/Scripts/Camera/SimpleCameraFollow.cs", CameraFollow());

        Create("Assets/Scripts/Setup/GameSetup.cs", GameSetup());
    }

    static void Create(string path, string content)
    {
        if (!File.Exists(path))
            File.WriteAllText(path, content);
    }

    // ================= PREFABS =================

    static void CreatePrefabs()
    {
        // Player
        GameObject player = new GameObject("Player");
        var rb = player.AddComponent<Rigidbody2D>();
        rb.gravityScale = 2;
        rb.freezeRotation = true;

        player.AddComponent<BoxCollider2D>();
        player.AddComponent<PlayerController>();
        player.AddComponent<PlayerCombat>();
        player.AddComponent<PlayerMeleeCombo>();

        SavePrefab(player, "Assets/Prefabs/Player/Player.prefab");

        // Enemy
        GameObject enemy = new GameObject("Enemy");
        var erb = enemy.AddComponent<Rigidbody2D>();
        erb.gravityScale = 2;
        erb.freezeRotation = true;

        enemy.AddComponent<BoxCollider2D>();
        enemy.AddComponent<EnemyBase>();

        SavePrefab(enemy, "Assets/Prefabs/Enemies/Enemy.prefab");

        // Platform
        GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject.DestroyImmediate(platform.GetComponent<BoxCollider>());
        platform.AddComponent<BoxCollider2D>();
        platform.transform.localScale = new Vector3(3, .5f, 1);

        SavePrefab(platform, "Assets/Prefabs/Level/Platform.prefab");

        // Bullet
        GameObject bullet = new GameObject("Bullet");
        bullet.AddComponent<Rigidbody2D>();
        bullet.AddComponent<BoxCollider2D>();

        SavePrefab(bullet, "Assets/Prefabs/Projectiles/Bullet.prefab");

        // Camera
        GameObject cam = new GameObject("Main Camera");
        cam.AddComponent<Camera>().orthographic = true;
        cam.AddComponent<SimpleCameraFollow>();

        SavePrefab(cam, "Assets/Prefabs/Camera.prefab");
        EditorApplication.delayCall += () =>
        {
            SafeDestroy(player);
            SafeDestroy(enemy);
            SafeDestroy(platform);
            SafeDestroy(bullet);
            SafeDestroy(cam);
        };
    }

    static void SavePrefab(GameObject obj, string path)
    {
        PrefabUtility.SaveAsPrefabAsset(obj, path);
    }

    static void SafeDestroy(GameObject obj)
    {
        if (obj == null) return;

        if (Application.isEditor)
            Object.DestroyImmediate(obj);
        else
            Object.Destroy(obj);
    }


    // ================= CORE =================

    static string GameManagerCode() => @"
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void RestartGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}
";

    static string HealthBaseCode() => @"
using UnityEngine;

public class HealthBase : MonoBehaviour
{
    public int maxHp = 3;
    protected int currentHp;

    protected virtual void Awake()
    {
        currentHp = maxHp;
    }

    public virtual void TakeDamage(int amount)
    {
        currentHp -= amount;
        if (currentHp <= 0)
            Die();
    }

    protected virtual void Die(){}
}
";

    static string PoolManagerCode() => @"
using UnityEngine;
using System.Collections.Generic;

public class PoolManager : MonoBehaviour
{
    public static PoolManager Instance;

    Dictionary<GameObject, Queue<GameObject>> pools = new();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public GameObject Spawn(GameObject prefab, Vector3 pos, Quaternion rot)
    {
        if (!pools.ContainsKey(prefab))
            pools[prefab] = new Queue<GameObject>();

        if (pools[prefab].Count == 0)
            return Instantiate(prefab, pos, rot);

        GameObject go = pools[prefab].Dequeue();
        go.transform.SetPositionAndRotation(pos, rot);
        go.SetActive(true);
        return go;
    }

    public void Despawn(GameObject prefab, GameObject obj)
    {
        obj.SetActive(false);
        pools[prefab].Enqueue(obj);
    }
}
";

    // ================= PLAYER =================

    static string PlayerController() => @"
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed = 8f;
    public float jumpForce = 14f;

    public float laneDistance = 1.5f;
    int lane = 0;

    Rigidbody2D rb;

    void Awake(){ rb = GetComponent<Rigidbody2D>(); }

    void Update()
    {
        float h = Input.GetAxisRaw(""Horizontal"");
        rb.velocity = new Vector2(h * speed, rb.velocity.y);

        if (Input.GetButtonDown(""Jump""))
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

        if (Input.GetKeyDown(KeyCode.UpArrow)) ChangeLane(1);
        if (Input.GetKeyDown(KeyCode.DownArrow)) ChangeLane(-1);
    }

    void ChangeLane(int dir)
    {
        lane = Mathf.Clamp(lane + dir, -1, 1);
        Vector3 p = transform.position;
        p.z = lane * laneDistance;
        transform.position = p;
    }
}
";

    static string PlayerCombat() => @"
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float speed = 15f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
            Shoot();
    }

    void Shoot()
    {
        if(!bulletPrefab || !firePoint) return;

        GameObject b = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = b.GetComponent<Rigidbody2D>();

        float dir = transform.localScale.x > 0 ? 1 : -1;
        rb.velocity = Vector2.right * speed * dir;
    }
}
";

    static string PlayerCombo() => @"
using UnityEngine;

public class PlayerMeleeCombo : MonoBehaviour
{
    public Transform point;
    public float radius = .6f;
    public LayerMask enemyLayer;

    int combo;
    float timer;
    public float reset = .6f;

    void Update()
    {
        timer -= Time.deltaTime;
        if(timer <= 0) combo = 0;

        if(Input.GetKeyDown(KeyCode.C))
            Attack();
    }

    void Attack()
    {
        timer = reset;
        combo = (combo + 1) % 3;

        Collider2D[] hits = Physics2D.OverlapCircleAll(point.position, radius, enemyLayer);
        foreach(var h in hits)
        {
            EnemyBase e = h.GetComponent<EnemyBase>();
            if(e) e.TakeDamage(1);
        }
    }
}
";

    // ================= ENEMY =================

    static string EnemyBase() => @"
using UnityEngine;

public class EnemyBase : HealthBase
{
    public float speed = 2f;
    public Transform leftPoint;
    public Transform rightPoint;

    bool right = true;
    Rigidbody2D rb;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float dir = right ? 1 : -1;
        rb.velocity = new Vector2(dir * speed, rb.velocity.y);

        if(right && transform.position.x >= rightPoint.position.x) right = false;
        if(!right && transform.position.x <= leftPoint.position.x) right = true;
    }

    public override void TakeDamage(int dmg)
    {
        base.TakeDamage(dmg);
    }
}
";

    // ================= CAMERA =================

    static string CameraFollow() => @"
using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(3,1,-10);
    public float smooth = 5f;

    void LateUpdate()
    {
        if(!target) return;
        transform.position = Vector3.Lerp(transform.position, target.position + offset, smooth * Time.deltaTime);
    }
}
";

    // ================= SETUP =================

    static string GameSetup() => @"
using UnityEngine;

public class GameSetup : MonoBehaviour
{
    public int sections = 5;
    public float length = 12f;
    public float laneDistance = 1.5f;
    public int enemiesPerSection = 2;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject platformPrefab;

    void Start()
    {
        Instantiate(playerPrefab, new Vector3(0,2,0), Quaternion.identity);
        Camera.main.GetComponent<SimpleCameraFollow>().target = GameObject.Find(""Player"").transform;

        float x = 0;

        for(int i=0;i<sections;i++)
        {
            for(int lane=-1; lane<=1; lane++)
            {
                GameObject g = Instantiate(platformPrefab);
                g.transform.position = new Vector3(x + length/2, 0, lane * laneDistance);
                g.transform.localScale = new Vector3(length,.5f,1);
            }

            for(int e=0;e<enemiesPerSection;e++)
            {
                Instantiate(enemyPrefab,
                    new Vector3(x + Random.Range(2,length-2),1, Random.Range(-1,2)*laneDistance),
                    Quaternion.identity);
            }

            x += length;
        }
    }
}
";
}
