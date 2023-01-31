using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCam : MonoBehaviour
{
    private Camera _cameraToLookAt;

    [SerializeField] Collider[] colliders;

    private void Start()
    {
        GetComponent<Canvas>().enabled = false;
    }

    void Update()
    {
        colliders = Physics.OverlapSphere(transform.position, 3f, LayerMask.GetMask("Player"));

        if (colliders.Length > 0)
        {
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.tag == "Player")
                {
                    GetComponent<Canvas>().enabled = true;
                    _cameraToLookAt = collider.gameObject.GetComponentInChildren<Camera>();
                }
            }

            if (_cameraToLookAt == null) return;

            transform.LookAt(_cameraToLookAt.transform.position);
            transform.Rotate(0, 180, 0);
        }
        else
        {
            GetComponent<Canvas>().enabled = false;
        }
    }
}
