﻿using UnityEngine;
using System.Collections;

public class ShooterProjectile : MonoBehaviour 
{

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            ActivateHittables.HitAll(collision.gameObject, gameObject.GetComponent<PlayerAttack>());
    }
}
