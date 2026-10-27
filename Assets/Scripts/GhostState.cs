using UnityEngine;
using System.Collections;
using TMPro;

public class GhostState : MonoBehaviour
{
    [SerializeField] private Animator[] ghosts;
    [SerializeField] private GhostAudio ghostAudio;
    [SerializeField] private GameObject ghostTimerObject;
    [SerializeField] private TextMeshProUGUI ghostTimerText;

    private const int Normal = 0;
    private const int Scared = 1;
    private const int Recovering = 2;
    private const int Dead = 3;

    private Coroutine scaredState;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ghostTimerObject.SetActive(false);
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
    // Update is called once per frame
    void Update()
    {
        
    }
}
