using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouseHide : MonoBehaviour
{
    private Coroutine mouseHideCor = null;
    private Coroutine mouseViewCor = null;

    // Start is called before the first frame update
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }

    void Start()
    {
        mouseHideCor = StartCoroutine(MouseHideCor());
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            StopMouseCor(mouseHideCor);
            mouseViewCor = StartCoroutine(MouseViewCor());
        }

        if (Input.GetKeyUp(KeyCode.LeftAlt))
        {
            StopMouseCor(mouseViewCor);
            mouseHideCor = StartCoroutine(MouseHideCor());
        }
    }

    void StopMouseCor(Coroutine cor)
    {
        if (cor != null)
        {
            StopCoroutine(cor);
            cor = null;
        }
    }

    IEnumerator MouseHideCor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        yield return null;
    }

    IEnumerator MouseViewCor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        yield return null;
    }
}
