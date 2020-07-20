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
    public GameObject light;
     
    private Camera mainCamera;
    
    public bool playRipple;

    public float stun;
    public ParticleSystem emit1;
   
    public int pointToSubtract = 3;
    
    

    private void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        renderer = GetComponent<SpriteRenderer>();
        mainCamera = Camera.main;
    }
    void DetachParticles()
    {
        // This splits the particle off so it doesn't get deleted with the parent
        emit1.transform.parent = null;
       
        // this stops the particle from creating more bits
        //emit2.Stop();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {     

        if (collision.gameObject.CompareTag("Player"))
        {
                    
            //collision.CompareTag(Constants.playerTag) || 
            BSPlayerController controller = collision.gameObject.GetComponent<BSPlayerController>();
            
            if (playRipple)
            {
             mainCamera.GetComponent<RippleController>().RipVanWinkle(this.transform.position);
             controller.SubtractUpgrade(pointToSubtract);
            }
            else
            {
             controller.AddUpgrade();

            }    
            //if (controller.isSlowed) { return; }
            //StartCoroutine(controller.SlowTimer());

            //collision.GetComponent<Rigidbody2D>().AddForce(slowForce);
            collision.rigidbody.AddForce(slowForce);
            collider.enabled = false;
            renderer.enabled = false;

            if (light != null)
            {
             Destroy(light);

            }
            controller.Stuntimer = stun;
            controller.HitBox = true;
            //Wwise trigger
            boxBreakEvent.Post(gameObject);
            //controller.animHandler.playerState = PlayerAnimatorHandler.PlayerState.Slowed;

            if (destroyVFX)
            {
                destroyVFX.Play();
                emit1.GetComponent<DestoyAfterTime>().TimeDestroy();
                DetachParticles();

                Destroy(this.gameObject,1f);

            }


        }
        
    }
}
