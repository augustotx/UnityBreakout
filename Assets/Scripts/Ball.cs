using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    private Rigidbody2D rb2d;
    private float bumpForce = 5.0f;
    private Vector2 currentVelocity;
    private Vector2 startPos;
    GameObject playerObj;

    [Header("UI Reference")]
    public Text livesText;

    private int lives = 3;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        currentVelocity = Vector2.zero;
        playerObj = GameObject.FindWithTag("Player");
        startPos = transform.position - playerObj.transform.position;
        UpdateLivesUI();
    }

    void resetBall()
    {
        rb2d.linearVelocity = Vector2.zero;
        transform.position = (Vector2)playerObj.transform.position + startPos;

    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        Vector2 vel = Vector2.zero;
        if (coll.collider.CompareTag("Player"))
        {
            var playerPos = coll.collider.attachedRigidbody.transform.position;
            var dir = transform.position - playerPos;
            dir.Normalize();

            Vector2 ballforce = bumpForce * dir;
            rb2d.linearVelocity = ballforce;
            return;
        }
        else if ((coll.collider.CompareTag("LeftBound") || coll.collider.CompareTag("RightBound")))
        {
            vel = currentVelocity;
            vel.x = -vel.x;
            rb2d.linearVelocity = vel;
            return;
        }
        else if (coll.collider.CompareTag("TopBound"))
        {
            vel = currentVelocity;
            vel.y = -vel.y;
            rb2d.linearVelocity = vel;
            return;
        }
        else if(coll.collider.CompareTag("BotBound"))
        {
            LoseLife();
            resetBall();
        }
        else if (coll.collider.CompareTag("Block"))
        {
            // normal do bloco pra reflexao
            Vector2 normal = coll.contacts[0].normal;

            Vector2 reflectDir = Vector2.Reflect(currentVelocity, normal);

            rb2d.linearVelocity = reflectDir;

            // Remove o bloco
            Destroy(coll.gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentVelocity = rb2d.linearVelocity;
    }

    void LoseLife()
    {
        lives--;
        UpdateLivesUI();

        if (lives <= 0)
        {
            Debug.Log("Game Over!");
        }
    }

    void UpdateLivesUI()
    {
        if (livesText != null)
        {
            livesText.text = "Vidas: " + lives;
        }
    }
}
