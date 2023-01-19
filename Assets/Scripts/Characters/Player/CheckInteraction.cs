using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckInteraction : MonoBehaviour
{
    public List<GameObject> interactables = new List<GameObject>();
    [SerializeField] private LayerMask _interactableLayer;
    private float checkDistance = 2f;
    public GameObject interactableObject;
    public Transform head;
    public RaycastHit hit;

    private void Awake()
    {
        interactableObject = null;
    }

    public void CheckInteractable()
    {
        Ray ray = new Ray(head.position, Camera.main.transform.forward);

        if (Physics.Raycast(ray, out hit, checkDistance, _interactableLayer))
        {
            interactableObject = hit.collider.gameObject;
            
            interactableObject.GetComponent<Interactable>()._interactE.enabled = true;
            
        }
        else if (interactableObject != null)
        {
            interactableObject.GetComponent<Interactable>()._interactE.enabled = false;
            interactableObject = null;
        }
    }
    
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(head.position, Camera.main.transform.forward);
    }


}

