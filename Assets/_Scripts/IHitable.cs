using UnityEngine;
using System.Collections;

public interface IHitable
{
    void Hit(PlayerAttack pa);
}
