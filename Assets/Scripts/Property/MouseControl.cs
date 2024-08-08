using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControl : MonoBehaviour
{
    bool isCursorVisible = false;
    bool isPressed = true;

    // Start is called before the first frame update
    void Start()
    {
        // 마우스를 숨기기
        Cursor.visible = isCursorVisible;
        // 마우스 중앙 고정 및 잠구기
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        // ESC 키를 누르면 마우스를 다시 보이게 하고 잠금을 해제합니다.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isCursorVisible = !isCursorVisible;
            UpdateCursorState();
        }

        if ((Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1)) && isPressed)
        {
            isCursorVisible = false;
            UpdateCursorState();
        }

        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            isPressed = false ;
            isCursorVisible = true;
            UpdateCursorState();
        }
        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            isPressed = true;
            isCursorVisible = false;
            UpdateCursorState();
        }
    }

    void UpdateCursorState()
    {
        // isCursorVisible 변수에 따라 마우스 커서 상태를 업데이트합니다.
        Cursor.visible = isCursorVisible;
        Cursor.lockState = isCursorVisible ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
