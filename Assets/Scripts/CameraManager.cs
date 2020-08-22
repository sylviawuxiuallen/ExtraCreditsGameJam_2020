using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public KeyCode MoveCameraUp = KeyCode.W;
    public KeyCode MoveCameraDown = KeyCode.S;
    public KeyCode MoveCameraLeft = KeyCode.A;
    public KeyCode MoveCameraRight = KeyCode.D;

    public bool enableMouseControl = true;

    public float zoomRate = -100.0f;
    public float panSpeed = 5.0f;
    public float zoomPanMult = 0.3f;

    public float mouseBoundarySize = 32.0f;

    [Range(1.0f, 100.0f)]

    public float minZoom = 2.5f;

    [Range(1.0f, 100.0f)]
    public float maxZoom = 50.0f;

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
        // mainCamera.orthographicSize
        Vector3 pan = Vector3.zero;
        pan = getKeyMovement(pan);

        if(enableMouseControl)
            pan = getMouseMovement(pan);

        Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize + zoomRate * Input.mouseScrollDelta.y * Time.deltaTime,minZoom,maxZoom);
        

        Camera.main.transform.position += pan * Time.deltaTime * zoomPanMult * Camera.main.orthographicSize;
    }

    private Vector3 getMouseMovement(Vector3 pan)
    {
        Vector3 mousePos = Input.mousePosition;

        //move UP
        if (mousePos.y > Camera.main.pixelHeight - mouseBoundarySize){
            pan += new Vector3(0, 1, 0) * panSpeed;}

        //move DOWN
        if (mousePos.y < mouseBoundarySize){
            pan += new Vector3(0, -1, 0) * panSpeed;}

        //move LEFT
        if (mousePos.x < mouseBoundarySize){
            pan += new Vector3(-1, 0, 0) * panSpeed;}

        //move RIGHT
        if (mousePos.x > Camera.main.pixelWidth - mouseBoundarySize){
            pan += new Vector3(1, 0, 0) * panSpeed;}

        return pan;
    }

    private Vector3 getKeyMovement(Vector3 pan)
    {
        if(Input.GetKey(MoveCameraUp))
        {
            pan += new Vector3(0, 1, 0) * panSpeed;
        }
        if (Input.GetKey(MoveCameraDown))
        {
            pan += new Vector3(0, -1, 0) * panSpeed;
        }
        if (Input.GetKey(MoveCameraLeft))
        {
            pan += new Vector3(-1, 0, 0) * panSpeed;
        }
        if (Input.GetKey(MoveCameraRight))
        {
            pan += new Vector3(1, 0, 0) * panSpeed;
        }
        return pan;
    }
}
