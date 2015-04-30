using UnityEngine;
using System.Collections;

public class EnemyAttackArea : MonoBehaviour 
{
    GameObject player;
    public PlayerAttack attackActive;



    public void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            if (attackActive != null)
            {
                ActivateHittables.HitAll(other.gameObject, attackActive);
            }
            else
            {
                player = other.gameObject;
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.tag.Equals("Player"))
        {
            player = null;
        }
    }

    public void ActivateAttack(PlayerAttack attack)
    {
        attackActive = attack;
        attackActive.Forward = transform.forward;

        if(player != null)
        {
            ActivateHittables.HitAll(player, attackActive);
        }
    }

    public void EndAttack()
    {
        attackActive = null;
    }
}
