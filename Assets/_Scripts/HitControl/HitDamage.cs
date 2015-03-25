using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(Stats))]
public class HitDamage : MonoBehaviour, IHitable
{
    bool Alive = true;
    Stats stats;
    IKillable killable;

    public void Start()
    {
        stats = GetComponent<Stats>();
        killable = (IKillable)GetComponent(typeof(IKillable));
        if(killable == null)
            throw new MissingComponentException("Requires an IKillable Component");

    }

    
    public void Hit(PlayerAttack pa)
    {
        stats.Health -= pa.Damage;
        if (Alive && stats.Health <= 0)
        {
            killable.Kill();
            Alive = false;
            
            OnHasDied();
        }
    }

    public void OnHasDied()
    {
        
    }
}
