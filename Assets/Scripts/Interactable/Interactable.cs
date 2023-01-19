using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private bool _isInteractable = true;
    public Vector3 lookAtRotation;

    public Canvas _interactE;
    public GameObject _player;

    private void Start()
    {
        _interactE = GetComponentInChildren<Canvas>();
        _interactE.enabled = false;

        _player = GameObject.FindGameObjectWithTag("Player");
    }

    public void Interact()
    {
        switch (gameObject.tag)
        {
            case "Door":
                break;
            case "Customize":
                _interactE.enabled = false;
                _player.GetComponent<PlayerCustomize>().enabled = true;
                _player.GetComponent<PlayerCustomize>().Initialize(lookAtRotation);

                break;
            default:
                break;
        }

        
        
    }
}
