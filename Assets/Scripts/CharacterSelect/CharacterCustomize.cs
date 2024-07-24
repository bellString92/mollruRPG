using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterCustomize : MonoBehaviour
{
    private class Descript {
        public string _descript { get; private set; }
        public string _status { get; private set; }

        public Descript(string descript, string status)
        {
            this._descript = descript;
            this._status = status;
        }
    }

    private Dictionary<string, Descript> desc = null;
    public Transform character = null;
    public Transform characterSlot = null;
    private Button[] btns = null;
    public TMPro.TMP_Text descriptionText = null;
    public TMPro.TMP_Text statusText = null;
    public Transform popupParent = null;
    private string jobSelect = "paladin_warrior";

    // Start is called before the first frame update
    void Start()
    {
        btns = characterSlot.GetComponentsInChildren<Button>();
        Descript warrior = new Descript(
            "<size=60><color=#FFFFFF>전사</color></size>\r\n전사는 오로지 전투를 위해 태어났습니다.\r\n강인한 신체와 강력한 검을 휘두르며 언제나 전장의 선봉에 섭니다. 전사는 평생을 검과 함께 영광을 위해 살아갑니다.",
            "     스테이터스\r\n\r\n   힘\t:\t10\r\n  민첩\t:\t5\r\n  지능\t:\t1\r\n   운\t:\t5\r\n 공격력\t:\t10\r\n  마력\t:\t1"
            );
        Descript magician = new Descript(
            "<size=60><color=#FFFFFF>마법사</color></size>\r\n마법사는 오랜 수련을 통해 화염과 얼음 원소를 자유자재로 다루는 것에 통달한 존재입니다. 적들을 화염으로 태우고 얼음으로 움직임을 봉쇄합니다.\r\n이 세계에서 마법사가 제압하지 못 할 적들은 없습니다.",
            "     스테이터스\r\n\r\n   힘\t:\t1\r\n  민첩\t:\t5\r\n  지능\t:\t1\r\n   운\t:\t5\r\n 공격력\t:\t1\r\n  마력\t:\t10"
            );

        desc = new Dictionary<string, Descript>()
        {
            {"warrior", warrior},
            {"magician", magician},
        };
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void JobBtnClick(int num)
    {
        switch (num){
            case 0:
                jobSelect = "paladin_warrior";
                break;
            case 1:
                jobSelect = "maria_warrior";
                break;
            case 2:
                jobSelect = "paladin_magician";
                break;
            case 3:
                jobSelect = "maria_magician";
                break;
        }
        if (num % 2 == 0)
        {
            character.GetChild(0).gameObject.SetActive(true);
            character.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            character.GetChild(0).gameObject.SetActive(false);
            character.GetChild(1).gameObject.SetActive(true);
        }

        if (num < 2)
        {
            descriptionText.text = desc["warrior"]._descript;
            statusText.text = desc["warrior"]._status;
        } else
        {
            descriptionText.text = desc["magician"]._descript;
            statusText.text = desc["magician"]._status;
        }

        Color color = btns[num].GetComponent<Image>().color;

        for (int i = 0; i < btns.Length; i++)
        {
            if (i == num) color.a = 1.0f;
            else color.a = 0.3f;
            btns[i].GetComponent<Image>().color = color;
        }
    }

    public void OnStartBtn()
    {
        GameObject textPopup = Instantiate(Resources.Load("Prefabs/TextPopup") as GameObject);
        textPopup.name = "TextPopup";
        textPopup.transform.SetParent(popupParent);
        textPopup.transform.localPosition = Vector3.zero;
        TMPro.TMP_Text[] texts = textPopup.GetComponentsInChildren<TMPro.TMP_Text>();

        texts[0].text = "닉네임 설정";
        texts[4].text = "취소";
        texts[5].text = "확인";

        Button[] btn = textPopup.GetComponentsInChildren<Button>();
        btn[0].onClick.AddListener(() =>
        {
            Destroy(textPopup);
        });

        btn[1].onClick.AddListener(() =>
        {
            string text = textPopup.GetComponentInChildren<TMPro.TMP_InputField>().text;
            string chkText = NicknameCheck(text);
            if (chkText == "")
            {
                texts[3].gameObject.SetActive(false);
                GameObject popup = Instantiate(Resources.Load("Prefabs/popup") as GameObject);
                popup.name = "Popup";
                popup.transform.SetParent(popupParent);
                popup.transform.localPosition = Vector3.zero;
                TMPro.TMP_Text[] popupTexts = popup.GetComponentsInChildren<TMPro.TMP_Text>();
                popupTexts[0].text = "캐릭터를 생성하시겠습니까?";
                popupTexts[1].text = "취소";
                popupTexts[2].text = "확인";
                Button[] popupBtn = popup.GetComponentsInChildren<Button>();
                popupBtn[0].onClick.AddListener(() =>
                {
                    Destroy(popup);
                });
                popupBtn[1].onClick.AddListener(() =>
                {
                    PlayerPrefs.SetString("jobSelect", jobSelect);
                    PlayerPrefs.SetString("nickName", text);
                    Destroy(textPopup);
                    Destroy(popup);
                    SceneManager.LoadSceneAsync(2);
                });
                popup.SetActive(true);
            }
            else
            {
                texts[3].text = chkText;
                texts[3].gameObject.SetActive(true);
            }
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
}
