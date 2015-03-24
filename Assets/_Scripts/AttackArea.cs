using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class AttackArea : MonoBehaviour 
{
    List<GameObject> collidedObjects;

    public PlayerAttack attackActive;

    void Start()
    {
        collidedObjects = new List<GameObject>();
    }

    public void OnTriggerEnter(Collider other)
    {
        print(other.gameObject.name);
        if (attackActive != null)
        {
            ActivateHittables.HitAll(other.gameObject, attackActive);
            collidedObjects.Add(other.gameObject);
        }
        else
        {
            collidedObjects.Add(other.gameObject);
        }
        
    }

    public void OnTriggerExit(Collider other)
    {
        collidedObjects.Remove(other.gameObject);
    }

    public void ActivateAttack(PlayerAttack attack)
    {
        attackActive = attack;

        for(int i = 0; i < collidedObjects.Count; ++i)
        {
            ActivateHittables.HitAll(collidedObjects[i], attackActive);
        }
    }

    public void EndAttack()
    {
        attackActive = null;
    }
}
