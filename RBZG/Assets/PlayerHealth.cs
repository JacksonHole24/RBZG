using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    private int health = 100;
    [SerializeField] int maxHealth = 100; // Maximum health value
    [SerializeField] float regenerationDelay = 5f; // Default delay before regeneration starts
    [SerializeField] float regenerationRate = 2f; // Default rate of health regeneration
    [SerializeField] bool canRegenerate = true; // Default value for regeneration availability

    public float regenerationTimer; // Timer for regeneration delay

    void Start()
    {
        regenerationTimer = regenerationDelay; // Set the timer to the delay value
    }

    void Update()
    {
        if (canRegenerate && health < maxHealth) // If regeneration is available and health is not at maximum
        {
            if (regenerationTimer <= 0f) // If the delay timer has expired
            {
                health = (int)Mathf.Lerp(health, maxHealth, regenerationRate * Time.deltaTime);
                Debug.Log(health);
            }
            else
            {
                regenerationTimer -= Time.deltaTime; // Decrease the delay timer
            }
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Death();
        }
        else
        {
            Debug.Log(health);
            regenerationTimer = regenerationDelay; // Reset the delay timer when taking damage
        }
    }

    private void Death()
    {
        SceneManager.LoadScene("Test Scene");
    }
}
