using UnityEngine;
using System.Collections;

public class ActivateHittables
{
    public static void HitAll(GameObject hitObject, PlayerAttack attack)
    {

        if (hitObject != null)
        {
            var hitables = hitObject.GetComponents(typeof(IHitable));

            if (hitables == null)
                return;

            foreach (IHitable hitable in hitables)
            {
                hitable.Hit(attack);
            }
        }
    }
}
