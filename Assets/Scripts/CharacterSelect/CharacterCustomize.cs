using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCustomize : MonoBehaviour
{
    private class Descript
    {
        public string _descript { get; private set; }

        public Descript(string descript)
        {
            this._descript = descript;
        }
    }
    
    private Dictionary<string, Descript> desc = null;
    [SerializeField] private Transform description;
    private string[] characterNames;
    [SerializeField] private LayerMask playerLayer;
    private Animator animator;
    private int selNum = -1;
    private bool selChk = true;
    [SerializeField] private TMPro.TMP_InputField nickName;
    [SerializeField] private TMPro.TMP_Text errorText;
    [SerializeField] private Transform popupParent;
    [SerializeField] private Transform dontTouch;
    private UnityEvent escapeAct;
    [SerializeField] private UnityEvent enterAct;

    private void Start()
    {
        escapeAct = new UnityEvent();
        enterAct = new UnityEvent();

        Descript warrior = new Descript(
            "<size=60><color=#FFFFFF>전사</color></size>\r\n전사는 오로지 전투를 위해 태어났습니다.\r\n강인한 신체와 강력한 검을 휘두르며 언제나 전장의 선봉에 섭니다. 전사는 평생을 검과 함께 영광을 위해 살아갑니다.");
        Descript magician = new Descript(
            "<size=60><color=#FFFFFF>마법사</color></size>\r\n마법사는 오랜 수련을 통해 화염과 얼음 원소를 자유자재로 다루는 것에 통달한 존재입니다. 적들을 화염으로 태우고 얼음으로 움직임을 봉쇄합니다.\r\n이 세계에서 마법사가 제압하지 못 할 적들은 없습니다.");

        desc = new Dictionary<string, Descript>()
        {
            {"warrior", warrior},
            {"magician", magician},
        };

        characterNames = new string[3];
        string characterPath = $"{Application.dataPath}/Data/SaveData/Characters.dat";
        Dictionary<int, CharacterData> characters = FileManager.LoadFromBinary<Dictionary<int, CharacterData>>(characterPath);
        if (characters != default)
        {
            for (int i = 0; i < characterNames.Length; i++)
            {
                if (characters.ContainsKey(i + 1))
                {
                    characterNames[i] = characters[i + 1].NickName;
                }
            }
        }

        nickName.onSelect.AddListener((string text) =>
        {
            enterAct.AddListener(() =>
            {
                OnCreateBtn();
            });
        });

        nickName.onDeselect.AddListener((string text) =>
        {
            enterAct.RemoveAllListeners();
        });
    }




    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && selChk)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, playerLayer))
            {
                if (selNum == hit.transform.GetSiblingIndex()) return;
                selNum = hit.transform.GetSiblingIndex();
                CharacterSelect();
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnEscapeEvent();
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            OnEnterEvent();
        }
    }

    private void CharacterSelect()
    {
        CharacterSelectAnimator();
        CharacterSelectDescript();
    }

    private void CharacterSelectAnimator()
    {
        if (animator != null)
        {
            animator.SetBool("idle", true);
            animator.SetBool("select", false);
        }
        animator = transform.GetChild(selNum).GetComponent<Animator>();
        animator.SetBool("idle", false);
        animator.SetBool("select", true);
        animator.SetTrigger("attack");
        selChk = false;
        StartCoroutine(WaitSeconds(1.0f));
    }

    private void CharacterSelectDescript()
    {
        description.gameObject.SetActive(true);
        if (selNum < 2)
        {
            description.GetComponentInChildren<TMPro.TMP_Text>().text = desc["warrior"]._descript;
        }
        else
        {
            description.GetComponentInChildren<TMPro.TMP_Text>().text = desc["magician"]._descript;
        }
    }

    IEnumerator WaitSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        selChk = true;
    }

    public void OnBackBtn()
    {
        SceneManager.LoadSceneAsync(2);
    }

    public void OnCreateBtn()
    {
        if (selNum < 0)
        {
            notSelectPopup();
            return;
        }

        string nickNameChk = NicknameCheck(nickName.text);
        if (!string.Empty.Equals(nickNameChk))
        {
            errorText.text = nickNameChk;
            return;
        }

        errorText.text = string.Empty;

        selChk = false;
        CreatePopup(selNum < 2);

    }

    private void notSelectPopup()
    {
        GameObject popup = Instantiate(Resources.Load("Prefabs/Popup/Popup") as GameObject);
        popup.name = "Popup";
        popup.transform.SetParent(popupParent);
        popup.transform.localPosition = Vector3.zero;
        TMPro.TMP_Text[] texts = popup.GetComponentsInChildren<TMPro.TMP_Text>();
        texts[0].text = "캐릭터를 선택해주세요.";
        RectTransform btn = (texts[1].transform.parent.parent.transform as RectTransform);
        Vector2 btnSize = btn.sizeDelta;
        btnSize.x = 200.0f;
        btn.sizeDelta = btnSize;
        texts[1].transform.parent.GetComponent<Button>().gameObject.SetActive(false);
        texts[2].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
        {
            dontTouch.GetChild(0).gameObject.SetActive(false);
            selChk = true;
            DestroyPopup(popup);
            EventSystem.current.SetSelectedGameObject(nickName.gameObject);
        });

        popup.SetActive(true);

        dontTouch.GetChild(0).gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(popup);

        escapeAct.AddListener(() => {
            texts[2].transform.parent.GetComponent<Button>().onClick.Invoke();
        });
        enterAct.AddListener(() => {
            texts[2].transform.parent.GetComponent<Button>().onClick.Invoke();
        });
    }

    private void CreatePopup(bool yet)
    {
        if (!yet)
        {
            DontReady();
            return;
        }
        GameObject popup = Instantiate(Resources.Load("Prefabs/Popup/Popup") as GameObject);
        popup.name = "Popup";
        popup.transform.SetParent(popupParent);
        popup.transform.localPosition = Vector3.zero;
        TMPro.TMP_Text[] texts = popup.GetComponentsInChildren<TMPro.TMP_Text>();
        texts[0].text = "캐릭터를 생성하시겠습니까?";
        texts[1].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
        {
            dontTouch.GetChild(0).gameObject.SetActive(false);
            selChk = true;
            DestroyPopup(popup);
            EventSystem.current.SetSelectedGameObject(nickName.gameObject);
        });
        texts[2].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
        {
            CreateCharacter();
            dontTouch.GetChild(0).gameObject.SetActive(false);
            DestroyPopup(popup);
        });


        popup.SetActive(true);

        dontTouch.GetChild(0).gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(popup);

        escapeAct.AddListener(() => {
            texts[1].transform.parent.GetComponent<Button>().onClick.Invoke();
        });
        enterAct.AddListener(() => {
            texts[2].transform.parent.GetComponent<Button>().onClick.Invoke();
        });
    }

    private void CreateCharacter()
    {
        string jobSelect;
        switch (selNum)
        {
            case 1:
                jobSelect = "maria";
                break;
            case 2:
                jobSelect = "pelegrini";
                break;
            case 3:
                jobSelect = "kachujin";
                break;
            case 0:
            default:
                jobSelect = "paladin";
                break;
        }
        int selNumber = PlayerPrefs.GetInt("selNum");
        CharacterData data = new CharacterData(selNumber, jobSelect, nickName.text);
        string characterPath = $"{Application.dataPath}/Data/SaveData/Characters.dat";
        Dictionary<int, CharacterData> characters = FileManager.LoadFromBinary<Dictionary<int, CharacterData>>(characterPath);
        if (characters == default) characters = new Dictionary<int, CharacterData>();
        if (characters.ContainsKey(selNumber))
            characters[selNumber] = data;
        else
            characters.Add(selNumber, data);
        FileManager.SaveToBinary(characterPath, characters);
        SceneManager.LoadSceneAsync(2);
    }

    private void DontReady()
    {
        GameObject popup = Instantiate(Resources.Load("Prefabs/Popup/Popup") as GameObject);
        popup.name = "Popup";
        popup.transform.SetParent(popupParent);
        popup.transform.localPosition = Vector3.zero;
        TMPro.TMP_Text[] texts = popup.GetComponentsInChildren<TMPro.TMP_Text>();
        texts[0].text = "준비 중 입니다.";
        RectTransform btn = (texts[1].transform.parent.parent.transform as RectTransform);
        Vector2 btnSize = btn.sizeDelta;
        btnSize.x = 200.0f;
        btn.sizeDelta = btnSize;
        texts[1].transform.parent.GetComponent<Button>().gameObject.SetActive(false);
        texts[2].transform.parent.GetComponent<Button>().onClick.AddListener(() =>
        {
            dontTouch.GetChild(0).gameObject.SetActive(false);
            selChk = true;
            DestroyPopup(popup);
            EventSystem.current.SetSelectedGameObject(nickName.gameObject);
        });

        popup.SetActive(true);

        dontTouch.GetChild(0).gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(popup);

        escapeAct.AddListener(() => {
            texts[2].transform.parent.GetComponent<Button>().onClick.Invoke();
        });
        enterAct.AddListener(() => {
            texts[2].transform.parent.GetComponent<Button>().onClick.Invoke();
        });
    }


    private string NicknameCheck(string text)
    {
        string result = "";

        if (text == "")
        {
            result = "닉네임을 작성해주세요.";
        }
        else if (text.Length < 2 || text.Length > 10)
        {
            result = "닉네임 길이는 2자에서 10자입니다.";
        }
        else if (text.Contains(" "))
        {
            result = "닉네임에 띄어쓰기가 있습니다.";
        }
        else if (characterNames.Contains(text))
        {
            result = "이미 사용중인 닉네임입니다.";
        }
        else
        {
            Regex reg = new Regex("[ㄱ-ㅎㅏ-ㅣ]");
            if (reg.IsMatch(text))
            {
                result = "잘못된 아이디 입니다.";
            }
        }

        return result;
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
