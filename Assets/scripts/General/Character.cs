using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour
{
    [Header("基本属性")]
    public float maxHealth;
    public float currentHealth;

    [Header("受伤无敌")]
    public float invulnerableDuration;
    private float invulnerableCounter;

    public bool invulnerable;

    public UnityEvent<Transform> OnTakeDamage;

    public UnityEvent OnDie;


    private void Start()
    {
        currentHealth = maxHealth; 
    }
    public void TakeDamage(Attack attacker)
    {
        if (invulnerable)
            return;

        //Debug.Log(attacker.damage);
        if (currentHealth - attacker.damage > 0){
            currentHealth -= attacker.damage;
            TriggerInvulnerable();

            //执行受伤
            OnTakeDamage?.Invoke(attacker.transform);

        }
        else {
            currentHealth = 0;
            //触发死亡
            OnDie?.Invoke();
        }


    }


    private void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if(invulnerableCounter <= 0 )
            {
                invulnerable = false;
            }

        }
    }

    private void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;

        }
    }
}
