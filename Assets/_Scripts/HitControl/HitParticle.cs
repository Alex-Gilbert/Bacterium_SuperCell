using UnityEngine;
using System.Collections;

public class HitParticle : MonoBehaviour, IHitable
{
    public ParticleSystem particleSystem;

    public void Hit(PlayerAttack pa)
    {
        particleSystem.Play();
    }
}
