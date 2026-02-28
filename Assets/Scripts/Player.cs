using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 10.0f;
    public float boundX = 6.5f;
    private Rigidbody2D rb2d;
    [Header("Audio")]
    public AudioClip hitSound;
    public AudioClip bgMusic;
    public float bgmVolume;
    private AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
        audioSource.PlayOneShot(bgMusic, bgmVolume);
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        audioSource.PlayOneShot(hitSound, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        var vel = rb2d.linearVelocity; // Acessa a velocidade da raquete
        if (Input.GetKey(KeyCode.D))
        { // Velocidade da Raquete para ir para direita
            vel.x = speed;
        }
        else if (Input.GetKey(KeyCode.A))
        { // Velocidade da Raquete para ir para esquerda
            vel.x = -speed;
        }
        else
        {
            vel.x = 0; // Velociade para manter a raquete parada
        }
        rb2d.linearVelocity = vel; // Atualizada a velocidade da raquete

        var pos = transform.position; // Acessa a Posição da raquete
        if (pos.x > boundX)
        {
            pos.x = boundX; // Corrige a posicao da raquete caso ele ultrapasse o limite superior
        }
        else if (pos.x < -boundX)
        {
            pos.x = -boundX; // Corrige a posicao da raquete caso ele ultrapasse o limite inferior
        }
        transform.position = pos; // Atualiza a posição da raquete
    }
}
