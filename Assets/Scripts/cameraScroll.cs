using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraScroll : MonoBehaviour
{
    private Camera camera;
    private void Awake()
    {
        camera = GetComponent<Camera>();
    }
    void Update()
    {
        if (Input.mouseScrollDelta != Vector2.zero)
        {

            camera.orthographicSize = camera.orthographicSize - Input.mouseScrollDelta.y;
        }
    }
}
