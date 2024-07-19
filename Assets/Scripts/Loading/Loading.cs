using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public Slider progressBar;
    public TMPro.TMP_Text _nextSceneText;
    public Image _nextSceneImage;

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
            SceneManager.LoadSceneAsync("Intro");
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
