using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public float health = 50f;
    public Animator animator;

    public void TakeDamage(float amount)
    {

       health -= amount;

        if (health <= 0)
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
