using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Gives a random powerup when picked up 
/// </summary>
public class PowerupBox : MonoBehaviour
{
    private Powerup powerup = null;

    #region Unity Callbacks 

    // Start function runs on an enumerator so all boxes get powerups at once 
    // Gets a random powerup from the game manager 
    IEnumerator Start()
    {
        powerup = PowerupManager.instance.GetPowerup();
        yield return null;
    }


    #endregion


    #region Private Functions

    /// <summary>
    /// Checks that the powerup has been hit by a player 
    /// </summary>
    /// <param name="collision"> The collider of the thing that hit this powerup </param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(Constants.playerTag))
        {
            // Get the player object
            BSPlayerController player = collision.GetComponent<BSPlayerController>();

            // Check that the player doesn't already have a powerup
            if ((player.currentPowerup != null) && (player.currentPowerup.powerupName != ""))
            {
                Debug.Log("-PowerupBox hit by: " + player.playerName + " but they already have a powerup: " + player.currentPowerup.powerupName);
                return;
            }

            Debug.Log("-PowerupBox picked up by: " + player.playerName);

            // Make the player pickup the powerup 
            PickupPowerupBox(player);
        }
    }

    private void PickupPowerupBox(BSPlayerController player)
    {
        // Give player powerup
        player.currentPowerup = powerup;

        Debug.Log("-Player " + player.playerName + " picked up powerup: " + powerup);

        // Disable this powerup so no one else can pick it up
        this.gameObject.SetActive(false);
    }

    #endregion 
}
