using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneChange
{
    public static void OnSceneChange(string nextScene = "Intro")
    {
        PlayerPrefs.SetString("nextScene", nextScene);
        SceneManager.LoadScene("Loading", LoadSceneMode.Additive);
    }
}