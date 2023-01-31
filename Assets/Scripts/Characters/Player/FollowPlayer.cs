using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private Transform _player;
    [SerializeField] private Vector3 _offset;

    // Update is called once per frame
    void Update()
    {

        transform.position = _player.position + _offset;
    }
}
