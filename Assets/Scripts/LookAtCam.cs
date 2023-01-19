using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    private Camera _cameraToLookAt;

    private void Start()
    {
        _cameraToLookAt = Camera.main;
    }


    void Update()
    {
        transform.LookAt(_cameraToLookAt.transform.position);
        transform.Rotate(0, 180, 0);
    }
}
