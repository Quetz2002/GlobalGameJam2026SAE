using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed = 3f;

    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);

        if (transform.position.y < -12f)
            Destroy(gameObject);
    }

    void Awake()
    {
        gameObject.tag = "Obstacle";
    }
}
