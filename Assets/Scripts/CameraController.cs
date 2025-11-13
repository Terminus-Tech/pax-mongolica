using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Rigidbody2D playerRb;

    public float speed = 1;

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector2 direction = new Vector2(Mathf.Floor(playerRb.position.x - rb.position.x), Mathf.Floor(playerRb.position.y - rb.position.y));
        rb.linearVelocity = direction.normalized * speed;
    }
}
