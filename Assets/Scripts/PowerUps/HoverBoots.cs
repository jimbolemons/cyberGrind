using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A powerup to make the player hover for some time period
/// </summary>
public class HoverBoots : Powerup
{
    public HoverBoots()
    {
        powerupName = "HoverBoots";
    }

    public override void ActivatePowerup(GameObject player)
    {
        PowerupManager.instance.StartCoroutine("DoCoroutine", this.Hover(player));
    }

    private IEnumerator Hover(GameObject player)
    {
        Debug.Log("Hover powerup started for " + player.name);

        // Get rigidbody for gravity shit 
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();

        // Save current gravity scale
        float gScale = rb.gravityScale;

        // Start hover 
        rb.gravityScale = 0;

        // Run timer 
        yield return new WaitForSeconds(PowerupManager.instance.hoverDuration);

        // End hover mode 
        rb.gravityScale = gScale;

        Debug.Log("Hover powerup ended for " + player.name);
    }
}
