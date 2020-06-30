using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slow : Powerup
{
    private float slowPowerLevel = -40f;

    public Slow()
    {
        powerupName = "Slow";
    }

    public override void ActivatePowerup(GameObject player)
    {
        PowerupManager.instance.StartCoroutine("DoCoroutine", this.SlowPlayer(player));
    }

    private IEnumerator SlowPlayer(GameObject player)
    {
        Debug.Log("Slow 'powerup' started for first player");

        //
        // For now, we replace the player with the FIRST PLACE PLAYER 
        GameObject targetPlayer = BSGameManager.instance.leadPlayer;
        //
        //

        float currentTime = 0f;

        // Get rigidbody of player  
        Rigidbody2D rb = targetPlayer.GetComponent<Rigidbody2D>();

        while(currentTime < PowerupManager.instance.slowDuration)
        {
            // Slow the player
            rb.AddForce(new Vector2(slowPowerLevel, 0));
            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        Debug.Log("Slow 'powerup' ended for first player");
    }
}
