using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
