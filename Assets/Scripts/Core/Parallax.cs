using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField] private Vector2 Velocity;

    private Vector2 offset;

    private Material material;

    private Rigidbody2D RB;

    private void Awake()
    {
        material = GetComponent<SpriteRenderer>().material;
        RB = GameObject.FindGameObjectWithTag("Player").GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        offset = (RB.linearVelocity.x * 0.1f) * Velocity * Time.deltaTime;
        material.mainTextureOffset += offset;
    }
}
