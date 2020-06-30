using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base powerup class that all powerups inherit from 
/// </summary>
[System.Serializable]
public class Powerup
{
    public string powerupName { get; protected set; }

    public Powerup()
    {
        powerupName = "";
    }

    virtual public void ActivatePowerup(GameObject player) { }
}
