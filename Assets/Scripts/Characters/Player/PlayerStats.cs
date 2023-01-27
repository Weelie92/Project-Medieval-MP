using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class PlayerStats : MonoBehaviour
{
    // Stats
    [Header("Stats")]
    public float PlayerHealth = 10f;
    public float PlayerMaxHealth = 10f;
    public float PlayerStamina = 10f;
    public float PlayerMaxStamina = 10f;
    public float PlayerWalkSpeed = .5f;
    public float PlayerJumpForce = 5f;
    public float PlayerRunSpeed = 2f;
    public int PlayerInventorySpace = 30;

    // Variables
    [Header("Variables")]
    [HideInInspector] public float NextAttackTime;
    [HideInInspector] public bool PlayerMoving = false;
    [HideInInspector] public bool PlayerRunning = false;
    [HideInInspector] public bool PlayerGrounded = true;
    [HideInInspector] public bool PlayerOffhanding = false;
    [HideInInspector] public bool PlayerAttacking = false;
    [HideInInspector] public float PlayerStaminaRegen;

    // Components
    [Header("Components")]
    public string PlayerWeapon = "Unarmed";
}
