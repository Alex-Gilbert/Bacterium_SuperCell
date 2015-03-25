using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(AudioSource))]
public class HitSound : MonoBehaviour, IHitable
{
    public AudioClip clip;

    public void Hit(PlayerAttack pa)
    {
        GetComponent<AudioSource>().PlayOneShot(clip);
    }
}
