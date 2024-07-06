using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public static SceneChange Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public void OnSceneChange(int nextSceneIdx = 0)
    {
        SceneManager.LoadScene(nextSceneIdx);
    }
}
