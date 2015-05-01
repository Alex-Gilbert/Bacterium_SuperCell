/// <summary>
/// Bacterium Interactive - Script for upcoming immune system game 2015
/// Programmers: Alex Gilbert, Darias Skiedra, Zhan Simeonov, Dave Elliott
/// Artists: Linda Marie Martinez, Steph Rivo
///
/// Bacterium Homepage: https://sites.google.com/site/bacteriuminteractive
/// </summary>

using UnityEngine;
using System.Collections;

public class Follow_Head : MonoBehaviour, IHitable, IKillable
{
    #region Properties (private)
    [SerializeField]
    private Transform piece;
    private Transform[] Parts;
    [SerializeField]
    private Transform Head;
    private Transform[] waypoints;
    private int numOfWaypoints = 10;
    private int waypointInt = 0;
    [SerializeField]
    private int TorsoSize = 10;
    private int AttackInterval = 0;
    private int NextAttack = 0;
    private GameObject Player;
    private Transform last;
    private Vector3 position;
    private Vector3 previous;
    private float step;
    private float speed = 18f;
    private float distance;
    private float warningInterval = 3f, warningTime = -1f;
    private float minimumEngagementTime = 3f, EngagementTime = 0;
    private bool engaged = false, warned = false, ResetNeeded = false, dead = false, Attacking = false;
    private float rotationSpeed = 5f;

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
        NextAttack = 0;
        waypoints = GameObject.Find("Ebola Waypoints").GetComponent<WaypointList>().waypointsListFlying;
        Initialize();
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>	
    /// 
    void Update()
    {
        distance = Vector3.Distance(Player.transform.position, Head.position);
        if (!Attacking && !dead)
        {
            if (distance < 30 && AttackInterval <= NextAttack)
                 engaged = true;

            if (engaged)
                StateUpdate();
            else
            {
                if (ResetNeeded)
                    Reset();
                Patrol();
            }
        }
    }

    void OnCollisionEnter(Collision collider)
    {
        /*//If Player projectile collides with enemy, Shot tag is a WIP
        if (collider.gameObject.tag == "Shot")
            Hit();*/
    }

    /// <summary>
    /// Debugging information should be placed here.
    /// </summary>
    void OnDrawGizmos()
    {

    }

    #endregion


    #region Methods

    void StateUpdate()
    {
        ResetNeeded = true;
        if (warned)
        {
            Attack();
        }
        else
        {
            if (warningTime == -1)
            {
                warningTime = Time.time + warningInterval;
            }
            else if (warningTime < Time.time)
                warned = true;
            Patrol();
        }
    }

    private void Attack()
    {
        if (distance > 2)
            Move(Player.transform.position);
        else
            Reset();
    }

    private void Patrol()
    {
        if (waypoints.Length == 0)
            Debug.Log("No waypoints set!");
        else
        {
            if (Vector3.Distance(Head.position, waypoints[waypointInt].position) <= 2)
            {
                if (waypointInt != (waypoints.Length - 1))
                    waypointInt++;
                else waypointInt = 0;
                NextAttack++;
            }
            Move(waypoints[waypointInt].position);
        }
    }

    void Reset()
    {
        warned = false;
        warningTime = -1;
        engaged = false;
        NextAttack = 0;
        AttackInterval = Random.Range(1, 5);
        ResetNeeded = false;
    }


    private void Initialize()
    {
        Parts = new Transform[TorsoSize];
        position = Head.position - Head.forward;
        for (int i = 0; i < TorsoSize; i++)
        {
            Parts[i] = (Transform)Instantiate(piece, position, Quaternion.identity);
            Parts[i].parent = this.transform;
            position = position - Parts[i].forward;
        }
    }

    private void Move(Vector3 target)
    {
        previous = Head.position;
        step = speed * Time.deltaTime;
        Head.position = Vector3.MoveTowards(Head.position, target, step);
        //Head.LookAt(position);
        Quaternion rotation = Quaternion.LookRotation(target - Head.position);
        Head.rotation = Quaternion.Slerp(Head.rotation, rotation, rotationSpeed * Time.deltaTime);
        //Head.position += Head.forward * step;
        position = Head.position;
        last = Head;
        foreach (Transform t in Parts)
        {
            position = t.position;
            t.position = previous;
            previous = position;
            t.LookAt(last);
             last = t;
         }
    }

    public void Kill()
    {
        Destroy(gameObject);
    }

    public void Hit(PlayerAttack pa)
    {
        //Hit
    }

    #endregion
}