using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Animator animator;

    public Rigidbody2D rb;
    public GameObject gameController;
    public AudioSource footstepAudio;

    public Vector2 inputDir = Vector2.zero;
    public Vector2 gridPos = Vector2.zero;

    public float moveTime = 0;
    public bool isMoving = false;
    public float speed = 1;
    public bool moveable = false;
    public int rations;

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

        if (Keyboard.current.anyKey.IsActuated() && !isMoving && moveable)
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
            if (isMoving && SceneManager.GetActiveScene().name == "MainScene")
            {
                rations--;
            }
            isMoving = false;
        }

        if (rations <= 0 && moveable && SceneManager.GetActiveScene().name == "MainScene")
        {
            gameController.GetComponent<GameController>().Encounter("lose", new string[] { "You lose...\n>", "Press any key to try again" });
        }

        if (isMoving)
        {
            animator.SetBool("Walking", true);
            if (inputDir.x > 0)
            {
                rb.transform.localScale = new Vector3(1, 1, 1);
            }
            else if (inputDir.x < 0)
            {
                rb.transform.localScale = new Vector3(-1, 1, 1);
            }
            if (!footstepAudio.isPlaying)
            {
                footstepAudio.Play();
            }
        }
        else
        {
            animator.SetBool("Walking", false);
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Wall") || collision.gameObject.layer == LayerMask.NameToLayer("Bound"))
        {
            Debug.Log("Collided with wall/bound!");
            rb.transform.position = gridPos;
            isMoving = false;
        }
    }

}
