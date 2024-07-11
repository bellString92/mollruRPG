using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Slider progressBar;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadingCor());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator LoadingCor()
    {
        string nextScene = PlayerPrefs.GetString("nextScene");
        if (nextScene == "")
        {
            SceneManager.LoadScene("Intro");
            yield break;
        }
        AsyncOperation ao = SceneManager.LoadSceneAsync(nextScene);
        ao.allowSceneActivation = false;
        float curTime = 0.0f;
        while (!ao.isDone)
        {
            curTime += Time.deltaTime;
            progressBar.value = curTime / 5.0f;
            
            if (curTime > 5.0f)
            {
                ao.allowSceneActivation = true;
            }
            yield return null;
        }
        
        

    }
}
