/// <summary>
/// Bacterium Interactive - Script for upcoming immune system game 2015
/// Programmers: Alex Gilbert, Darias Skiedra, Zhan Simeonov, Dave Elliott
/// Artists: Linda Marie Martinez, Steph Rivo
///
/// Bacterium Homepage: https://sites.google.com/site/bacteriuminteractive
/// </summary>

using UnityEngine;
using System.Collections;

public class SporelingSpawnerAI : MonoBehaviour, IHitable, IKillable
{

    #region Variables (private)

    private GameObject Player;
    private Vector3 prevLoc, Loc, nextLoc;
    private float moveSpeed;
    private RaycastHit hit;
    private Transform[] waypoints;
    private int waypointInt = 0, Health;
	private Vector3 shotPosition, lastPos, vel;
    [SerializeField]
	private float SpawnInterval = 3f;
	private float shotInterval;
	private Quaternion rotate;
    private GameObject ball;
    [SerializeField]
    private GameObject SpawnBall;
    private bool boolSpawn=true;

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
        waypoints = null;
        shotInterval = Time.time;
        moveSpeed = 3;
        waypointInt = 0;
        Health = 100;
    }

    /// <summary>
    /// Patrol
    /// </summary>
    void Patrol()
    {

    }

    /// <summary>
    /// Attack protocol
    /// </summary>
    void Attack()
    {
        ball = (GameObject)Instantiate(SpawnBall, transform.position, transform.rotation);
        Physics.IgnoreCollision(GetComponent<Collider>(), ball.GetComponent<Collider>());
        shotPosition = Player.transform.position + ((Player.transform.position - lastPos) * 18);
        if (shotPosition.magnitude > 0)
            vel = (shotPosition - ball.transform.position).normalized * 18;
        else vel = (Player.transform.position - ball.transform.position).normalized * 18;
        vel.y += 2;
        ball.GetComponent<Rigidbody>().velocity = vel;
        shotInterval = Time.time + SpawnInterval;
    }

    /// <summary>
    /// Chase the player
    /// </summary>
    void Chase()
    {

    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>	
    /// 
    IEnumerator Wait()
    {
        yield return new WaitForSeconds(25);
        boolSpawn = true;
    }
    void Update()
    {
		transform.LookAt (Player.transform.position);
	    if (Vector3.Distance (Player.transform.position, this.transform.position) < 30 && shotInterval <= Time.time && boolSpawn==true) 
		{
            GetComponent<Renderer>().material.color = Color.red;
            for (int i = 0; i<3; i++)
            {
                Attack();
            }
            boolSpawn = false;
            StartCoroutine(Wait());
		}
        else GetComponent<Renderer>().material.color = Color.white; 
	    GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
		lastPos = Player.transform.position;
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

    public void Kill()
    {
        Destroy(gameObject);
    }

    public void Hit(PlayerAttack pa)
    {
        if (Health > 20)
            Health -= 20;
        else Kill();
    }

    #endregion
}