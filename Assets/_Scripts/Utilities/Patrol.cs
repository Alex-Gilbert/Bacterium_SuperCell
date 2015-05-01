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
    [SerializeField]
    private int maxDistance = 5;
    private int waypointInt = 0;
    private NavMeshAgent Agent;

    public float MoveSpeed = 3;
    public float RotateSpeed = .2f;

	#endregion
	
	
	#region Properties (public)
	
	#endregion
	
	
	#region Unity event functions

    /// <summary>
    /// Use this for initialization.
    /// </summary>
    void Start()
    {
        GameObject.Find("Waypoints").GetComponent<WaypointList>().AlexSetWayPoints(out waypoints, numOfWaypoints, transform.position, maxDistance, !flying);
        
        if(!flying)
            Agent = GetComponent<NavMeshAgent>();
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
		if(waypoints.Length == 0)
			Debug.Log ("No waypoints set!");
		else
		{
            float MoveAmount = MoveSpeed * Time.deltaTime;

            if (Vector3.Distance(transform.position, waypoints[waypointInt].position) <= MoveSpeed)
            {
                waypointInt = waypointInt == waypoints.Length - 1 ? 0 : waypointInt + 1;
                print(waypoints[waypointInt].position);
            }

            Vector3 toLookAt = waypoints[waypointInt].position - transform.position;
            Quaternion lr = Quaternion.LookRotation(toLookAt);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, lr, RotateSpeed * Time.deltaTime);

            //Quaternion referentialShift = Quaternion.FromToRotation(transform.forward, toLookAt);
            //transform.rotation = Quaternion.Lerp(transform.rotation, transform.rotation * referentialShift, RotateSpeed * Time.deltaTime);

            

            transform.position += transform.forward * MoveAmount;
            //transform.position += Vector3.up * Mathf.Lerp(0, waypoints[waypointInt].position.y - transform.position.y, MoveAmount * .1f);
		}
	}
	
	#endregion
}
