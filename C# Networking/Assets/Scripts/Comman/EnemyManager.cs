using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{

    public int id;
    public float health;
    public float maxHealth = 100;

    public void Initialize(int _id)
    {
        id = _id;
        health = maxHealth;
    }

    public void SetHealth(float _Health)
    {
        health = _Health;

        if(health < 0)
        {
            GameManager.enemies.Remove(id);
            Destroy(gameObject);
        }

    }


}
