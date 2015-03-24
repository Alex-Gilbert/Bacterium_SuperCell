using UnityEngine;
using System.Collections;

public class PlayerProjectileAttack : MonoBehaviour 
{
    public void OnCollisionEnter(Collision collision)
    {
        ActivateHittables.HitAll(collision.gameObject, gameObject.GetComponent<PlayerAttack>());
    }
}
