using UnityEngine;
using System.Collections;

public class CheckPoint : MonoBehaviour, IHitable 
{
    [SerializeField]
    GameObject waterSpray;

    SC_CharacterController player;

	// Use this for initialization
	void Start () {
        waterSpray.SetActive(false);
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<SC_CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void TurnOff()
    {
        waterSpray.SetActive(false);
    }

    public void Hit(PlayerAttack pa)
    {
        if(pa.AttackType == PlayerAttackType.Dive)
        {
            waterSpray.SetActive(true);

            player.SetCheckPoint(this);
        }
    }
}
