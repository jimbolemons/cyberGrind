using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableCrate : MonoBehaviour
{
    [SerializeField] private ParticleSystem destroyVFX;
    [SerializeField] private Vector2 slowForce;

    //Wwise Stuff
    [SerializeField] private AK.Wwise.Event boxBreakEvent;

    private BoxCollider2D collider;
    private SpriteRenderer renderer;

    private void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.playerTag))
        {
            BSPlayerController controller = collision.gameObject.GetComponent<BSPlayerController>();
            if (controller.isSlowed) { return; }
            StartCoroutine(controller.SlowTimer());

            collision.GetComponent<Rigidbody2D>().AddForce(slowForce);
            collider.enabled = false;
            renderer.enabled = false;

            //Wwise trigger
            boxBreakEvent.Post(gameObject);
            controller.animHandler.playerState = PlayerAnimatorHandler.PlayerState.Slowed;

            if (destroyVFX)
                destroyVFX.Play();
        }
    }
}
