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
    public Text yippeeText;

    [Header("Audio")]
    public AudioClip bounceSound;
    private AudioSource audioSource;

    [Header("Game Logic")]
    public int lives = 3;

    [Header("Prefabs")]
    public GameObject powerup;

    // pra bolas instanciadas
    private SpriteRenderer spriteRenderer;
    private bool isSpawned = false;

    [Header("Talking Head")]
    public GameObject yippee;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb2d = GetComponent<Rigidbody2D>();
        currentVelocity = Vector2.zero;
        playerObj = GameObject.FindWithTag("Player");
        startPos = transform.position - playerObj.transform.position;
        audioSource = GetComponent<AudioSource>();
        UpdateLivesUI();
    }

    void ResetBall()
    {
        rb2d.linearVelocity = Vector2.zero;
        transform.position = (Vector2)playerObj.transform.position + startPos;

    }

    public void SpawnNewBall()
    {
        isSpawned = true;
        if (playerObj == null) playerObj = GameObject.FindWithTag("Player");
        if (rb2d == null) rb2d = GetComponent<Rigidbody2D>();
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = Color.blue;
        startPos = new Vector2(0.24f, 0.21f);
        ResetBall();
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
        else if (coll.collider.CompareTag("LeftBound") || coll.collider.CompareTag("RightBound"))
        {
            vel = currentVelocity;
            vel.x = -vel.x;
            rb2d.linearVelocity = vel;
            audioSource.PlayOneShot(bounceSound, 1f);
            return;
        }
        else if (coll.collider.CompareTag("TopBound"))
        {
            vel = currentVelocity;
            vel.y = -vel.y;
            rb2d.linearVelocity = vel;
            audioSource.PlayOneShot(bounceSound, 1f);
            return;
        }
        else if (coll.collider.CompareTag("BotBound"))
        {
            LoseLife();
            ResetBall();
        }
        else if (coll.collider.CompareTag("Block"))
        {
            // normal do bloco pra reflexao
            Vector2 normal = coll.contacts[0].normal;

            Vector2 reflectDir = Vector2.Reflect(currentVelocity, normal);

            rb2d.linearVelocity = reflectDir;

            // logica de powerup
            spawnPowerUp();

            // Remove o bloco
            audioSource.PlayOneShot(bounceSound, 1f);
            Destroy(coll.gameObject);
        }
        else if (coll.collider.CompareTag("Ball"))
        {
            Vector2 normal = coll.contacts[0].normal;

            Vector2 reflectDir = Vector2.Reflect(currentVelocity, normal);

            rb2d.linearVelocity = reflectDir;

            audioSource.PlayOneShot(bounceSound, 1f);
        }
    }

    void spawnPowerUp()
    {
        int randomInt = Random.Range(0, 20);
        if (randomInt == 1)
        {
            int randomID = Random.Range(0, 5);
            GameObject localpowerup = Instantiate(powerup, transform.position, transform.rotation);
            Powerup localpowerupscript = localpowerup.GetComponent<Powerup>();
            localpowerupscript.Initialize(randomID);
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentVelocity = rb2d.linearVelocity;
    }

    void LoseLife()
    {
        if (isSpawned) Destroy(this.gameObject);
        lives--;
        UpdateLivesUI();

        if (lives <= 0)
        {
            GameObject y = Instantiate(yippee, Vector2.zero, Quaternion.identity);
            YippeTalk ys = y.GetComponent<YippeTalk>();
            ys.DefeatTalk(yippeeText);
            Destroy(this.gameObject);
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

    public void VictorySpeech()
    {
        GameObject y = Instantiate(yippee, Vector2.zero, Quaternion.identity);
        YippeTalk ys = y.GetComponent<YippeTalk>();
        ys.VictoryTalk(yippeeText);
        Destroy(this.gameObject);
    }
}
