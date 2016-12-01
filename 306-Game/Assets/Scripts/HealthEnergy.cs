using UnityEngine;
using System.Collections;

public class HealthEnergy : MonoBehaviour {

    public Stat health;

    public Stat energy;

    Animator anim;
    public bool isDead;
    bool damaged;

    void Awake()
    {
        health.Initialize();
        energy.Initialize();
        anim = GetComponent<Animator>();
    }

    //Reduces the character's health by "amount" and checks if the character has died, if they have it calls the death function
    public void TakeDamage(float amount)
    {
        health.CurrentVal -= amount;
        if (health.CurrentVal <= 0 && !isDead)
        {
            Death();
        }
    }

    //Increases the character's health by "amount"
    public void RecoverHealth(float amount)
    {
        health.CurrentVal += amount;
    }

    //Sets the players state to isDead and activates the Death animation
    private void Death()
    {
        isDead = true;

        anim.SetTrigger("Dead");
    }

    //increases the character's energy by "amount"
    public void RecoverEnergy(float amount)
    {
        energy.CurrentVal += amount;
    }

    //Decreases the character's energy by "amount"
    public void LoseEnergy(float amount)
    {
        energy.CurrentVal -= amount;
    }
}
