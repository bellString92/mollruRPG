using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    public string nextScene = "Intro";
    public string nextSceneText = "·Îµù Áß";
    public string nextSceneImage = "Dungeon";
    public LayerMask targetMask;
    private Scene curScene;
    // Start is called before the first frame update
    void Start()
    {
        curScene = SceneManager.GetActiveScene();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if ((1 << other.gameObject.layer & targetMask) != 0)
        {
            PlayerPrefs.SetString("nextSceneText", nextSceneText);
            PlayerPrefs.SetString("nextSceneImage", nextSceneImage);
            SceneChange.OnSceneChange(nextScene);
        }
    }
}
