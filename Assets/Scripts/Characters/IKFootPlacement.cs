using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKFootPlacement : MonoBehaviour
{
    Animator _playerAnimator;

    public float DistanceToGround = 0.06f;

    public LayerMask layerMask;

    public GameObject LeftFoot;
    public GameObject RightFoot;




    void Start()
    {
        _playerAnimator = GetComponent<Animator>();
    }


    private void OnAnimatorIK(int layerIndex)
    {
        if (_playerAnimator)
        {
            _playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1f);
            _playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, 1f);

            _playerAnimator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1f);
            _playerAnimator.SetIKRotationWeight(AvatarIKGoal.RightFoot, 1f);

            RaycastHit hitLeft;
            RaycastHit hitRight;
            RaycastHit hitLeftBase;
            RaycastHit hitRightBase;

            Ray rayLeft = new Ray(_playerAnimator.GetIKPosition(AvatarIKGoal.LeftFoot) + Vector3.up, Vector3.down);
            Ray rayLeftBase = new Ray(LeftFoot.transform.position + Vector3.up, Vector3.down);

            Ray rayRight = new Ray(_playerAnimator.GetIKPosition(AvatarIKGoal.RightFoot) + Vector3.up, Vector3.down);
            Ray rayRightBase = new Ray(RightFoot.transform.position + Vector3.up, Vector3.down);

            Physics.Raycast(rayLeftBase, out hitLeftBase, 2f);
            Physics.Raycast(rayRightBase, out hitRightBase, 2f);
            

            // Left foot

            if (Physics.Raycast(rayLeft, out hitLeft, DistanceToGround + 1f, layerMask))
            {
                if (hitLeft.collider.gameObject.layer == LayerMask.NameToLayer("Walkable"))
                {
                    Vector3 footPosition = hitLeft.point;

                    footPosition.y += DistanceToGround;
                    _playerAnimator.SetIKPosition(AvatarIKGoal.LeftFoot, footPosition);
                    Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hitLeft.normal);
                    _playerAnimator.SetIKRotation(AvatarIKGoal.LeftFoot, Quaternion.LookRotation(forward, hitLeft.normal));
                }

                
            } 


            // Right foot

            if (Physics.Raycast(rayRight, out hitRight, DistanceToGround + 1f, layerMask))
            {
                if (hitRight.collider.gameObject.layer == LayerMask.NameToLayer("Walkable"))
                {
                    Vector3 footPosition = hitRight.point;
                    footPosition.y += DistanceToGround;
                    _playerAnimator.SetIKPosition(AvatarIKGoal.RightFoot, footPosition);
                    Vector3 forward = Vector3.ProjectOnPlane(transform.forward, hitRight.normal);
                    _playerAnimator.SetIKRotation(AvatarIKGoal.RightFoot, Quaternion.LookRotation(forward, hitRight.normal));
                }
            }


            Debug.Log("Right foot: " + hitRightBase.distance);
            Debug.Log("Left foot: " + hitLeftBase.distance);

            if (!_playerAnimator.GetBool("isMoving") || !_playerAnimator.GetBool("isFalling"))
            {

                if (hitRightBase.distance > hitLeftBase.distance)
                {
                    gameObject.GetComponent<CharacterController>().height = 2f - ((hitRightBase.distance - 1) * 2);


                }
                else
                {

                    gameObject.GetComponent<CharacterController>().height = 2f - ((hitLeftBase.distance - 1) * 2);
                }

            }
            else
            {
                gameObject.GetComponent<CharacterController>().height = 2;

            }





        }
    }
}
