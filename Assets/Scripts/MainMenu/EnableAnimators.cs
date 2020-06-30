using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableAnimators : MonoBehaviour
{
    [SerializeField] private Animator[] animatorsToStart;

    private void Start()
    {
        // Do this cause aniamtor is bugged on start, thanks unity
        foreach (Animator anim in animatorsToStart)
        {
            anim.enabled = false;
            anim.enabled = true;
        }
    }

    public void StartAnimators()
    {
        foreach(Animator anim in animatorsToStart)
        {
            anim.enabled = true;
        }
    }
}
