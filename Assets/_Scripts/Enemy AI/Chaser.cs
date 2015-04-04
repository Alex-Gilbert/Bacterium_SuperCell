﻿/// <summary>
/// Bacterium Interactive - Script for upcoming immune system game 2015
/// Programmers: Alex Gilbert, Darias Skiedra, Zhan Simeonov, Dave Elliott
/// Artists: Linda Marie Martinez, Steph Rivo
///
/// Bacterium Homepage: https://sites.google.com/site/bacteriuminteractive
/// </summary>

using UnityEngine;
using System.Collections;

/// <summary>
/// #DESCRIPTION OF CLASS#
/// </summary>
public class Chaser : MonoBehaviour, IHitable, IKillable
{
	#region Variables (private)
	private bool engaged = false, warned = false, ResetNeeded = false, playerAttacking = false, dead = false, Attacking = false;
    private Vector3 vec;
	private RaycastHit hit;
    [SerializeField]
	private int dodgeChance = 100;
	private float distance, AttackInterval = 1f, NextAttack, ComboInterval = 2f;
	private float warningInterval = 3f, warningTime = -1f; 
	private float minimumEngagementTime = 3f, EngagementTime = 0;
	private NavMeshAgent Agent;
    private GameObject Player;
    [SerializeField]
	public Animator anim;
	#endregion
	
	
	#region Properties (public)
    [HideInInspector]
    public bool left_collide = false, right_collide = false, back_collide = false; 
	
	#endregion
	
	
	#region Unity event functions
	
	/// <summary>
	/// Use this for initialization.
	/// </summary>
	void Start()
	{
		Agent = GetComponent<NavMeshAgent>();
		Player = GameObject.FindWithTag("Player");
		NextAttack = 0;
		ColorMe(Color.white);
		//anim = GetComponentInChildren<Animator>();
	}

	/// <summary>
	/// Attack protocol
	/// </summary>
	void Attack()
	{
        anim.SetBool("Walk", false);
        anim.SetBool("Attack", true);
        Attacking = true;
        Agent.Stop();
        StartCoroutine(Timer());
		//Attacks
	}
	
	/// <summary>
	/// Chase the player
	/// </summary>
	void Chase()
	{
        if (Time.time >= NextAttack && distance < 2)
        {
            NextAttack = Time.time + AttackInterval;
            Attack();
        }
        else Agent.SetDestination(Player.transform.position);
	}
	
	/// <summary>
	/// Update is called once per frame.
	/// </summary>	
	void Update()
	{
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
		distance = Vector3.Distance(Player.transform.position, transform.position);
        if (!Attacking && !dead)
        {
            if (distance < 12)
            {
                engaged = true;
                EngagementTime = minimumEngagementTime + Time.time;
                playerAttacking = Input.GetButton("FireRanged") || Input.GetButton("LightAttack") || Input.GetButton("HeavyAttack");
                if (playerAttacking)
                    Dodge();
            }
            else engaged = false;

            if (engaged || EngagementTime > Time.time)
                StateUpdate();
            else
            {
                if (ResetNeeded)
                    Reset();
                GetComponent<Patrol>().Patrolling();
            }
        }
	}
	
	void OnCollisionEnter(Collision collider)
	{
		//Player collision incase player interacts with enemy before it 
		if (collider.gameObject.tag == "Shot")
		{
			engaged = true;
			EngagementTime = minimumEngagementTime + Time.time;
			Agent.SetDestination(Player.transform.position);
			//Debug.Log("Collision!");
		}
	}
	
	/// <summary>
	/// Debugging information should be placed here.
	/// </summary>
	void OnDrawGizmos()
	{
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(vec, 1);
	}
	
	#endregion
	
	
	#region Methods

	void StateUpdate()
	{
		ResetNeeded = true;
		transform.LookAt(Player.transform.position);
		if (warned)
		{
			Agent.Resume();
			ColorMe(Color.red);
            anim.SetBool("Walk", true);
			Chase();
		}
		else
		{
			if (warningTime == -1)
			{
				ColorMe(Color.yellow);
				warningTime = Time.time + warningInterval;
				Agent.Stop();
                anim.SetBool("Walk", false);
			}
            else if (warningTime < Time.time)
                warned = true;
		}
	}

	void Reset()
	{
		warned = false;
		warningTime = -1;
		ResetNeeded = false;
		Agent.Resume();
        anim.SetBool("Walk", true);
		ColorMe(Color.white);
	}

	void Dodge()
	{
		int rangeValue = Random.Range (0, 100);
		if(rangeValue <= dodgeChance)
		{  
            //float angle = Quaternion.FromToRotation(transform.position, GameObject.FindWithTag("MainCamera").transform.position).eulerAngles.y;
            if(!left_collide)
				transform.position = transform.right * -0.5f + transform.position;
            else if(!right_collide)
				transform.position = transform.right * -0.5f + transform.position;
            else if(!back_collide)
				transform.position = transform.forward * -0.5f + transform.position;
		}
	}

	public void Kill()
	{
        anim.SetBool("Death", true);
        dead = true;
        Agent.Stop();
		Destroy(gameObject, 2f);
	}
	
	public void Hit(PlayerAttack pa)
	{
		print("You got me!");
	}
	
	void ColorMe(Color col)
	{
		Renderer[] child = GetComponentsInChildren<Renderer>();
		for (int i = 0; i < child.Length; i++)
		{
			child[i].material.color = col;
		}
	}
	
    IEnumerator Timer()
    {
        yield return new WaitForSeconds(1f);
        anim.SetBool("Attack", false);
        Attacking = false;
        Agent.Resume();
    }

	#endregion
}
