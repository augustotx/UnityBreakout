using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    [Header("Colors")]
    public Sprite redColor;
    public Sprite greenColor;
    public Sprite blueColor;
    public Sprite purpleColor;
    public Sprite greyColor;

    [Header("Game Logic")]
    public GameObject ball;
    public bool gameIsFinished = false;

    private int numberOfChildren = -1;

    void Start()
    {
        Sprite[] colorSprites = new Sprite[] { redColor, greenColor, blueColor, purpleColor, greyColor };

        foreach (Transform child in transform)
        {
            SpriteRenderer sr = child.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.sprite = colorSprites[Random.Range(0, colorSprites.Length)];
            }
            else
            {
                Debug.LogWarning($"Sprite Renderer não existe!.");
            }
        }
    }

    void Update()
    {
        numberOfChildren = transform.childCount;
        Scene scene = SceneManager.GetActiveScene();

        if (numberOfChildren == 0)
            ChangeScene(scene.name);
    }

    void ChangeScene(string sceneName)
    {
        if (sceneName == "Scene1")
        {
            SceneManager.LoadScene("Scene2");
            return;
        }
        if (sceneName == "Scene2" && ball != null && !gameIsFinished)
        {
            gameIsFinished = true;
            ball.GetComponent<Ball>().VictorySpeech();
        }
    }
}