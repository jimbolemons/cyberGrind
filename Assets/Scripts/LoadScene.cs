using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartLoad());
    }

    private IEnumerator StartLoad()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainMenu");
        asyncLoad.allowSceneActivation = false;
        float timeWaited = 0;
        while (!asyncLoad.isDone && timeWaited < 4f)
        {
            yield return new WaitForEndOfFrame();
            timeWaited += Time.deltaTime;
        }

        asyncLoad.allowSceneActivation = true;
    }
}
