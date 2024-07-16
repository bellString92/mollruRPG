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
    private string _nickName = "";
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
        if (GameObject.Find($"Paladin{num}") == null && GameObject.Find($"Maria{num}") == null)
            CreateNickName(PopupType.CREATE, num);
        else
        {
            string nickName = GameObject.Find($"NickName{num}").transform.GetComponentInChildren<TMPro.TMP_Text>().text;
            OnCreateSelectSlotClickOk(num, nickName);
        }
    }

    private void OnCreateSelectSlotClickOk(int num, string nickName = "")
    {
        _nickName = nickName;
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
            GameObject paladin = Instantiate(Resources.Load("Prefabs/Paladin") as GameObject);
            paladin.name = $"Paladin{num}";
            paladin.transform.SetParent(selSlot.transform);
            paladin.transform.localPosition = Vector3.zero;
            paladin.transform.localRotation = Quaternion.identity;
        }

        if (GameObject.Find($"Maria{num}") == null)
        {
            GameObject maria = Instantiate(Resources.Load("Prefabs/Maria") as GameObject);
            maria.name = $"Maria{num}";
            maria.transform.SetParent(selSlot.transform);
            maria.transform.localPosition = Vector3.zero;
            maria.transform.localRotation = Quaternion.identity;
        }

        if (!"".Equals(nickName))
        {
            GameObject.Find($"NickName{num}").transform.GetChild(0).gameObject.SetActive(true);
            GameObject.Find($"NickName{num}").transform.GetComponentInChildren<TMPro.TMP_Text>().text = nickName;
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
        Destroy(GameObject.Find($"Maria{num}"));

        GameObject.Find($"NickName{num}").transform.GetChild(0).gameObject.SetActive(false);

        GameObject.Find("CreateSelectSlot").transform.GetComponent<Transform>().Find($"Slot{num}Btn").GetComponentInChildren<TMPro.TMP_Text>().text = "신규캐릭터 생성";
        GameObject.Find("DeleteSlot").transform.GetComponent<Transform>().Find($"Slot{num}Btn").transform.gameObject.SetActive(false);
        if (num == startNum) startNum = 0;
    }

    public void GameStart()
    {
        CreatePopup(PopupType.START, startNum);
    }

    private void GameStartOk(int num)
    {
        PlayerPrefs.SetString("nextSceneText", "마을");
        PlayerPrefs.SetString("nextSceneImage", "MollRu_Town");
        PlayerPrefs.SetString("nickName", _nickName);
        SceneChange.OnSceneChange("MollRuRPGScene");
    }

    private void CreateNickName(PopupType type, int num)
    {
        GameObject.Find("DontTouch").transform.GetChild(0).gameObject.SetActive(true);
        GameObject textPopup = Instantiate(Resources.Load("Prefabs/TextPopup") as GameObject);

        textPopup.transform.GetChild(1).GetComponentInChildren<TMPro.TMP_Text>().text = "닉네임 설정";
        textPopup.transform.GetChild(3).GetComponentInChildren<TMPro.TMP_Text>().text = "확인";
        textPopup.transform.GetComponentInChildren<Button>().onClick.AddListener(() => {
            string nickName = textPopup.transform.GetChild(2).GetComponentInChildren<TMPro.TMP_InputField>().text;
            string result = NickNameCheck(nickName);
            if ("".Equals(result))
            {
                textPopup.transform.GetChild(2).GetChild(1).transform.gameObject.SetActive(false);
                Destroy(textPopup);
                GameObject.Find("DontTouch").transform.GetChild(0).gameObject.SetActive(false);
                CreatePopup(type, num, nickName);
            }
            else
            {
                textPopup.transform.GetChild(2).GetChild(1).GetComponent<TMPro.TMP_Text>().text = result;
                textPopup.transform.GetChild(2).GetChild(1).transform.gameObject.SetActive(true);
            }
        });
        
        textPopup.transform.SetParent(GameObject.Find("NickName").transform);
        textPopup.transform.localPosition = Vector3.zero;
        textPopup.SetActive(true);

    }

    private string NickNameCheck(string nickName)
    {
        if ("".Equals(nickName))
        {
            return "닉네임을 입력해주세요.";
        }

        if (nickName.Length > 10)
        {
            return "닉네임은 최대 10글자 입니다.";
        }

        if (nickName.Contains(" "))
        {
            return "띄어쓰기는 불가능합니다.";
        }

        return "";
        
    }

    private void CreatePopup(PopupType type, int num = 0, string nickName = "")
    {
        GameObject.Find("DontTouch").transform.GetChild(0).gameObject.SetActive(true);
        GameObject popup = Instantiate(Resources.Load("Prefabs/Popup") as GameObject);
        TMPro.TMP_Text[] texts = popup.transform.GetComponentsInChildren<TMPro.TMP_Text>();

        Button[] btns = popup.transform.GetComponentsInChildren<Button>();
        btns[0].onClick.AddListener(() => {
            Destroy(popup);
            GameObject.Find("DontTouch").transform.GetChild(0).gameObject.SetActive(false);
        });
        switch (type)
        {
            case PopupType.CREATE:
                texts[0].text = "생성 하시겠습니까?";
                btns[1].onClick.AddListener(() => {
                    OnCreateSelectSlotClickOk(num, nickName);
                    Destroy(popup);
                    GameObject.Find("DontTouch").transform.GetChild(0).gameObject.SetActive(false);
                });
                break;
            case PopupType.DELETE:
                texts[0].text = "삭제 하시겠습니까?";
                btns[1].onClick.AddListener(() => {
                    OnDeleteSlotClickOk(num); Destroy(popup);
                    GameObject.Find("DontTouch").transform.GetChild(0).gameObject.SetActive(false);
                });
                break;
            case PopupType.START:
                if (num == 0)
                {
                    texts[0].text = "캐릭터를 선택해주세요.";
                    btns[1].onClick.AddListener(() => {
                        Destroy(popup);
                        GameObject.Find("DontTouch").transform.GetChild(0).gameObject.SetActive(false);
                    });
                } else
                {
                    texts[0].text = "게임 시작하시겠습니까?";
                    btns[1].onClick.AddListener(() => {
                        GameStartOk(num);
                        Destroy(popup);
                        GameObject.Find("DontTouch").transform.GetChild(0).gameObject.SetActive(false);
                    });
                }
                break;
        }
        popup.transform.SetParent(GameObject.Find("PopupParent").transform);
        popup.transform.localPosition = Vector3.zero;
        popup.SetActive(true);
    }
}
