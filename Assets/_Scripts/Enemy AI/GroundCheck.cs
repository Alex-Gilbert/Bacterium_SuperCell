using UnityEngine;
using System.Collections;

public class GroundCheck : MonoBehaviour 
{
    public bool PlayerAbove = false;

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            PlayerAbove = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            PlayerAbove = false;
        }
    }

}
