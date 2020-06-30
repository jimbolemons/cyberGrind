using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    // Variables for tracking various things about player state
    public bool recentlyTeleported = false;
    private int teleporterCooldown = 3;
        
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Telporter cooldown 
    IEnumerator TeleportCooldown()
    {
        recentlyTeleported = true;
        yield return new WaitForSeconds(teleporterCooldown); // Cooldown value for teleporting 
        recentlyTeleported = false;
    }

    public void Teleported()
    {
        StartCoroutine(TeleportCooldown());
    }
}
