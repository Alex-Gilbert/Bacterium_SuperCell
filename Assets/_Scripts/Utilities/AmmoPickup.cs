using UnityEngine;
using System.Collections;

public class AmmoPickup : MonoBehaviour 
{
    SC_CharacterController player;
    public float AmmoAmount = 5;

    public void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<SC_CharacterController>();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player.AmmoUp(AmmoAmount);   
            Destroy(gameObject);
        }
    }
}
