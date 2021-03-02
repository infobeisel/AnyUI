using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSCamera : MonoBehaviour {
    private static float MOUSE_DELTA_FACTOR = 5.0f;
    private Vector2 oldMousePosition;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        oldMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    void Update () {
        Vector2 mouseDelta = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) - oldMousePosition;
        mouseDelta *= MOUSE_DELTA_FACTOR;
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Camera.main.transform.Rotate(0.0f, mouseDelta.x, 0.0f, Space.World);
            Camera.main.transform.Rotate(-mouseDelta.y, 0.0f, 0.0f, Space.Self);
            //oldMousePosition = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        }

        if(Input.GetKeyUp(KeyCode.LeftAlt))
        {
            Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
}
