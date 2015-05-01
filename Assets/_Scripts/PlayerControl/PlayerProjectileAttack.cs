using UnityEngine;
using System.Collections;

public class PlayerProjectileAttack : MonoBehaviour 
{
    public void Start()
    {
        Destroy(gameObject, 5);
    }
    

    public void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag != "Player")
            ActivateHittables.HitAll(collision.gameObject, gameObject.GetComponent<PlayerAttack>());
    }
}
