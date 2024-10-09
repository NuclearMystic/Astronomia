using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealingArea : MonoBehaviour
{
    public DamageType healingType;
    public float healingAmount;

    private void OnTriggerEnter2D(Collider2D other) // Corrected signature
    {
        if (other.CompareTag("Player"))
        {
            if (healingType == DamageType.Stamina)
            {
                PlayerVitals.instance.RestoreStamina(healingAmount);
                Debug.Log("Player entered stamina restore area.");
            }
            else if (healingType == DamageType.Magic)
            {
                PlayerVitals.instance.ReplenishMagic(healingAmount);
                Debug.Log("Player entered magic healing area.");
            }
            else if (healingType == DamageType.Health)
            {
                Debug.Log("Player entered health healing area.");
                PlayerVitals.instance.RestoreHealth(healingAmount);
            }
        }
    }
}

