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
public class NewSporelingAI : MonoBehaviour, IHitable, IKillable
{
    #region Variables (private)
    private bool engaged = false, warned = false;
    private Vector3 prevLoc, Loc, nextLoc;
    private RaycastHit hit;
    private Transform[] waypoints;
    private int waypointInt = 0, projectile_hits;
    [SerializeField]
    private int numOfWaypoints = 5;
    [SerializeField]
    private int NumOfRangedHits = 3;
    private float distance, AttackInterval = .666f, NextAttack, NextEngagement, ComboInterval = 2f, warningInterval = 3f, warningTime = -1;
    private NavMeshAgent Sporeling;
    private GameObject Player, Shot;
    [SerializeField]
    private GameObject Projectile;
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
        Sporeling = GetComponent<NavMeshAgent>();
        Player = GameObject.FindWithTag("Player");
        GameObject.Find("Waypoints").GetComponent<WaypointList>().AlexSetWayPoints(out waypoints, numOfWaypoints, 0, transform.position, 15, true);
        waypointInt = -1;
        projectile_hits = NumOfRangedHits;
        NextAttack = 0;
        NextEngagement = 0;
        //anim = GetComponentInChildren<Animator>();
    }

    /// <summary>
    /// Patrol
    /// </summary>
    void Patrol()
    {

        if (waypoints.Length == 0)
            Debug.Log("No waypoints set!");
        else
        {
            if (waypointInt == -1 || Vector3.Distance(transform.position, waypoints[waypointInt].position) <= 2)
            {
                if (waypointInt < (waypoints.Length - 1))
                {
                    waypointInt++;
                    print(waypoints[waypointInt].position);
                }
                else
                {
                    waypointInt = 0;
                    print(waypoints[waypointInt].position);
                }
            }
            Sporeling.SetDestination(waypoints[waypointInt].position);
        }
    }

    /// <summary>
    /// Attack protocol
    /// </summary>
    void Attack()
    {
        //Debug.Log("Attack");
        if (projectile_hits > 0 && distance > 2)
        {
            anim.SetBool("Melee", false);
            anim.SetBool("Walk", false);
            anim.SetBool("Spit", true);
            projectile_hits--;
            Shot = (GameObject)Instantiate(Projectile, transform.position + Vector3.forward, Quaternion.identity);
            Physics.IgnoreCollision(GetComponent<Collider>(), Shot.GetComponent<Collider>());
            //Shot.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.Continuous;
            //Shot.GetComponent<Rigidbody>().useGravity = false;
            Shot.transform.LookAt(Player.transform.position + Vector3.forward);
            Shot.GetComponent<Rigidbody>().velocity = Shot.transform.forward;//(Shot.transform.position - Player.transform.position).normalized * Time.deltaTime * 18f;
            //Shot.GetComponent<Rigidbody>().velocity.Set(Shot.GetComponent<Rigidbody>().velocity.x, 0,Shot.GetComponent<Rigidbody>().velocity.z);
            Shot.GetComponent<Rigidbody>().velocity *= 10f;

            Shot.AddComponent<Destroy>();
        }
        else if (distance < 2)
        {
            anim.SetBool("Walk", false);
            anim.SetBool("Spit", false);
            anim.SetBool("Melee", true);
            projectile_hits = NumOfRangedHits;
            Sporeling.SetDestination(prevLoc);
            NextEngagement = Time.time + ComboInterval;
        }
        else
        {
            anim.SetBool("Melee", false);
            anim.SetBool("Spit", false);
            anim.SetBool("Walk", true);
            Sporeling.SetDestination(Player.transform.position);
        }
    }

    /// <summary>
    /// Chase the player
    /// </summary>
    void Chase()
    {

        distance = Vector3.Distance(Player.transform.position, transform.position);
        if (distance <= 9 && Time.time > NextAttack)
        {
            NextAttack = Time.time + AttackInterval;
            engaged = true;
            if (Time.time > NextEngagement)
                Attack();
        }
        else if (distance < 12)
        {
            Sporeling.SetDestination(new Vector3(Player.transform.position.x, this.transform.position.y, Player.transform.position.z));
            engaged = true;
            anim.SetBool("Walk", true);
        }
        else
        {
            anim.SetBool("Walk", true);
            engaged = false;
            ColorMe(Color.white);
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
        if (!anim.GetBool("Death"))
        {
            if (engaged == true || Vector3.Distance(Player.transform.position, transform.position) < 12)
            {
                transform.LookAt(Player.transform.position);
                Physics.Raycast(transform.position, Player.transform.position, out hit, 100);
                //if(hit.collider.gameObject.tag != "Untagged")
                //Debug.Log(hit.collider.gameObject.tag);
                //if (hit.collider.gameObject.tag == "Player")
                if (warned)
                {
                    Sporeling.Resume();
                    ColorMe(Color.red);
                    Chase();
                }
                else
                {
                    if (warningTime == -1)
                    {
                        ColorMe(Color.yellow);
                        warningTime = Time.time + warningInterval;
                        //Sporeling.Stop();
                    }
                    else if (warningTime < Time.time)
                    {
                        //Sporeling.Resume();
                        warned = true;
                    }
                    Sporeling.Stop();
                    anim.SetBool("Melee", false);
                    anim.SetBool("Spit", false);
                    anim.SetBool("Walk", false);
                }
            }
            else
            {
                Sporeling.Resume();
                anim.SetBool("Melee", false);
                anim.SetBool("Spit", false);
                anim.SetBool("Walk", true);
                ColorMe(Color.white);
                warned = false;
                warningTime = -1;
                Patrol();
            }
        }
    }

    void OnCollisionEnter(Collision collider)
    {
        //Player collision incase player interacts with enemy before it 
        if (collider.gameObject.tag == "Player")
        {
            engaged = true;
            Sporeling.SetDestination(Player.transform.position);
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
    public void Kill()
    {
        if (!anim.GetBool("Death"))
        {
            anim.SetBool("Death", true);
            Destroy(gameObject, 2f);
        }
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
