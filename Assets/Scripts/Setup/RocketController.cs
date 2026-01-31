using UnityEngine;
using UnityEngine.SceneManagement;

public class RocketController : MonoBehaviour
{
    [Header("Vertical Speed")]
    public float baseSpeed = 3f;
    public float maxSpeed = 6f;
    public float minSpeed = 1.5f;

    public float acceleration = 3f;
    public float deceleration = 4f;

    float currentSpeed;

    [Header("Horizontal")]
    public float horizontalSpeed = 6f;

    [Header("Bounds")]
    public float minX = -6f;
    public float maxX = 6f;

    void Start()
    {
        currentSpeed = baseSpeed;
    }

    void Update()
    {
        HandleVerticalSpeed();
        HandleHorizontalMovement();
        MoveUp();
    }

    // ================= SPEED CONTROL =================

    void HandleVerticalSpeed()
    {
        // Acelerar (ej: Shift)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            currentSpeed += acceleration * Time.deltaTime;
            Camera.main.transform.position += Random.insideUnitSphere * 0.02f;
        }
        // Frenar (ej: Ctrl)
        else if (Input.GetKey(KeyCode.LeftControl))
        {
            currentSpeed -= deceleration * Time.deltaTime;
        }
        // Volver suave a velocidad base
        else
        {
            currentSpeed = Mathf.Lerp(currentSpeed, baseSpeed, Time.deltaTime * 2f);
        }

        currentSpeed = Mathf.Clamp(currentSpeed, minSpeed, maxSpeed);
    }

    // ================= HORIZONTAL =================

    void HandleHorizontalMovement()
    {
        float h = Input.GetAxisRaw("Horizontal");

        Vector3 pos = transform.position;
        pos.x += h * horizontalSpeed * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);

        transform.position = pos;
    }

    // ================= MOVE UP =================

    void MoveUp()
    {
        transform.Translate(Vector2.up * currentSpeed * Time.deltaTime);
    }

    // ================= CRASH =================

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Obstacle"))
        {
            Crash();
        }
    }

    void Crash()
    {
        Debug.Log("💥 Rocket crashed!");

        SceneManager.LoadScene(0);
    }
}