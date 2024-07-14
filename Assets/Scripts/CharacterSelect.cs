using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class CharacterSelect : MonoBehaviour
{
    public int startNum = 0;
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private enum PopupType
    {
        CREATE,
        DELETE,
        START
    }

    public void OnCreateSelectSlotClick(int num)
    {
        if (GameObject.Find($"Paladin{num}") == null)
            CreatePopup(PopupType.CREATE, num);
        else
            OnCreateSelectSlotClickOk(num);
    }

    private void OnCreateSelectSlotClickOk(int num)
    {
        GameObject selSlot = GameObject.Find($"Slot{num}");
        for (int i = 1; i <= 3; i++)
        {
            Transform slot = GameObject.Find($"Slot{i}").transform;
            Transform t = slot.GetComponent<Transform>().Find("Magic circle").transform;
            for (int j = 0; j < t.childCount; j++)
            {
                if (i == num)
                    t.GetChild(j).gameObject.SetActive(true);
                else
                    t.GetChild(j).gameObject.SetActive(false);
            }
        }

        if (GameObject.Find($"Paladin{num}") == null)
        {
            GameObject prefab = Resources.Load("Prefabs/Paladin") as GameObject;
            GameObject paladin = Instantiate(prefab);
            paladin.name = $"Paladin{num}";
            paladin.transform.parent = selSlot.transform;
            paladin.transform.localPosition = Vector3.zero;
            paladin.transform.localRotation = Quaternion.identity;
        }

        GameObject.Find("CreateSelectSlot").transform.GetComponent<Transform>().Find($"Slot{num}Btn").GetComponentInChildren<TMPro.TMP_Text>().text = "캐릭터 선택";
        GameObject.Find("DeleteSlot").transform.GetComponent<Transform>().Find($"Slot{num}Btn").transform.gameObject.SetActive(true);

        startNum = num;
    }

    public void OnDeleteSlotClick(int num)
    {
        CreatePopup(PopupType.DELETE, num);
    }

    private void OnDeleteSlotClickOk(int num)
    {
        for (int i = 1; i <= 3; i++)
        {
            Transform slot = GameObject.Find($"Slot{i}").transform;
            Transform t = slot.GetComponent<Transform>().Find("Magic circle").transform;
            for (int j = 0; j < t.childCount; j++)
            {
                if (i == num)
                    t.GetChild(j).gameObject.SetActive(false);
            }
        }
        Destroy(GameObject.Find($"Paladin{num}"));
        GameObject.Find("CreateSelectSlot").transform.GetComponent<Transform>().Find($"Slot{num}Btn").GetComponentInChildren<TMPro.TMP_Text>().text = "신규캐릭터 생성";
        GameObject.Find("DeleteSlot").transform.GetComponent<Transform>().Find($"Slot{num}Btn").transform.gameObject.SetActive(false);
        if (num == startNum)
            startNum = 0;
    }

    public void GameStart()
    {
        CreatePopup(PopupType.START, startNum);
    }

    private void GameStartOk(int num)
    {
        PlayerPrefs.SetString("nextSceneText", "마을");
        PlayerPrefs.SetString("nextSceneImage", "MollRu_Town");
        SceneChange.OnSceneChange("MollRuRPGScene");
    }

    private void CreatePopup(PopupType type, int num = 0)
    {
        GameObject prefabs = Resources.Load("Prefabs/Popup") as GameObject;
        GameObject popup = Instantiate(prefabs) as GameObject;
        TMPro.TMP_Text[] texts = popup.transform.GetComponentsInChildren<TMPro.TMP_Text>();

        Button[] btns = popup.transform.GetComponentsInChildren<Button>();
        btns[0].onClick.AddListener(() => { Destroy(popup); });
        switch (type)
        {
            case PopupType.CREATE:
                texts[0].text = "생성 하시겠습니까?";
                btns[1].onClick.AddListener(() => { OnCreateSelectSlotClickOk(num); Destroy(popup); });
                break;
            case PopupType.DELETE:
                texts[0].text = "삭제 하시겠습니까?";
                btns[1].onClick.AddListener(() => { OnDeleteSlotClickOk(num); Destroy(popup); });
                break;
            case PopupType.START:
                if (num == 0)
                {
                    texts[0].text = "캐릭터를 선택해주세요.";
                    btns[1].onClick.AddListener(() => { Destroy(popup); });
                } else
                {
                    texts[0].text = "게임 시작하시겠습니까?";
                    btns[1].onClick.AddListener(() => { GameStartOk(num); Destroy(popup); });
                }
                break;
        }
        popup.transform.parent = GameObject.Find("PopupParent").transform;
        popup.transform.localPosition = Vector3.zero;
        popup.SetActive(true);
    }
}
