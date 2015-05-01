using UnityEngine;
using System.Collections;

public enum PlayerAttackType
{
    Melee,
    Ranged,
    Heavy,
    Dive
}

public class PlayerAttack : MonoBehaviour
{
    [SerializeField]
    PlayerAttackType attackType;

    [SerializeField]
    int damage = 10;

    public int Damage { get { return damage; } }

    public PlayerAttackType AttackType { get { return attackType; } }

    public Vector3 Forward { get; set; }
}
