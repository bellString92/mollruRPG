using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : AnimatorProperty
{
    [SerializeField] private Slider progressBar;
    [SerializeField] private TMPro.TMP_Text _nextSceneText;
    [SerializeField] private Image _nextSceneImage;
    [SerializeField] private GameObject runProgress;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(LoadingCor());

        string nextSceneText = PlayerPrefs.GetString("nextSceneText");
        string nextSceneImage = PlayerPrefs.GetString("nextSceneImage");

        if ("".Equals(nextSceneText)) nextSceneText = "로딩 중";
        if ("".Equals(nextSceneImage)) nextSceneImage = "Dungeon";

        _nextSceneText.text = nextSceneText;
        _nextSceneText.outlineWidth = 0.2f;
        _nextSceneImage.sprite = Resources.Load<Sprite>($"Images/Loading/{nextSceneImage}");

        myAnim.SetBool("idle", false);
        myAnim.SetBool("run", true);
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
            SceneManager.LoadSceneAsync(0);
            yield break;
        }
        AsyncOperation ao = SceneManager.LoadSceneAsync(nextScene);
        ao.allowSceneActivation = false;
        float curTime = 0.0f;
        float runPosX = 0.0f;
        while (!ao.isDone)
        {
            curTime += Time.deltaTime;
            progressBar.value = curTime / 5.0f;
            runPosX = 20.0f * progressBar.value;
            Vector3 handlePos = new Vector3(runPosX-10.0f, -4.5f, 0);
            runProgress.transform.position = handlePos;
            if (curTime > 5.0f)
            {
                ao.allowSceneActivation = true;
            }
            yield return null;
        }
        
    }
}
