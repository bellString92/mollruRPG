using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.TextCore.Text;

public class CharacterSelect : MonoBehaviour
{
    private int selNum = 0;

    public int slotCount = 3;
    public Transform characterSlot = null;
    public Transform createBtn = null;
    public Transform nickName = null;
    public Transform deleteBtn = null;
    private GameObject[] characterSlotArr = null;
    private GameObject[] createBtnArr = null;
    private GameObject[] nickNameArr = null;
    private GameObject[] deleteBtnArr = null;
    private string[] playCharacters = null;

    public Transform popupParent = null;
    public Transform dontTouch = null;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        characterSlotArr = new GameObject[slotCount];
        createBtnArr = new GameObject[slotCount];
        nickNameArr = new GameObject[slotCount];
        deleteBtnArr = new GameObject[slotCount];
        playCharacters = new string[slotCount];

        for (int i = 0; i < slotCount; i++)
        {
            Vector3 oriViewportPoint = new Vector3(1.0f / (slotCount + 1) * (i + 1), 0.5f, 0);
            Vector3 worldPoint = Camera.main.ViewportToWorldPoint(oriViewportPoint);
            worldPoint.y = 0; worldPoint.z = 0;
            Vector3 screenPoint = Camera.main.ViewportToScreenPoint(oriViewportPoint);

            Vector3 changeViewportPoint = oriViewportPoint;


            if (i == 0)
            {
                characterSlotArr[i] = characterSlot.GetChild(i).gameObject;
                worldPoint.y = 0; worldPoint.z = 0;
                characterSlotArr[i].transform.position = worldPoint;
                changeViewportPoint.y = 0.2f;
                screenPoint = Camera.main.ViewportToScreenPoint(changeViewportPoint);
                createBtnArr[i] = createBtn.GetChild(i).gameObject;
                createBtnArr[i].transform.position = screenPoint;
                changeViewportPoint.y = 0.3f;
                screenPoint = Camera.main.ViewportToScreenPoint(changeViewportPoint);
                nickNameArr[i] = nickName.GetChild(i).gameObject;
                nickNameArr[i].transform.position = screenPoint;
                changeViewportPoint.y = 0.8f;
                screenPoint = Camera.main.ViewportToScreenPoint(changeViewportPoint);
                deleteBtnArr[i] = deleteBtn.GetChild(i).gameObject;
                deleteBtnArr[i].transform.position = screenPoint;
            }
            else
            {
                characterSlotArr[i] = Instantiate(characterSlotArr[0], characterSlot);
                characterSlotArr[i].name = $"Slot{i + 1}";
                characterSlotArr[i].transform.position = worldPoint;
                createBtnArr[i] = Instantiate(createBtnArr[0], createBtn);
                createBtnArr[i].name = $"CreateBtn{i + 1}";
                changeViewportPoint.y = 0.2f;
                screenPoint = Camera.main.ViewportToScreenPoint(changeViewportPoint);
                createBtnArr[i].transform.position = screenPoint;
                nickNameArr[i] = Instantiate(nickNameArr[0], nickName);
                nickNameArr[i].name = $"NickName{i + 1}";
                changeViewportPoint.y = 0.3f;
                screenPoint = Camera.main.ViewportToScreenPoint(changeViewportPoint);
                nickNameArr[i].transform.position = screenPoint;
                deleteBtnArr[i] = Instantiate(deleteBtnArr[0], deleteBtn);
                deleteBtnArr[i].name = $"DeleteBtn{i + 1}";
                changeViewportPoint.y = 0.8f;
                screenPoint = Camera.main.ViewportToScreenPoint(changeViewportPoint);
                deleteBtnArr[i].transform.position = screenPoint;
            }

        }
        string jobSelect = PlayerPrefs.GetString("jobSelect");
        GameObject character = null;
        string playCharacter = "Paladin";
        if (!"".Equals(jobSelect))
        {
            switch (jobSelect)
            {
                case "paladin_warrior":
                case "paladin_magician":
                    playCharacter = "Paladin";
                    character = Instantiate(Resources.Load("Prefabs/Paladin") as GameObject);
                    break;
                case "maria_warrior":
                case "maria_magician":
                    playCharacter = "Maria";
                    character = Instantiate(Resources.Load("Prefabs/Maria") as GameObject);
                    break;
            }
            int selNum = PlayerPrefs.GetInt("selNum");
            character.transform.SetParent(characterSlotArr[selNum-1].transform);
            character.transform.localPosition = Vector3.zero;
            nickNameArr[selNum-1].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = PlayerPrefs.GetString("nickName");
            nickNameArr[selNum-1].transform.GetChild(0).gameObject.SetActive(true);
            createBtnArr[selNum-1].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = "캐릭터 선택";
            deleteBtnArr[selNum-1].gameObject.SetActive(true);
            deleteBtnArr[selNum-1].GetComponent<Button>().onClick.AddListener(() =>
            {
                GameObject popup = Instantiate(Resources.Load("Prefabs/Popup") as GameObject);
                popup.name = "Popup";
                popup.transform.SetParent(popupParent);
                popup.transform.localPosition = Vector3.zero;
                TMPro.TMP_Text[] texts = popup.GetComponentsInChildren<TMPro.TMP_Text>();
                texts[0].text = "삭제하시겠습니까?";
                texts[1].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
                {
                    dontTouch.GetChild(0).gameObject.SetActive(false);
                    Destroy(popup);
                });
                texts[2].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
                {
                    nickNameArr[selNum-1].transform.GetChild(0).gameObject.SetActive(false);
                    createBtnArr[selNum-1].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = "신규 캐릭터 생성";
                    deleteBtnArr[selNum-1].gameObject.SetActive(false);
                    Transform circles = transform.GetChild(selNum-1).GetChild(0);
                    for (int j = 0; j < circles.childCount; j++)
                    {
                        circles.GetChild(j).gameObject.SetActive(false);
                    }
                    Destroy(character);
                    dontTouch.GetChild(0).gameObject.SetActive(false);
                    Destroy(popup);
                    playCharacters[selNum-1] = "";
                    selNum = 0;
                });
                popup.SetActive(true);
                dontTouch.GetChild(0).gameObject.SetActive(true);
            });
            playCharacters[selNum-1] = playCharacter;
            PlayerPrefs.SetString("jobSelect", "");
        }
    }

    public void OnCreateBtn(int num)
    {
        selNum = num + 1;
        if (transform.GetChild(num).childCount == 1)
        {
            GameObject popup = Instantiate(Resources.Load("Prefabs/Popup") as GameObject);
            popup.name = "Popup";
            popup.transform.SetParent(popupParent);
            popup.transform.localPosition = Vector3.zero;
            TMPro.TMP_Text[] texts = popup.GetComponentsInChildren<TMPro.TMP_Text>();
            texts[0].text = "생성하시겠습니까?";
            texts[1].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
            {
                dontTouch.GetChild(0).gameObject.SetActive(false);
                Destroy(popup);
            });
            texts[2].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
            {
                PlayerPrefs.SetInt("selNum", selNum);
                //SceneChange.OnSceneChange("Customize");
                SceneManager.LoadSceneAsync(4);
                dontTouch.GetChild(0).gameObject.SetActive(false);
                Destroy(popup);
            });
            popup.SetActive(true);
            dontTouch.GetChild(0).gameObject.SetActive(true);
        }
        else
        {
            int i = 0;
            foreach (Transform t in transform)
            {
                if (i == num)
                {
                    Transform circles = transform.GetChild(i).GetChild(0);
                    for (int j = 0; j < circles.childCount; j++)
                    {
                        circles.GetChild(j).gameObject.SetActive(true);
                    }

                }
                else
                {
                    Transform circles = transform.GetChild(i).GetChild(0);
                    for (int j = 0; j < circles.childCount; j++)
                    {
                        circles.GetChild(j).gameObject.SetActive(false);
                    }
                }
                i++;
            }
        }
    }

    public void OnStartBtn()
    {
        if (selNum == 0)
        {
            GameObject popup = Instantiate(Resources.Load("Prefabs/Popup") as GameObject);
            popup.name = "Popup";
            popup.transform.SetParent(popupParent);
            popup.transform.localPosition = Vector3.zero;
            TMPro.TMP_Text[] texts = popup.GetComponentsInChildren<TMPro.TMP_Text>();
            texts[0].text = "캐릭터를 선택해주세요.";
            texts[1].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
            {
                dontTouch.GetChild(0).gameObject.SetActive(false);
                Destroy(popup);
            });
            texts[2].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
            {
                dontTouch.GetChild(0).gameObject.SetActive(false);
                Destroy(popup);
            });
            popup.SetActive(true);
            dontTouch.GetChild(0).gameObject.SetActive(true);
        } else
        {
            PlayerPrefs.SetString("playCharacter", playCharacters[selNum-1]);
            SceneChange.OnSceneChange("MollRuRPGScene");
        }

    }

}
