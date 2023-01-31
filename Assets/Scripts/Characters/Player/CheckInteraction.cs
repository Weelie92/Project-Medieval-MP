using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInteraction : MonoBehaviour
{
    //public List<GameObject> interactables = new List<GameObject>();
    //[SerializeField] private LayerMask _interactableLayer;
    //private float checkDistance = 2f;
    //public GameObject interactableObject;
    //public Transform head;
    //public RaycastHit hit;

    //private PlayerController _player;

    //private void Awake()
    //{
    //    interactableObject = null;
    //    _player = gameObject.GetComponent<PlayerController>();
    //}

    //public void Update()
    //{
    //    if (_player.isCustomizing || _player.isTakingSurvey) return;

    //    Ray ray = new Ray(head.position, gameObject.GetComponent<PlayerController>()._camera.transform.forward);

    //    if (Physics.Raycast(ray, out hit, checkDistance, _interactableLayer))
    //    {
    //        interactableObject = hit.collider.gameObject;
            
    //        interactableObject.GetComponent<Interactable>()._interactE.enabled = true;
            
    //    }
    //    else if (interactableObject != null && !interactableObject.GetComponent<Interactable>().showInteractE)
    //    {
    //        interactableObject.GetComponent<Interactable>()._interactE.enabled = false;
    //        interactableObject = null;
    //    }
    //    else
    //    {
    //        interactableObject = null;
    //    }
    //}
}

