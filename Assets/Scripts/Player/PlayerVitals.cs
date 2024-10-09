using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements.Experimental;

public class PlayerVitals : MonoBehaviour
{
    public static PlayerVitals instance;

    private void Awake()
    {
        instance = this;
    }

    [Header("Sliders")]
    [Tooltip("Slider from the health bar.")]
    [SerializeField] private Slider healthSlider;
    [Tooltip("Slider from the stamina bar.")]
    [SerializeField] private Slider staminaSlider;
    [Tooltip("Slider from the magic bar.")]
    [SerializeField] private Slider magicSlider;

    // health
    private float currentHealth;
    private float maxHealth = 100;

    // stamina
    private float currentStamina;
    private float maxStamina = 100;

    // magic
    private float currentMagic;
    private float maxMagic = 100;

    void Start()
    {
        healthSlider.maxValue = maxHealth;
        staminaSlider.maxValue = maxStamina;
        magicSlider.maxValue = maxMagic;
        currentHealth = maxHealth;
        currentStamina = maxStamina;
        currentMagic = maxMagic;

        // initialize sliders with starting current health
        RefreshBarsUI();
    }

    private void Update()
    {
        if(currentHealth < 0)
        {
            currentHealth = 0;
        }
        if(currentStamina < 0)
        {
            currentStamina = 0;
        }
        if(currentMagic < 0)
        {
            currentMagic = 0;
        }
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        if(currentStamina > maxStamina)
        {
            currentStamina = maxStamina;
        }
        if(currentMagic > maxMagic)
        {
            currentMagic = maxMagic;
        }
    }

    public void RefreshBarsUI()
    {
        // refresh slider values so they match current health values
        healthSlider.value = currentHealth;
        staminaSlider.value = currentStamina;
        magicSlider.value = currentMagic;
    }

    // call this method with a damage value to damage player health
    public void DamageHealth(float damage)
    {
        currentHealth -= damage;
        RefreshBarsUI();
        Debug.Log("Player health damaged " + damage);
    }

    // call this method with a healing value to restore player health
    public void RestoreHealth(float healing)
    {
        currentHealth += healing;
        RefreshBarsUI();
        Debug.Log("Player health restored " + healing);
    }

    // call this with the amount of stamina to drain from player
    public void DrainStamina(float stamina)
    {
        StartCoroutine(DrainPlayerStamina(stamina));
        RefreshBarsUI();
        Debug.Log("Player stamina drained " + stamina);
    }

    // makes stamina drain over time
    private IEnumerator DrainPlayerStamina(float drainAmount)
    {
        float drainedStamina = drainAmount; // Track the remaining stamina to drain
        while (drainedStamina > 0)
        {
            currentStamina -= drainAmount * Time.deltaTime;
            drainedStamina -= drainAmount * Time.deltaTime;
            RefreshBarsUI();
            yield return null;
        }
    }

    // call this to restore player stamina
    public void RestoreStamina(float stamina)
    {
        StartCoroutine(RestorePlayerStamina(stamina));
        RefreshBarsUI();
        Debug.Log("Player stamina restored " + stamina);
    }

    // stamina restores over time as well
    private IEnumerator RestorePlayerStamina(float restoreAmount)
    {
        float restoredStamina = restoreAmount;
        while (restoredStamina > 0)
        {
            currentStamina += restoreAmount * Time.deltaTime;
            restoredStamina -= restoreAmount * Time.deltaTime;
            RefreshBarsUI();
            yield return null;
        }
    }

    public void UseMagic(float magic)
    {
        currentMagic -= magic;
        RefreshBarsUI();
        Debug.Log("Player used magic " + magic);
    }

    public void ReplenishMagic(float magic)
    {
        StartCoroutine(RestorePlayerMagic(magic));
        RefreshBarsUI();
        Debug.Log("Player magic replenished " + magic);
    }

    // magic restores over time instead of instantly
    private IEnumerator RestorePlayerMagic(float restoreAmount)
    {
        float restoredMagic = restoreAmount;
        while (restoredMagic > 0)
        {
            currentMagic += restoreAmount * Time.deltaTime;
            restoredMagic -= restoreAmount * Time.deltaTime;
            RefreshBarsUI();
            yield return null;
        }
    }
}
