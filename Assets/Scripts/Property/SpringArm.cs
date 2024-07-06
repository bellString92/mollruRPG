using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SpringArm : CameraProperty
{
    public LayerMask crashMask;
    public float rotSpeed = 10.0f;
    public Vector2 rotRange = new Vector2(-60, 80);
    public float zoomSpeed = 3.0f;
    public Vector2 zoomRange = new Vector2(0.5f, 8.0f);
    public float camOffset = 0.5f;
    Vector3 curRot;
    float camDist;
    float desireDist;
    // Start is called before the first frame update
    void Start()
    {
        curRot = transform.localRotation.eulerAngles;
        desireDist = camDist = -myCam.transform.localPosition.z;
    }

    // Update is called once per frame
    void Update()
    {
        //if (EventSystem.current.IsPointerOverGameObject()) return;

        float x = Input.GetAxis("Mouse X");
        transform.parent.Rotate(Vector3.up * x * rotSpeed);
        float y = -Input.GetAxis("Mouse Y");
        
        //curRot.x += y;
        //if (curRot.x > rotRange.y) curRot.x = rotRange.y;
        //if (curRot.x < rotRange.x) curRot.x = rotRange.x;
        curRot.x = Mathf.Clamp(curRot.x + y * rotSpeed, rotRange.x, rotRange.y);
        transform.localRotation = Quaternion.Euler(curRot);
        //transform.Rotate(Vector3.right * y * rotSpeed);

        desireDist = Mathf.Clamp(desireDist - Input.GetAxis("Mouse ScrollWheel") * zoomSpeed, 
            zoomRange.x, zoomRange.y);

        camDist = Mathf.Lerp(camDist, desireDist, Time.deltaTime * 5.0f);
        
        Ray ray = new Ray(transform.position, -transform.forward);
        if(Physics.Raycast(ray, out RaycastHit hit, camDist + camOffset, crashMask))
        {
            camDist = hit.distance - camOffset;
        }

        myCam.transform.localPosition = new Vector3( 0, 0, -camDist);
    }
}
