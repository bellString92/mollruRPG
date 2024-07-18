using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class NickName : MonoBehaviour
{
    public GameObject Cam;
    Vector3 startScale;
    public float distance = 8;

    // Start is called before the first frame update
    void Start()
    {
        string nickName = PlayerPrefs.GetString("nickName");
        if ("".Equals(nickName)) nickName = "닉네임";
        gameObject.GetComponent<TMPro.TMP_Text>().text = nickName;
        gameObject.GetComponent<TMPro.TMP_Text>().outlineColor = Color.white;
        gameObject.GetComponent<TMPro.TMP_Text>().outlineWidth = 0.05f;
        startScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Cam.transform.rotation;
        float dist = Vector3.Distance(Cam.transform.position, transform.position);
        Vector3 newScale = startScale * dist / distance;
        transform.localScale = newScale;

        transform.rotation = Cam.transform.rotation;
    }
}
