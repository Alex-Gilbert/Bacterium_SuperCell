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
public class Shooter : MonoBehaviour, IHitable, IKillable
{
    #region Variables (private)
    private bool engaged = false, warned = false, ResetNeeded = false, dead = false, Attacking = false;
    private float distance, AttackInterval = 1.5f, NextAttack;
    private float warningInterval = 1f, warningTime = -1f;
    private float minimumEngagementTime = 6f, EngagementTime = 0;
    private float rotationSpeed = 5f;
    private float LineOfSight;
    private NavMeshAgent Agent;
    private GameObject Player;
    [SerializeField]
    private Transform LeftHand, RightHand;
    [SerializeField]
    private EnemySight Sight;
    [SerializeField]
    private GameObject Projectile;
    private GameObject LeftShot, RightShot;
    [SerializeField]
    public Animator anim;
    #endregion


    #region Properties (public)
    [HideInInspector]
    public bool left_collide = false, right_collide = false, back_collide = false;

    public Color patColor;
    public Color warnColor;
    public Color engagedColor;

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
        ColorMe(patColor);
        LineOfSight = GetComponent<EnemySight>().distance;
        anim.SetBool("Walk", true);
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
            if (distance < LineOfSight)
            {
                if (distance < (LineOfSight / 2) || Sight.Sighted())
                {
                    engaged = true;
                    EngagementTime = minimumEngagementTime + Time.time;
                }
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
        }
    }

    /// <summary>
    /// Debugging information should be placed here.
    /// </summary>
    void OnDrawGizmos(){}

    #endregion


    #region Methods

    void StateUpdate()
    {
        Quaternion rotation = Quaternion.LookRotation(Player.transform.position - transform.position);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, rotationSpeed * Time.deltaTime);
        ResetNeeded = true;
        if (warned)
        {
            ColorMe(engagedColor);
            anim.SetBool("Attack", true);
            anim.SetBool("Walk", true);
            Chase();
        }
        else
        {
            if (warningTime == -1)
            {
                ColorMe(warnColor);
                warningTime = Time.time + warningInterval;
                Agent.Stop();
                anim.SetBool("Walk", false);
            }
            else if (warningTime < Time.time)
                warned = true;
        }
    }

    /// <summary>
    /// Attack protocol
    /// </summary>
    void Attack()
    {
        LeftShot = (GameObject)Instantiate(Projectile, LeftHand.position, Quaternion.identity);
        Physics.IgnoreCollision(GetComponent<Collider>(), LeftShot.GetComponent<Collider>());
        LeftShot.GetComponent<Rigidbody>().velocity = transform.forward * 10f;
        LeftShot.AddComponent<Destroy>();
        RightShot = (GameObject)Instantiate(Projectile, RightHand.position, Quaternion.identity);
        Physics.IgnoreCollision(LeftShot.GetComponent<Collider>(), RightShot.GetComponent<Collider>());
        Physics.IgnoreCollision(GetComponent<Collider>(), RightShot.GetComponent<Collider>());
        RightShot.GetComponent<Rigidbody>().velocity = transform.forward * 10f;
        RightShot.AddComponent<Destroy>();
    }

    /// <summary>
    /// Chase the player
    /// </summary>
    void Chase()
    {
        if (Time.time >= NextAttack && distance < 10)
        {
            NextAttack = Time.time + AttackInterval;
            Attack();
        }
        else if (distance <= 6)
        {
                anim.SetBool("Walk", false);
                Agent.Stop();
        }
        else
        {
                anim.SetBool("Walk", true);
                Agent.SetDestination(Player.transform.position);
                Agent.Resume();
        }
    }

    void Reset()
    {
        anim.SetBool("Attack", false);
        anim.SetBool("Walk", true);
        warned = false;
        warningTime = -1;
        ResetNeeded = false;
        Agent.Resume();
        ColorMe(patColor);
    }

    public void Kill()
    {
        Player.GetComponent<SC_CharacterController>().ScoreUp(100);
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
            child[i].material.SetColor("_EmissionColor", col);
        }
    }
    #endregion
}

