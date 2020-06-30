using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StartEvent : MonoBehaviour
{
    public AK.Wwise.Event menuMusicEvent;
    public AK.Wwise.Event stopMusicEvent;

    private void Start()
    {
        stopMusicEvent.Post(gameObject);
        menuMusicEvent.Post(gameObject);
    }
}
