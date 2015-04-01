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
	
	#endregion
	
	
	#region Properties (public)
	
	#endregion
	
	
	#region Unity event functions
	
	/// <summary>
	/// Update is called once per frame.
	/// </summary>	
	void Update()
	{
		
	}
	
	/// <summary>
	/// Debugging information should be placed here.
	/// </summary>
	void OnDrawGizmos()
	{
		
	}
	
	#endregion
	
	
	#region Methods
	
	public void PatrolGround(Transform[] waypoints, ref int waypointInt)
	{
        NavMeshAgent Agent = GetComponent<NavMeshAgent>();
		if (waypoints.Length == 0)
			Debug.Log("No waypoints set!");
		else
		{
			if (Vector3.Distance(transform.position, waypoints[waypointInt].position) <= 2)
			{
				if (waypointInt < waypoints.Length)
				waypointInt++;
				else waypointInt = 0;
			}
			Agent.SetDestination(waypoints[waypointInt].position);
		}
	}

	public void PatrolAir(Transform[] waypoints, ref int waypointInt, int moveSpeed)
	{
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
