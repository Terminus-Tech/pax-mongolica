using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;

    Vector2 inputDir = Vector2.zero;
    Vector2 gridPos = Vector2.zero;
    float moveTime = 0;
    bool isMoving = false;
    public float speed = 1;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isMoving)
        {
            rb.linearVelocity = Vector2.zero;
            inputDir = Vector2.zero;
            gridPos = new Vector2(Mathf.Round(rb.transform.position.x), Mathf.Round(rb.transform.position.y));
            rb.transform.position = gridPos;
        }

        rb.MovePosition(Vector2.Lerp(gridPos, gridPos + inputDir, (Time.time - moveTime) * speed));

        if (Keyboard.current.anyKey.IsActuated() && !isMoving)
        {
            
            if ((Keyboard.current.aKey.IsActuated() || Keyboard.current.leftArrowKey.IsActuated()) && inputDir.x > -1)
            {
                inputDir.x = -1;
                moveTime = Time.time;
                isMoving = true;
            }
            if ((Keyboard.current.dKey.IsActuated() || Keyboard.current.rightArrowKey.IsActuated()) && inputDir.x < 1)
            {
                inputDir.x = 1;
                moveTime = Time.time;
                isMoving = true;
            }
            if ((Keyboard.current.wKey.IsActuated() || Keyboard.current.upArrowKey.IsActuated()) && inputDir.y < 1)
            {
                inputDir.y = 1;
                moveTime = Time.time;
                isMoving = true;
            }
            if ((Keyboard.current.sKey.IsActuated() || Keyboard.current.downArrowKey.IsActuated()) && inputDir.y > -1)
            {
                inputDir.y = -1;
                moveTime = Time.time;
                isMoving = true;
            }
            
        }

        if (Vector2.Distance(rb.transform.position, gridPos + inputDir) < 0.1f)
        {
            isMoving = false;
        }

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            Debug.Log("Collided with wall!");
            isMoving = false;
        }
    }
}
