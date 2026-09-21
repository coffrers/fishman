using UnityEngine;

public class PacStudentMovement : MonoBehaviour
{
    public float speed = 2f;
    public Vector3[] corners;
    public Animator animator;
    public AudioSource audioSource;
    public AudioClip moveAudio;

    private int currentIndex = 0;
    private Vector3 startPos;
    private Vector3 endPos;
    private float startTime;
    private float duration;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = corners[0];
        currentIndex = 1;

        if (animator != null)
            animator.SetBool("ScriptControlled", true);
        StartAnim();

        if(audioSource != null && moveAudio != null)
        {
            audioSource.clip = moveAudio;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        float elapsed = Time.time - startTime;
        float t = elapsed / duration;
        if (t > 1f)
            t = 1f;
        transform.position = Vector3.Lerp(startPos, endPos, t);
        
        if (t >= 1f)
        {
            currentIndex = (currentIndex + 1) % corners.Length;
            StartAnim();
        }
    }

    private void StartAnim()
    {
        startPos = transform.position;
        endPos = corners[currentIndex];
        float distance = Vector3.Distance(startPos, endPos);
        duration = distance / speed;
        startTime = Time.time;

        PlayAnim(endPos - startPos);

    }

    private void PlayAnim(Vector3 direction)
    {
        if (animator == null)
            return;
        if(direction.x > 0)
        {
            animator.Play("right");
        }
        else if(direction.x < 0)
        {
            animator.Play("left");
        }
        else if(direction.y > 0)
        {
            animator.Play("up");
        }
        else if(direction.y < 0)
        {
            animator.Play("down");
        }
    }
}
