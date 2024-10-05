using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Random = UnityEngine.Random;

public class Target : MonoBehaviour
{
    public event EventHandler OnDamaged;

    public WeaponSwap weapon;

    public float maxHealth = 50f;
    public float currentHealth;
    public Animator animator;

    public Image barImage;
    public Image healthBar;
    public Image damagedBar;
    public Color damagedColor;

    public GameObject damageNumberPrefab;  // Drag your damage number prefab here in Unity
    public Transform damageNumberSpawnPoint;  // Optional, but a point where the damage number spawns above the enemy

    private const float DAMAGED_HEALTH_FADE_TIMER_MAX = 1f;
    private float damagedHealthshrinkTimer;

    private Canvas enemyCanvas;  // Reference to the enemy's canvas

    public GunScript gunScript;

    void Start()
    {
        currentHealth = maxHealth;

        // Find the Canvas on the enemy (should be a child of the enemy)
        enemyCanvas = GetComponentInChildren<Canvas>();
        gunScript = GameObject.Find("Rifle").GetComponent<GunScript>();
        weapon = GameObject.Find("WeaponHolder").GetComponent<WeaponSwap>();
        if (enemyCanvas == null)
        {
            Debug.LogError("Enemy Canvas not found! Make sure the Canvas is a child of the enemy object.");
        }
    }

    void Update()
    {
        // Handle the damaged health bar shrinking effect
        damagedHealthshrinkTimer -= Time.deltaTime;
        if (damagedHealthshrinkTimer <= 0)
        {
            if (healthBar.fillAmount < damagedBar.fillAmount)
            {
                float shrinkSpeed = 1f;
                damagedBar.fillAmount -= shrinkSpeed * Time.deltaTime;
            }
        }

        healthBar.fillAmount = currentHealth / maxHealth;
    }

    public void TakeDamage(float amount)
    {

        if (weapon.selectedWeapon == 0)
        {
            gunScript = GameObject.Find("Rifle").GetComponent<GunScript>();
        }
        else if (weapon.selectedWeapon == 1)
        {
            gunScript = GameObject.Find("Heavy_03").GetComponent<GunScript>();
        }else if (weapon.selectedWeapon == 2)
        {
            gunScript = GameObject.Find("Pistol_07").GetComponent<GunScript>();
        }

        damagedHealthshrinkTimer = 0.2f;
        currentHealth -= amount;
        
        // Display damage number
        ShowDamageNumber(amount);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void ShowDamageNumber(float amount)
    {
        if (enemyCanvas == null)
        {
            Debug.LogError("Enemy canvas not set!");
            return;
        }

        // Instantiate the damage number prefab under the enemy's canvas
        GameObject damageNumber = Instantiate(damageNumberPrefab, enemyCanvas.transform);

       

        
        
        
            damageNumber.transform.position = enemyCanvas.transform.position + new Vector3(0, 4.3f, 0); // Adjust as needed
        

        // Get the Text component and set the damage amount
        TextMeshProUGUI damageText = damageNumber.GetComponent<TextMeshProUGUI>(); // Use TextMeshProUGUI if you're using TextMesh Pro
        damageText.text = amount.ToString();
        if (gunScript.hs == true)
        {
            damageText.color = Color.yellow;
        }
        // Optionally destroy the damage number after a short time
        Destroy(damageNumber, 1f);  // Destroy after 1 second
    }

    void Die()
    {
        animator.SetBool("isDead", true);
        Destroy(gameObject, 5f);
    }
}
