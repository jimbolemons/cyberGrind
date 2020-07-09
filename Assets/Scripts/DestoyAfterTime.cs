using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoyAfterTime : MonoBehaviour
{
    public float delayTimer; 
    // Start is called before the first frame update
    
    public void TimeDestroy()
    {
       
        Destroy(this.gameObject,delayTimer);

    }
    
}
