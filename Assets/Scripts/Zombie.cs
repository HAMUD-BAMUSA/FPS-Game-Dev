using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    public ZombieHand zombiehand;

    public int zombieDamage;

    private void Start()
    {
        zombiehand.damage = zombieDamage;
    }
}
