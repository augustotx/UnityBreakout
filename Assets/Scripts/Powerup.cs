using UnityEngine;

public class Powerup : MonoBehaviour
{
    public int id;
    public GameObject ball;
    private Rigidbody2D rb2d;

    public void Initialize(int initId)
    {
        id = initId;
        // por agora
        id = 1;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.CompareTag("Player"))
        {
            activatePowerup();
            Destroy(this.gameObject);
        }
    }

    void activatePowerup()
    {
        switch (id)
        {
            case 1:
                GameObject b = Instantiate(ball, transform.position, transform.rotation);
                Ball bs = b.GetComponent<Ball>();
                bs.SpawnNewBall();
                return;
            default:
                return;
        }
    }

    // Update is called once per frame
    void Update()
    {
        rb2d.linearVelocity = Vector2.down * 2f;
    }
}
