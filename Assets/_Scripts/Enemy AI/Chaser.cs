/// <summary>
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
	private bool engaged = false, warned = false, ResetNeeded = false;
	private Vector3 prevLoc, Loc, nextLoc;
	private RaycastHit hit;
    private Transform[] waypoints;
    private int waypointInt = 0;
	[SerializeField]
	private int numOfWaypoints = 5;
	private float distance, AttackInterval = .666f, NextAttack, ComboInterval = 2f;
	private float warningInterval = 3f, warningTime = -1f; 
	private float minimumEngagementTime = 3f, EngagementTime = 0;
	private NavMeshAgent Agent;
	private GameObject Player, Shot;
	public Animator anim;
	#endregion
	
	
	#region Properties (public)
	
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
        GameObject.Find("Waypoints").GetComponent<WaypointList>().setWaypoints(out waypoints, numOfWaypoints, 0, transform.position);
		//anim = GetComponentInChildren<Animator>();
	}

	/// <summary>
	/// Attack protocol
	/// </summary>
	void Attack()
	{
		//Attacks
	}
	
	/// <summary>
	/// Chase the player
	/// </summary>
	void Chase()
	{
		distance = Vector3.Distance(Player.transform.position, transform.position);
		if (Time.time >= NextAttack)
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
		distance = Vector3.Distance(Player.transform.position, transform.position);
        if (distance < 12)
        {
            engaged = true;
            EngagementTime = minimumEngagementTime + Time.time;
        }
        else engaged = false;

		if(engaged || EngagementTime > Time.time)
			StateUpdate();
		else if(ResetNeeded) Reset();
	}
	
	void OnCollisionEnter(Collision collider)
	{
		//Player collision incase player interacts with enemy before it 
		if (collider.gameObject.tag == "Shot")
		{
			engaged = true;
			Agent.SetDestination(Player.transform.position);
			//Debug.Log("Collision!");
		}
	}
	
	/// <summary>
	/// Debugging information should be placed here.
	/// </summary>
	void OnDrawGizmos()
	{
		
	}
	
	#endregion
	
	
	#region Methods

	public void StateUpdate()
	{
		ResetNeeded = true;
		transform.LookAt(Player.transform.position);
		//Physics.Raycast(transform.position, Player.transform.position, out hit, 100);
		if (warned)
		{
			Agent.Resume();
			ColorMe(Color.red);
			Chase();
		}
		else
		{
			if (warningTime == -1)
			{
				ColorMe(Color.yellow);
				warningTime = Time.time + warningInterval;
				Agent.Stop();
			}
			else if (warningTime < Time.time)
			{
				Agent.Resume();
				warned = true;
			}
			Agent.Stop();
		}
	}

	public void Reset()
	{
		warned = false;
		warningTime = -1;
		ResetNeeded = false;
		Agent.Resume ();
		ColorMe(Color.white);
		GetComponent<Patrol>().PatrolGround(waypoints, ref waypointInt);
	}

	public void Kill()
	{
		Destroy(gameObject, 2f);
	}
	
	public void Hit(PlayerAttack pa)
	{
		print("You got me!");
	}
	
	public void ColorMe(Color col)
	{
		Renderer[] child = GetComponentsInChildren<Renderer>();
		for (int i = 0; i < child.Length; i++)
		{
			child[i].material.color = col;
		}
	}
	
	#endregion
}
