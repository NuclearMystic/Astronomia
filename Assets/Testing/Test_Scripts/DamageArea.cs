using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageArea : MonoBehaviour
{
    public DamageType damageType;
    public float damageAmount;

    private void OnTriggerEnter2D(Collider2D other) // Corrected signature
    {
        if (other.CompareTag("Player"))
        {
            if (damageType == DamageType.Stamina)
            {
                PlayerVitals.instance.DrainStamina(damageAmount);
                Debug.Log("Player entered stamina drain area.");
            }
            else if (damageType == DamageType.Magic)
            {
                PlayerVitals.instance.UseMagic(damageAmount);
                Debug.Log("Player entered magic damage area.");
            }
            else if (damageType == DamageType.Health)
            {
                Debug.Log("Player entered health damage area.");
                PlayerVitals.instance.DamageHealth(damageAmount);
            }
        }
    }
}

public enum DamageType
{
    Health,
    Stamina,
    Magic
}
