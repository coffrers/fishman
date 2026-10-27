using UnityEngine;
using System.Collections;
using TMPro;

public class GhostState : MonoBehaviour
{
    [SerializeField] private Animator[] ghosts;
    [SerializeField] private GhostAudio ghostAudio;
    [SerializeField] private GameObject ghostTimerObject;
    [SerializeField] private TextMeshProUGUI ghostTimerText;

    public const int Normal = 0;
    public const int Scared = 1;
    public const int Recovering = 2;
    public const int Dead = 3;

    private Coroutine scaredState;
    private Vector3[] startPositions;
    public bool GhostsFrozen { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ghostTimerObject.SetActive(false);
        startPositions = new Vector3[ghosts.Length];
        for(int i = 0; i < ghosts.Length; i++)
        {
            startPositions[i] = ghosts[i].transform.position;
        }
    }

    public void StartScared()
    {
        if(scaredState != null)
        {
            StopCoroutine(scaredState);
        }
        scaredState = StartCoroutine(ScaredState());
    }

    private IEnumerator ScaredState()
    {
        SetAllGhosts(Scared);
        ghostAudio.PlayScared();
        ghostTimerObject.SetActive(true);
        int secondsLeft = 10;
        while(secondsLeft > 0)
        {
            ghostTimerText.text = secondsLeft.ToString();

            if (secondsLeft == 3)
            {
                SetAllGhosts(Recovering);
            }

            yield return new WaitForSeconds(1f);
            secondsLeft = secondsLeft - 1;
        }

        SetAllGhosts(Normal);
        ghostAudio.PlayNormal();
        ghostTimerObject.SetActive(false);
        scaredState = null;
    }

    private void SetAllGhosts(int state)
    {
        foreach(Animator ghost in ghosts)
        {
            if (ghost.GetInteger("GhostState") == Dead) continue;
            ghost.SetInteger("GhostState", state);
        }
    }

    public void FreezeGhosts()
    {
        GhostsFrozen = true;
    }

    public void ResetGhosts()
    {
        if(scaredState != null)
        {
            StopCoroutine(scaredState);
            scaredState = null;
        }

        for(int i = 0; i < ghosts.Length; i++)
        {
            ghosts[i].transform.position = startPositions[i];
            ghosts[i].SetInteger("GhostState", Normal);
        }

        ghostTimerObject.SetActive(false);
        ghostAudio.PlayNormal();
        GhostsFrozen = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
