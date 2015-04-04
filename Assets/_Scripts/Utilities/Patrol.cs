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
public class Patrol : MonoBehaviour
{
	#region Variables (private)
    [SerializeField]
    private bool flying = false;
    private Transform[] waypoints;
    [SerializeField]
    private int numOfWaypoints = 5;
    private int waypointInt = 0;
    private NavMeshAgent Agent;
	
	#endregion
	
	
	#region Properties (public)
	
	#endregion
	
	
	#region Unity event functions

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        if(flying)
        {
            GameObject.Find("Waypoints").GetComponent<WaypointList>().setWaypoints(out waypoints, numOfWaypoints, 1, transform.position);
        }
        else
        {
            GameObject.Find("Waypoints").GetComponent<WaypointList>().setWaypoints(out waypoints, numOfWaypoints, 0, transform.position);
            Agent = GetComponent<NavMeshAgent>();
        }
    }

    #endregion

    #region Methods

    public void Patrolling()
    {
        if (flying)
            PatrolAir();
        else PatrolGround();
    }
	
	public void PatrolGround()
	{
		if (waypoints.Length == 0)
			Debug.Log("No waypoints set!");
		else
		{
			if (Vector3.Distance(transform.position, waypoints[waypointInt].position) <= 2)
			{
				if (waypointInt < (waypoints.Length - 1))
				waypointInt++;
				else waypointInt = 0;
			}
			Agent.SetDestination(waypoints[waypointInt].position);
		}
	}

	public void PatrolAir()
	{
        float moveSpeed = 3;
		if(waypoints.Length == 0)
			Debug.Log ("No waypoints set!");
		else
		{
			if(transform.position == waypoints[waypointInt].position)
			{
				if(waypointInt != (waypoints.Length - 1))
					waypointInt++;
				else waypointInt = 0;
				transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointInt].position, moveSpeed * Time.deltaTime);
			}
			transform.position = Vector3.MoveTowards(transform.position, waypoints[waypointInt].position, moveSpeed * Time.deltaTime);
		}
	}
	
	#endregion
}
