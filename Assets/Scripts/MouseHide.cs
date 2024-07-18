using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouseHide : MonoBehaviour
{
    private Coroutine mouseHideCor = null;
    private Coroutine mouseViewCor = null;
    public bool mouseHide { get; private set; }
    public static MouseHide Instance = null;
    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartMouseHide();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftAlt))
        {
            if (mouseHide)
            {
                StartMouseView();
            }
            else
            {
                StartMouseHide();
            }
        }

    }

    public void StartMouseHide()
    {
        StopMouseCor(mouseViewCor);
        mouseHideCor = StartCoroutine(MouseHideCor());
        mouseHide = true;
    }

    public void StartMouseView()
    {
        StopMouseCor(mouseHideCor);
        mouseViewCor = StartCoroutine(MouseViewCor());
        mouseHide = false;
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
