using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class YippeTalk : MonoBehaviour
{
    [Header("Music")]
    public AudioClip bgMusic;
    public float bgmVolume = 0.3f;

    [Header("Diag")]
    public string[] diags;
    public string nextScene;
    public float typingSpeed = 0.035f;
    public Text textComponent;

    [Header("Click Text")]
    public Text instructText;
    public float delayInstruct = 2f;
    public float fadeDuration = 1.0f;
    private CanvasGroup instructCanvasGroup;

    [Header("Credit Text")]
    public Text creditText;
    public float delayCredit = 3f;
    public float creditFadeDuration = 1f;
    private CanvasGroup creditCanvasGroup;

    [Header("Balatro Voice")]
    public AudioClip[] audioClips;
    private AudioSource audioSource;

    [Header("Movement")]
    public float floatAmplitude = 0.1f;
    public float floatSpeed = 1.5f;
    public float tiltAngle = 5f;
    public float tiltSpeed = 1.2f;

    private int curIndex = 0;
    private bool isTalking = false;
    private Vector3 initialPos;
    private Quaternion initialRot;
    private Coroutine talkRoutine;
    private Coroutine instructRoutine;
    private Coroutine creditRoutine;

    void Start()
    {
        initialPos = transform.localPosition;
        initialRot = transform.localRotation;
        audioSource = GetComponent<AudioSource>();


        if (instructText != null)
        {
            instructCanvasGroup = instructText.GetComponent<CanvasGroup>();
            if (instructCanvasGroup == null)
                instructCanvasGroup = instructText.gameObject.AddComponent<CanvasGroup>();

            instructText.gameObject.SetActive(false);
            instructCanvasGroup.alpha = 0;
        }

        if (creditText != null)
        {
            creditCanvasGroup = creditText.GetComponent<CanvasGroup>();
            if (creditCanvasGroup == null)
                creditCanvasGroup = creditText.gameObject.AddComponent<CanvasGroup>();

            creditText.gameObject.SetActive(false);
            creditCanvasGroup.alpha = 0;
        }

        if (bgMusic != null) audioSource.PlayOneShot(bgMusic, bgmVolume);
        if (creditText != null) creditRoutine = StartCoroutine(ShowCredit());
        StartDiag();
    }

    void Update()
    {
        // 1. Movimentação Visual (Senoide)
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.localPosition = initialPos + new Vector3(0, newY, 0);
        float tilt = Mathf.Sin(Time.time * tiltSpeed) * tiltAngle;
        transform.localRotation = initialRot * Quaternion.Euler(0, 0, tilt);

        // 2. Input do Jogador
        if (Input.GetMouseButtonDown(0))
        {
            if (isTalking)
            {
                CompleteTextInstantly();
            }
            else
            {
                NextDiag();
            }
        }
    }

    void StartDiag()
    {
        if (curIndex < diags.Length)
        {
            // Reseta a instrução para o próximo diálogo
            if (instructText != null)
            {
                instructText.gameObject.SetActive(false);
                instructCanvasGroup.alpha = 0;
                if (instructRoutine != null) StopCoroutine(instructRoutine);
            }
            talkRoutine = StartCoroutine(TypeText(diags[curIndex]));

        }
    }

    void NextDiag()
    {
        curIndex++;
        if (curIndex < diags.Length)
        {
            StartDiag();
        }
        else
        {
            SceneManager.LoadScene(nextScene);
        }
    }

    void CompleteTextInstantly()
    {
        StopCoroutine(talkRoutine);
        textComponent.text = diags[curIndex];
        FinishTalking();
    }

    IEnumerator TypeText(string txt)
    {
        isTalking = true;
        textComponent.text = "";

        for (int i = 0; i < txt.Length; i++)
        {
            textComponent.text += txt[i];

            if (i % 2 == 0 && audioClips.Length > 0)
            {
                audioSource.PlayOneShot(audioClips[Random.Range(0, audioClips.Length)], 0.5f);
            }
            yield return new WaitForSeconds(typingSpeed);
        }
        FinishTalking();
    }

    void FinishTalking()
    {
        isTalking = false;
        // Inicia o timer para o Fade In
        if (instructRoutine != null) StopCoroutine(instructRoutine);
        if (instructText != null) instructRoutine = StartCoroutine(ShowInstruct());
    }

    IEnumerator ShowInstruct()
    {
        // Espera o tempo determinado (ex: 2 segundos)
        yield return new WaitForSeconds(delayInstruct);

        // Atualiza o texto conforme o progresso
        if (curIndex == diags.Length - 1)
            instructText.text = "Clique para começar...";
        else
            instructText.text = "Clique para continuar...";

        instructText.gameObject.SetActive(true);

        // Loop de Fade In usando CanvasGroup.alpha
        float timeSpent = 0;
        while (timeSpent < fadeDuration)
        {
            timeSpent += Time.deltaTime;
            instructCanvasGroup.alpha = Mathf.Lerp(0, 1, timeSpent / fadeDuration);
            yield return null;
        }

        instructCanvasGroup.alpha = 1;
    }

    IEnumerator ShowCredit()
    {
        yield return new WaitForSeconds(delayCredit);
        creditText.gameObject.SetActive(true);

        float timeSpent = 0;
        while (timeSpent < creditFadeDuration)
        {
            timeSpent += Time.deltaTime;
            creditCanvasGroup.alpha = Mathf.Lerp(0, 1, timeSpent / creditFadeDuration);
            yield return null;
        }

        creditCanvasGroup.alpha = 1;
    }

    public void DefeatTalk(Text tc)
    {
        diags = new[] { "Que pena... você morreu.", "Vamos tentar de novo!" };
        textComponent = tc;
        bgMusic = null;
    }
    public void VictoryTalk(Text tc)
    {
        diags = new[] { "Yippee!", "Você conseguiu!", "Parabéns e obrigado por jogar!", "Sempre dá pra tentar de novo :D" };
        nextScene = "";
        textComponent = tc;
        bgMusic = null;
    }
}
