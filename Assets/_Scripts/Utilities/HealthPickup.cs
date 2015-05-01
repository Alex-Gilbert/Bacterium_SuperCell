using UnityEngine;
using System.Collections;

public class HealthPickup : MonoBehaviour 
{
    SC_CharacterController player;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<SC_CharacterController>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            if(player.HealUP())
            {
                Destroy(gameObject);
            }
        }
    }


}
