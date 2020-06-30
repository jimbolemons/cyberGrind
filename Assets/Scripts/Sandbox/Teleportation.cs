using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleportation : MonoBehaviour
{
    public GameObject otherTeleporter;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    { 
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if a player has collided with this teleporter 
        if(other.gameObject.tag == "Player")
        {
            Debug.Log("Player hit a teleporter!");

            GameObject Player = other.gameObject;

            // Check if player is eligible to teleport again 
            if (Player.GetComponent<PlayerStats>().recentlyTeleported == false)
            {
                // Only teleport if the player is off cooldown
                StartCoroutine(Teleport(Player));
            }
        }
    }

    IEnumerator Teleport(GameObject Player)
    {
        Debug.Log("Starting teleport IEnumerator!");
        yield return new WaitForSeconds(0.001f);
        // Change player stats so cooldown is handled 
        Player.GetComponent<PlayerStats>().Teleported();
        Debug.Log("Done waiting for teleport IEnumerator!");
        // Player.transform.position = new Vector3(0, 1, 0); // Jumps back to origin for simple testing
        Player.transform.position = otherTeleporter.transform.position; // Teleports to the given connected otherTeleporter
        Debug.Log("Player has teleported!");
    }
}
