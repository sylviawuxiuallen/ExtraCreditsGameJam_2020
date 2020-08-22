using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCamera;

    private float CameraPanMultiplier = 0.8f;
    private bool ShiftHeld = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Use PageUp and PageDown to zoom out and in
        // Hold arrow keys to pan the camera
        // Hold Shift to pan faster

        if(Input.GetKeyDown(KeyCode.PageUp))
        {
            mainCamera.orthographicSize += 1.0f;
        }

        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            mainCamera.orthographicSize -= 1.0f;
        }

        if(Input.GetKey(KeyCode.UpArrow))
        {
            mainCamera.transform.position += new Vector3(0, CameraPanRate(), 0);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            mainCamera.transform.position += new Vector3(0, -CameraPanRate(), 0);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            mainCamera.transform.position += new Vector3(-CameraPanRate(), 0, 0);
        }

        if (Input.GetKey(KeyCode.RightArrow))
        {
            mainCamera.transform.position += new Vector3(CameraPanRate(), 0, 0);
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            ShiftHeld = true;
        }
        else
        {
            ShiftHeld = false;
        }
    }

    private float CameraPanRate()
    {
        if (ShiftHeld)
        {
            return CameraPanMultiplier * mainCamera.orthographicSize * Time.deltaTime * 3.0f;
        } else
        {
            return CameraPanMultiplier * mainCamera.orthographicSize * Time.deltaTime;
        }
    }
}
