using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySound : MonoBehaviour
{
    [SerializeField] private AK.Wwise.Event soundToPlay;

    public void Play()
    {
        soundToPlay.Post(gameObject);
    }
}
