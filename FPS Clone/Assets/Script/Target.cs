using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;




public class Target : MonoBehaviour
{
    public event EventHandler OnDamaged;

    public float maxHealth = 50f;
    public float currentHealth;
    public Animator animator;

    public Image barImage;
    public Image healthBar;
    private int healthAmount;
    

    public const float DAMAGED_HEALTH_FADE_TIMER_MAX = 1f;
    public Image damagedBar;
    public Color damagedColor;
    public float damagedHealthshrinkTimer;

    void Start()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {
        

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
        damagedHealthshrinkTimer = 0.2f;
        currentHealth -= amount;

        if (currentHealth <= 0)
        {
            Die();
        }

       
    }

    void Die()
    {
       animator.SetBool("isDead", true);
       Destroy(gameObject, 5f);
    }

   
}
