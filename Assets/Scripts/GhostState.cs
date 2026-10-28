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
    private Coroutine[] deadStates;
    private Vector3[] startPositions;
    private int scaredSecondsLeft;
    private int deadGhostCount;
    public bool GhostsFrozen { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ghostTimerObject.SetActive(false);
        startPositions = new Vector3[ghosts.Length];
        deadStates = new Coroutine[ghosts.Length];
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
        scaredSecondsLeft = 10;
        scaredState = StartCoroutine(ScaredState());
    }

    private IEnumerator ScaredState()
    {
        SetAllGhosts(Scared);
        UpdateGhostMusic();
        ghostTimerObject.SetActive(true);
        while(scaredSecondsLeft > 0)
        {
            ghostTimerText.text = scaredSecondsLeft.ToString();

            if (scaredSecondsLeft == 3)
            {
                SetAllGhosts(Recovering);
            }

            yield return new WaitForSeconds(1f);
            scaredSecondsLeft = scaredSecondsLeft - 1;
        }

        SetAllGhosts(Normal);
        ghostTimerObject.SetActive(false);
        scaredState = null;
        UpdateGhostMusic();
    }

    public bool EatGhost(Animator ghost)
    {
        int index = System.Array.IndexOf(ghosts, ghost);
        if (index < 0) return false;
        int state = ghost.GetInteger("GhostState");
        if (state == Normal) return false;
        if (state == Dead) return false;

        ghost.SetInteger("GhostState", Dead);
        deadGhostCount = deadGhostCount + 1;
        UpdateGhostMusic();
        deadStates[index] = StartCoroutine(DeadState(index));
        return true;
    }

    private IEnumerator DeadState(int index)
    {
        yield return new WaitForSeconds(3f);
        if(scaredSecondsLeft <= 0)
        {
            ghosts[index].SetInteger("GhostState", Normal);
        }
        else if (scaredSecondsLeft <= 3)
        {
            ghosts[index].SetInteger("GhostState", Recovering);
        }
        else
        {
            ghosts[index].SetInteger("GhostState", Scared);
        }
        deadGhostCount = deadGhostCount - 1;
        deadStates[index] = null;
        UpdateGhostMusic();
    }

    private void UpdateGhostMusic()
    {
        if(deadGhostCount > 0)
        {
            ghostAudio.PlayDead();
        }
        else if(scaredSecondsLeft > 0)
        {
            ghostAudio.PlayScared();
        }
        else
        {
            ghostAudio.PlayNormal();
        }
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

        for (int i = 0; i < deadStates.Length; i++)
        {
            if(deadStates[i] != null)
            {
                StopCoroutine(deadStates[i]);
                deadStates[i] = null;
            }
        }

        deadGhostCount = 0;
        scaredSecondsLeft = 0;

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
