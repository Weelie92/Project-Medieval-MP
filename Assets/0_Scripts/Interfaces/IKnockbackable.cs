using UnityEngine;

public interface IKnockbackable
{
    bool CanBeKnockedBack { get; set; }
    void Knockback(Vector3 direction, float force);
}