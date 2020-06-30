using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] private GameObject loadScreen;

    public void LoadScene(string sceneName)
    {
        StartCoroutine(StartLoad(sceneName));
    }

    private IEnumerator StartLoad(string sceneName)
    {
        loadScreen.SetActive(true);
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
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
