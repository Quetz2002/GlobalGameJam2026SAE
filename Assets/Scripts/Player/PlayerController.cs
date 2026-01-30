
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
        float h = Input.GetAxisRaw("Horizontal");
        rb.linearVelocity = new Vector2(h * speed, rb.linearVelocity.y);

        if (Input.GetButtonDown("Jump"))
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
