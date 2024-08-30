using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.TextCore.Text;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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

    private UnityEvent escapeAct;
    private UnityEvent enterAct;
    
    private Dictionary<int, CharacterData> characters = null;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        characterSlotArr = new GameObject[slotCount];
        createBtnArr = new GameObject[slotCount];
        nickNameArr = new GameObject[slotCount];
        deleteBtnArr = new GameObject[slotCount];
        playCharacters = new string[slotCount];

        escapeAct = new UnityEvent();
        enterAct = new UnityEvent();

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

        string characterPath = $"{Application.dataPath}/Data/SaveData/Characters.dat";

        characters = FileManager.LoadFromBinary<Dictionary<int, CharacterData>>(characterPath);
        
        for (int i = 1; i <= slotCount; i++)
        {
            int selNumber = i;
            GameObject character;
            if (characters == default) return;
            if (characters.ContainsKey(selNumber))
            {
                string jobSelect = characters[selNumber].JobSelect;
                string playCharacter = "Paladin";
                switch (jobSelect)
                {
                    case "maria":
                        playCharacter = "Maria";
                        character = Instantiate(Resources.Load("Prefabs/Character/Maria") as GameObject);
                        break;
                    case "paladin":
                    default:
                        playCharacter = "Paladin";
                        character = Instantiate(Resources.Load("Prefabs/Character/Paladin") as GameObject);
                        break;
                }
                character.transform.SetParent(characterSlotArr[i - 1].transform);
                character.transform.localPosition = Vector3.zero;
                character.transform.localRotation = Quaternion.identity;
                nickNameArr[selNumber - 1].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = characters[selNumber].NickName;
                nickNameArr[selNumber - 1].transform.GetChild(0).gameObject.SetActive(true);
                createBtnArr[selNumber - 1].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = "캐릭터 선택";
                deleteBtnArr[selNumber - 1].gameObject.SetActive(true);
                
                deleteBtnArr[selNumber - 1].GetComponent<Button>().onClick.AddListener(() =>
                {
                    GameObject popup = Instantiate(Resources.Load("Prefabs/Popup/Popup") as GameObject);
                    popup.name = "Popup";
                    popup.transform.SetParent(popupParent);
                    popup.transform.localPosition = Vector3.zero;
                    TMPro.TMP_Text[] texts = popup.GetComponentsInChildren<TMPro.TMP_Text>();
                    texts[0].text = "삭제하시겠습니까?";
                    texts[1].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        dontTouch.GetChild(0).gameObject.SetActive(false);
                        DestroyPopup(popup);
                    });
                    texts[2].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        nickNameArr[selNumber - 1].transform.GetChild(0).gameObject.SetActive(false);
                        createBtnArr[selNumber - 1].transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = "신규 캐릭터 생성";
                        deleteBtnArr[selNumber - 1].gameObject.SetActive(false);
                        Transform circles = transform.GetChild(selNumber - 1).GetChild(0);
                        for (int j = 0; j < circles.childCount; j++)
                        {
                            circles.GetChild(j).gameObject.SetActive(false);
                        }
                        Destroy(character);
                        dontTouch.GetChild(0).gameObject.SetActive(false);
                        DestroyPopup(popup);
                        playCharacters[selNumber - 1] = "";
                        characters.Remove(selNumber);
                        FileManager.SaveToBinary(characterPath, characters);

                    });
                    popup.SetActive(true);
                    dontTouch.GetChild(0).gameObject.SetActive(true);
                    escapeAct.AddListener(() => {
                        texts[1].transform.parent.GetComponent<Button>().onClick.Invoke();
                    });
                    enterAct.AddListener(() => {
                        texts[2].transform.parent.GetComponent<Button>().onClick.Invoke();
                    });
                });
                playCharacters[selNumber - 1] = playCharacter;
                PlayerPrefs.SetString("jobSelect", "");
            }
        }
    }

    public void OnCreateBtn(int num)
    {
        selNum = num + 1;
        if (transform.GetChild(num).childCount == 1)
        {
            GameObject popup = Instantiate(Resources.Load("Prefabs/Popup/Popup") as GameObject);
            popup.name = "Popup";
            popup.transform.SetParent(popupParent);
            popup.transform.localPosition = Vector3.zero;
            TMPro.TMP_Text[] texts = popup.GetComponentsInChildren<TMPro.TMP_Text>();
            texts[0].text = "생성하시겠습니까?";
            texts[1].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
            {
                dontTouch.GetChild(0).gameObject.SetActive(false);
                DestroyPopup(popup);
            });
            texts[2].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
            {
                PlayerPrefs.SetInt("selNum", selNum);
                //SceneChange.OnSceneChange("Customize");
                SceneManager.LoadSceneAsync(3);
                dontTouch.GetChild(0).gameObject.SetActive(false);
                DestroyPopup(popup);
            });
            popup.SetActive(true);
            dontTouch.GetChild(0).gameObject.SetActive(true);
            escapeAct.AddListener(() => {
                texts[1].transform.parent.GetComponent<Button>().onClick.Invoke();
            });
            enterAct.AddListener(() => {
                texts[2].transform.parent.GetComponent<Button>().onClick.Invoke();
            });
        }
        else
        {
            int i = 0;
            foreach (Transform t in transform)
            {
                if (i == num)
                {
                    Transform circles = t.GetChild(0);
                    for (int j = 0; j < circles.childCount; j++)
                    {
                        circles.GetChild(j).gameObject.SetActive(true);
                    }
                    t.GetComponentInChildren<Animator>().SetBool("idle", false);
                    t.GetComponentInChildren<Animator>().SetBool("select", true);
                    t.GetComponentInChildren<Animator>().SetTrigger("selectIdle");
                }
                else
                {
                    Transform circles = t.GetChild(0);
                    for (int j = 0; j < circles.childCount; j++)
                    {
                        circles.GetChild(j).gameObject.SetActive(false);
                    }
                    t.GetComponentInChildren<Animator>()?.SetBool("select", false);
                    t.GetComponentInChildren<Animator>()?.SetBool("idle", true);
                }
                i++;
            }
        }
    }

    public void OnStartBtn()
    {
        GameObject popup = Instantiate(Resources.Load("Prefabs/Popup/Popup") as GameObject);
        popup.name = "Popup";
        popup.transform.SetParent(popupParent);
        popup.transform.localPosition = Vector3.zero;
        TMPro.TMP_Text[] texts = popup.GetComponentsInChildren<TMPro.TMP_Text>();
        if (selNum == 0)
        {
            texts[0].text = "캐릭터를 선택해주세요.";
            texts[1].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
            {
                dontTouch.GetChild(0).gameObject.SetActive(false);
                DestroyPopup(popup);
            });
            texts[2].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
            {
                dontTouch.GetChild(0).gameObject.SetActive(false);
                DestroyPopup(popup);
            });
        } 
        else
        {
            texts[0].text = "게임을 시작하시겠습니까?";
            texts[1].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
            {
                dontTouch.GetChild(0).gameObject.SetActive(false);
                DestroyPopup(popup);
            });
            texts[2].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
            {
                PlayerPrefs.SetString("playCharacter", playCharacters[selNum - 1]);
                PlayerPrefs.SetString("nickName", nickNameArr[selNum - 1].GetComponentInChildren<TMPro.TMP_Text>().text);
                PlayerPrefs.SetString("nextSceneText", "몰루타운");
                PlayerPrefs.SetString("nextSceneImage", "Dungeon");
                SceneChange.OnSceneChange("3. Village");
            });
        }
        popup.SetActive(true);
        dontTouch.GetChild(0).gameObject.SetActive(true);
        escapeAct.AddListener(() => {
            texts[1].transform.parent.GetComponent<Button>().onClick.Invoke();
        });
        enterAct.AddListener(() => {
            texts[2].transform.parent.GetComponent<Button>().onClick.Invoke();
        });
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && popupParent.childCount > 0)
        {
            OnEscapeEvent();
        }

        if (Input.GetKeyDown(KeyCode.Return) && popupParent.childCount > 0)
        {
            OnEnterEvent();
        }
    }

    public void OnEscapeEvent()
    {
        escapeAct?.Invoke();
    }

    public void OnEnterEvent()
    {
        enterAct?.Invoke();
    }
    
    public void DestroyPopup(GameObject popup)
    {
        escapeAct.RemoveAllListeners();
        enterAct.RemoveAllListeners();
        EventSystem.current.SetSelectedGameObject(null);
        Destroy(popup);
    }

}
