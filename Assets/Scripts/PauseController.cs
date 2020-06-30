using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseController : MonoBehaviour
{
    public GameObject PauseCanvas;


    public void update()
    {
        if (PauseCanvas.active == false)
        {
            if (Input.GetKey("escape"))
                    PauseCanvas.active = true;
        }
    }


    public void Resume()
    {
        PauseCanvas.active = false;

    }

    public void MainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Exit()
    {
        Application.Quit();
    }
}
