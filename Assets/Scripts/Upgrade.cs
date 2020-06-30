using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    [SerializeField] private ParticleSystem collectVFX;
    [SerializeField] private AK.Wwise.Event collectEvent;

    private SpriteRenderer renderer;
    private Collider2D collider;

    private void Start()
    {
        renderer = GetComponent<SpriteRenderer>();
        collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.playerTag))
        {
            collision.GetComponent<BSPlayerController>().AddUpgrade();
            collectVFX.Play();
            if (renderer)
                renderer.enabled = false;

            if(collider)
                collider.enabled = false;

            collectEvent.Post(gameObject);
        }
    }
}
