using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void QuitGame()
    {
        if (!Application.isEditor) 
            System.Diagnostics.Process.GetCurrentProcess().Kill();
    }
}
