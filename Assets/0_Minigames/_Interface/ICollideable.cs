using UnityEngine;

public interface ICollideable
{
    void OnTriggerEnter(Collider other);
}