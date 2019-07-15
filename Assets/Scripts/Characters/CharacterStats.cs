using System;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth { get; private set; }

    public Stat damage; 
    public Stat armor;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Update()
    {
      /*  if (Input.GetKeyDown(KeyCode.T))
        {
            TakeDamage(10);//Testing purposes only
        }*/
    }

    public void TakeDamage(int damage)
    {
        damage -= armor.GetValue();
        damage = Mathf.Clamp(damage, 0, int.MaxValue);

        currentHealth -= damage;
        Debug.Log(transform.name + " takes" + damage + " damage.");

        if(currentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Die()
    {
        //meant to be overwritten
        Debug.Log(transform.name + " died.");
    }
}
