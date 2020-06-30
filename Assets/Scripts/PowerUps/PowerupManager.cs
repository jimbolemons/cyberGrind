using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupManager : Singleton<PowerupManager>
{
    private Powerup[] potentialPowerups;

    [Header("HoverBoots")]
    public float hoverDuration = 10f;
    public float slowDuration = 5f;

    private void Start()
    {
        potentialPowerups = new Powerup[2];
        potentialPowerups[0] = new HoverBoots();
        potentialPowerups[1] = new Slow();
    }

    #region Public Functions

    /// <summary>
    /// Picks a random powerup from the list of potentials
    /// </summary>
    /// <returns> A powerup to be used by powerup boxes </returns>
    public Powerup GetPowerup()
    {
        if (potentialPowerups.Length == 0) { return null; }

        // Randomly pick a powerup from the options we have 
        int index = Random.Range(0, potentialPowerups.Length);

        //Debug.Log("Index for powerup is " + index);

        return potentialPowerups[index];
    }


    public IEnumerator DoCoroutine(IEnumerator cor)
    {
        while (cor.MoveNext())
            yield return cor.Current;
    }

    #endregion
}
