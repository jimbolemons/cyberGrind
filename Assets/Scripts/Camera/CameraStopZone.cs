/*
 * CameraStopZone.cs
 * 
 * By: Jerod D'Epifanio
 * 
 * Waits for all players to hit the zone before continuing the camera
 * 
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraStopZone : MonoBehaviour
{
    private List<GameObject> enteredPlayers = new List<GameObject>();
    private bool done = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        // Camera wall has hit, time to wait
        if (collision.CompareTag(Constants.cameraWallTag) && !done)
        {
            BSGameManager.instance.mainCameraFollow.SetCameraLock(true);
            done = true;
        }


        if (collision.CompareTag(Constants.playerTag))
        {
            if (!enteredPlayers.Contains(collision.gameObject))
            {
                // Add new player entering zone
                enteredPlayers.Add(collision.gameObject);

                Debug.Log(enteredPlayers.Count + " : " + PlayerManager.instance.players.Count);

                // All players have made it, move on
                if (enteredPlayers.Count == PlayerManager.instance.players.Count)
                {
                    BSGameManager.instance.mainCameraFollow.SetCameraLock(false);
                    done = true;
                }
                    
            }
                
        }
    }
}
