using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadLookAtDirection : MonoBehaviour
{
    public bool UseCameraDirection = true;
    public Transform DirectionReference;
    [Range(0,1)]
    public float HeadWeight = 1;
    [Range(0, 1)]
    public float SpineHeight = 0.05f;
    Animator anim;
    private void Start()
    {
        if (UseCameraDirection)
            DirectionReference = FindObjectOfType<Camera>().transform;
    }
    private void OnAnimatorIK(int layerIndex)
    {
        if (anim == null)
            anim = GetComponent<Animator>();

        anim.SetLookAtWeight(1,SpineHeight, HeadWeight);
        anim.SetLookAtPosition(DirectionReference.position + DirectionReference.forward * 50);
    }
}
