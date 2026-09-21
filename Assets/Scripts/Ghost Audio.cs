using UnityEngine;

public class GhostAudio : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip introClip;
    public AudioClip ghostNormalAudio;

    private bool switched = false;
    private float timer = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource.clip = introClip;
        audioSource.loop = false;
        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (switched)
            return;

        timer += Time.deltaTime;
        if (timer >= 3f)
        {
            switchGhostLoop();
        }
        else if (!audioSource.isPlaying)
        {
            switchGhostLoop();
        }
    }

    private void switchGhostLoop()
    {
        switched = true;
        audioSource.clip = ghostNormalAudio;
        audioSource.loop = true;
        audioSource.Play();
    }
}
