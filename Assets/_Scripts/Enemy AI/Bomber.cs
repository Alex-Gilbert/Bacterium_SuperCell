using UnityEngine;
using System.Collections;

public class Bomber : MonoBehaviour {

	private GameObject Player;
	public GameObject Bullet;
	private Vector3 vec;
	private Vector3 start_location;
	private Vector3 set_location;
	private float startTime;


	private bool attack = true;
	private float speed = 3.0f;
	private float distance;
	private float journeyLength;
	private bool chase = true;
	private int bulletmax = 20;

// Use this for initialization
	void Start () {
		Player = GameObject.FindWithTag("Player");
	}

	IEnumerator WaitMethod() {
		yield return new WaitForSeconds(10*Time.deltaTime);
		attack = true;
	}

	void Attack (){
		var weapon = GameObject.Instantiate(Bullet, this.transform.position,Quaternion.identity) as GameObject;
		Physics.IgnoreCollision(weapon.GetComponent<Collider>(),this.GetComponent<Collider>());	
		//weapon.rigidbody.AddForce(v*Time.deltaTime*2*speed);
		var rb = weapon.GetComponent<Rigidbody>();
		rb.velocity = new Vector3 (0, -5, 0);
		StartCoroutine (WaitMethod());
	}
	// Update is called once per frame
	void Update () {
		distance = Vector3.Distance(Player.transform.position, transform.position);
		vec = gameObject.transform.position;
		if ((distance < 5) && (attack == true) ) {
			Attack ();
			attack=false;
		}
		if (distance < 10) {
			Vector3 player_position = Player.transform.position;
			player_position.y = 3.0f;
			transform.position= Vector3.MoveTowards(transform.position,player_position,3f*Time.deltaTime);
		}
	}
}