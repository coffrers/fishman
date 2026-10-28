using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;

public class UIManager : MonoBehaviour
{
    public AudioSource audioSource;

    [SerializeField] private GameObject countdownImage;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private GhostAudio ghostAudio;
    [SerializeField] private PacStudentController pacStudent;
    [SerializeField] private GhostState ghostState;

    [SerializeField] private TextMeshProUGUI gameTimerText;

    private bool roundStarted;
    private float gameTime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if(countdownText != null)
        {
            gameTimerText.text = "00:00:00";
            StartCoroutine(Countdown());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!roundStarted) return;
        gameTime = gameTime + Time.deltaTime;

        int minutes = (int)(gameTime / 60f);
        int seconds = (int)(gameTime % 60f);
        int hundredths = (int)((gameTime * 100f) % 100f);
        gameTimerText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + hundredths.ToString("00");
    }

    private IEnumerator Countdown()
    {
        pacStudent.SetControl(false);
        ghostState.FreezeGhosts();
        countdownImage.SetActive(true);
        countdownText.gameObject.SetActive(true);
        ghostAudio.PlayIntro();

        countdownText.text = "3";
        yield return new WaitForSeconds(1f);

        countdownText.text = "2";
        yield return new WaitForSeconds(1f);

        countdownText.text = "1";
        yield return new WaitForSeconds(1f);

        countdownText.text = "GO!";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);
        countdownImage.SetActive(false);
        ghostAudio.PlayNormal();
        pacStudent.SetControl(true);
        ghostState.UnfreezeGhosts();
        roundStarted = true;
    }

    public void LoadLevel1()
    {
        audioSource.Play();
        Invoke("NextScene", 0.3f);
    }

    void NextScene()
    {
        SceneManager.LoadScene("MAINscene");
    }

    public void ExittoStartScene()
    {
        SceneManager.LoadScene("StartScene");
    }
}
