using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityFlipZone : MonoBehaviour
{
    #region Unity Callbacks

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void Awake()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.playerTag))
        {
            // Make sure player is not already flipped
            // Set the bool in playerController
            BSPlayerController playerController = collision.gameObject.GetComponent<BSPlayerController>();
            playerController.IsInGravityZone = true;

            float x_to_compare = collision.gameObject.transform.rotation.x;
            if (x_to_compare == 0)
            {

                // Flip the player upside down 
                collision.gameObject.transform.rotation = Quaternion.Euler(180, 0, 0);

            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.playerTag))
        {
            // Set the bool in playerController
            BSPlayerController playerController = collision.gameObject.GetComponent<BSPlayerController>();
            playerController.IsInGravityZone = false;

            Debug.Log("Player left a gravity flip zone");

            // Flip the player back to normal
            collision.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.playerTag))
        {
            Rigidbody2D rb = collision.gameObject.GetComponent<Rigidbody2D>();
            rb.AddForce(Physics.gravity * rb.gravityScale * rb.mass * -1);
        }
    }

    #endregion
}
