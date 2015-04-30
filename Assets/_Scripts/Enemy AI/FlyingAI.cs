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
public class FlyingAI : MonoBehaviour, IHitable, IKillable
{
	#region Variables (private)
	private GameObject Player;
    private bool warned = false;
	private Vector3 prevLoc, Loc, nextLoc;
    [SerializeField]
	private float moveSpeed = 3, warningTime = -1, warningInterval = 3f;
	private RaycastHit hit;
	private Transform[] waypoints;
	private int waypointInt = 0, Health;
    private bool Attacked;
    [SerializeField]
    private int numOfWaypoints = 3;
	
	#endregion
	
	
	#region Properties (public)
	
	#endregion
	
	
	#region Unity event functions
	
	/// <summary>
	/// Use this for initialization.
	/// </summary>
	void Start()
	{
		Player = GameObject.FindWithTag("Player");
        GameObject.Find("Waypoints").GetComponent<WaypointList>().AlexSetWayPoints(out waypoints, numOfWaypoints, transform.position, 15, false);
		waypointInt = 0;
        Health = 100;
        Attacked = false;
	}

	/// <summary>
	/// Patrol
	/// </summary>
	void Patrol()
	{
		if(waypoints.Length == 0)
			Debug.Log ("No waypoints set!");
		else
		{
            if (Attacked)
                if (Vector3.Distance(Player.transform.position, transform.position) > 8)
                    Attacked = false;
			if(transform.position != waypoints[waypointInt].position)
			transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointInt].position, moveSpeed * Time.deltaTime);
			else
			{
				if(waypointInt != (waypoints.Length - 1))
				waypointInt++;
				else waypointInt = 0;
                transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointInt].position, moveSpeed * Time.deltaTime);
			}
		}
	}

	/// <summary>
	/// Attack protocol
	/// </summary>
	void Attack()
	{
        transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, moveSpeed * 4 * Time.deltaTime);
	}

	/// <summary>
	/// Chase the player
	/// </summary>
	void Chase()
	{
        if (Vector3.Distance(Player.transform.position, transform.position) > 4)
        {
            transform.position = Vector3.MoveTowards(transform.position, Player.transform.position, moveSpeed * 2 * Time.deltaTime);
        }
        else if (!Attacked) Attack();
        else
        {
            GetComponent<Renderer>().material.color = Color.white;
            warned = false;
            warningTime = -1;
            Patrol();
        }
	}
	
	/// <summary>
	/// Update is called once per frame.
	/// </summary>	
	void Update()
	{
        if (Attacked || Vector3.Distance(Player.transform.position, transform.position) > 12)
        {
            GetComponent<Renderer>().material.color = Color.white;
            warned = false;
            warningTime = -1;
            Patrol();
        }
        else if (warned)
        {
            GetComponent<Renderer>().material.color = Color.red;
            Chase();
        }
        else
        {
            if (warningTime == -1)
            {
                GetComponent<Renderer>().material.color = Color.yellow;
                warningTime = Time.time + warningInterval;
            }
            else if (warningTime < Time.time)
                warned = true;
        }
	}

    void OnCollisionEnter(Collision collider)
    {
        //Player collision incase player interacts with enemy before it 
        if (collider.gameObject.tag == "Player")
        {
            Attacked = true;
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

    public void Kill()
    {
        Destroy(gameObject);
    }

	public void Hit(PlayerAttack pa)
    {
        print("Im flying and you got me");
    }

	#endregion
}
