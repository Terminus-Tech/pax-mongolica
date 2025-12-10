using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Rigidbody2D rb;
    public Rigidbody2D playerRb;

    public float speed;

    // Update is called once per frame
    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(Mathf.Clamp((playerRb.position.x - rb.position.x), -speed, speed), Mathf.Clamp((playerRb.position.y - rb.position.y), -speed, speed));
    }
}
