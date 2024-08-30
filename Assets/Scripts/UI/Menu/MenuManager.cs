using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Transform dontTouch;
    [SerializeField] private GameObject menuWindow;
    [SerializeField] private Player player;
    [SerializeField] private Image menuIconBtn;
    private bool menuOpen = false;

    public static MenuManager Instance = null;

    // Start is called before the first frame update

    private void Awake()
    {
        Instance = this;
    }

    public void OnMenuClick()
    {
        dontTouch.GetChild(0).gameObject.SetActive(true);
        menuWindow.SetActive(true);
        UIManager.Instance.cursorState = 1;
        menuOpen = true;
    }

    public void OnCancelClick()
    {
        dontTouch.GetChild(0).gameObject.SetActive(false);
        menuWindow.SetActive(false);
        UIManager.Instance.cursorState = 2;
        menuOpen = false;
    }

    public void OnExitClick()
    {
        SaveData();
        //Application.Quit(); // 실제 게임 종료
        SceneManager.LoadSceneAsync(0);
    }

    public void OnCharacterSelect()
    {
        SaveData();
        PlayerPrefs.SetString("nextSceneText", "캐릭터 선택");
        PlayerPrefs.SetString("nextSceneImage", "Dungeon");
        SceneChange.OnSceneChange("2-1. CharacterSelect");
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            Color menuIconColor = menuIconBtn.color;
            menuIconColor.a = 1.0f;
            menuIconBtn.color = menuIconColor;
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt) && !menuOpen)
        {
            Color menuIconColor = menuIconBtn.color;
            menuIconColor.a = 0.0f;
            menuIconBtn.color = menuIconColor;
        }
    }

    public void SaveData()
    {
        string characterPath = $"{Application.dataPath}/Data/SaveData/Characters.dat";
        Dictionary<int, CharacterData> characters = FileManager.LoadFromBinary<Dictionary<int, CharacterData>>(characterPath);
        characters[PlayerPrefs.GetInt("selNum")].MyStat = player.myStat;
        FileManager.SaveToBinary(characterPath, characters);
    }

}
