using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Represents cursor's location
    public Transform cursor;

    //How far away camera should be from cursor
    public Vector3 offset;
    //Zoom camera in or out

    //Smooth camera movement
    public float smoothSpeed = 0.125f;
    public float smoothTime = 0.2f;
    private Vector3 velocity = Vector3.zero;

    //Dead Zone for cursor where camera doesn't move
    public Rect deadZone = new Rect(0.4f, 0.4f, 0.2f, 0.2f);

    //Zoom
    public float zoomSpeed = 5f;
    public float[] zooms = {20f, 60f, 100f};
    public int zoomIndex = 1;

    // Update is called once per frame
    void Update()
    {
        float currentFov = Camera.main.fieldOfView;

        //zoom in
        if (Input.GetKeyDown(KeyCode.I) && zoomIndex > 0)
        {
            zoomIndex--;
            Camera.main.fieldOfView = zooms[zoomIndex];
        }
        //zoom out
        if (Input.GetKeyDown(KeyCode.O) && zoomIndex < 2)
        {
            zoomIndex++;
            Camera.main.fieldOfView = zooms[zoomIndex];
        }
    }

    private void LateUpdate()
    {
        //Convert world position to viewport position
        Vector3 viewportPoint = Camera.main.WorldToViewportPoint(cursor.position);

        //Move camera if cursor outside of dead zone
        Vector3 nextPosition = cursor.position + offset;
        transform.position = Vector3.SmoothDamp(transform.position, nextPosition, ref velocity, smoothTime);
    }
}
