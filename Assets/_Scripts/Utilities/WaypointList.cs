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
public class WaypointList : MonoBehaviour
{
	#region Variables (private)
	
	#endregion
	
	
	#region Properties (public)
	public Transform[] waypointsListFlying;
    public Transform[] waypointsListGround;

    public GameObject GroundWaypoints;
    public GameObject Airwaypoints;

	#endregion
	
	
	#region Unity event functions
	
	/// <summary>
	/// Use this for initialization.
	/// </summary>
	void Awake()
	{
        waypointsListGround = GroundWaypoints.GetComponentsInChildren<Transform>();
        waypointsListFlying = Airwaypoints.GetComponentsInChildren<Transform>();
	}
	
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

	//Scan waypoints and select closest patrol route
	public void setWaypoints(out Transform[] waypoints, int numOfWaypoints, int type, Vector3 Position)
	{
		Transform[] waypointsTotal;

		if(type == 0)
		waypointsTotal = waypointsListGround;
		else waypointsTotal = waypointsListFlying;

		waypoints = new Transform[numOfWaypoints];
		int i, j, temp, temp2;
		int length_total = waypointsTotal.Length;
		
		if(length_total < numOfWaypoints + 1)
		{
			int length_current = 0;
			for(i = 0; i < numOfWaypoints; i++)
			{
                if (length_current > length_total - 1)
                    length_current = 0;
                waypoints[i] = waypointsTotal[length_current];
                length_current++;
			}
		}
		else
		{
            float[] distance = new float[length_total];
            int[] index = new int[length_total];

			for(i = 0; i < length_total; i++)
			{
				distance[i] = Vector3.Distance(waypointsTotal[i].position, Position);
				index[i] = i;
			}
			for(i = 0; i < numOfWaypoints; i++)
			{
				temp = index[i];
				for(j = i + 1; j < length_total; j++)
				{
					if(distance[temp] > distance[index[j]])
						temp = index[j];
				}
				temp2 = index[i];
				index[i] = index[temp];
				index[temp] = temp2;
			}
			for(i = 0; i < numOfWaypoints; i++)
				waypoints[i] = waypointsTotal[index[i]];
		}
	}

    public void AlexSetWayPoints(out Transform[] waypoints, int numOfWaypoints, Vector3 Position, float maxDistance, bool WantGround)
    {
        Transform[] proposedArray = new Transform[numOfWaypoints];

        Transform[] toCheck = WantGround ? waypointsListGround : waypointsListFlying;

        int waypointsFound = 0;
        for(int i = 0; i < toCheck.Length; ++i)
        {
            if (waypointsFound < numOfWaypoints)
            {
                if (Vector3.Distance(Position, toCheck[i].position) <= maxDistance)
                {
                    proposedArray[waypointsFound] = toCheck[i];
                    waypointsFound++;
                }
            }
            else
                break;
        }

        waypoints = new Transform[waypointsFound];

        for(int i = 0; i < waypointsFound; ++i)
        {
            waypoints[i] = proposedArray[i];
        }
    }

	#endregion
}
