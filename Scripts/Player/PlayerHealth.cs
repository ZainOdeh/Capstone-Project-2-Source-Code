using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public int currentHealth;


    private void Start() {
        // Player will have the same health he had before entering this level:
        // TODO: Replace this with your existing reference (GameManager / SaveSystem / static var etc.)
        // currentHealth = YourHealthReference.CurrentHealth;
        // For now:
        if (currentHealth <= 0) currentHealth = maxHealth;
        print(currentHealth);
    }

    public void TakeDamage(int dmg) {
        currentHealth -= Mathf.Abs(dmg);
        if (currentHealth <= 0) {
            currentHealth = 0;
            Die();
        }
        print(currentHealth);
    }

    private void Die() {
        print("Dead");
    }
}


