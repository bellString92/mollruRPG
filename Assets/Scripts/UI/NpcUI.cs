using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NpcUI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape)) // esc키로 닫기
        {
            closedTab();
        }
    }
    public void closedTab() // 버튼 클릭시 호출해 창닫기
    {
        transform.gameObject.SetActive(false);
    }
}
