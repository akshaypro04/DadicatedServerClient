using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public int id;
    public string userName;
    public float health;
    public float maxHealth;
    public int ItemCount = 0;
    public MeshRenderer model;


    public void Initialize(int _id, string _Name)
    {
        id = _id;
        userName = _Name;
        health = maxHealth;
    }

    public void SetHealth(float _health)
    {
        health = _health;

        if(health <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        model.enabled = false;  
    }

    public void ReSpawn()
    {
        model.enabled = true;
        SetHealth(maxHealth);
    }


}
