using UnityEngine;
using System.Collections;

public class Bomber : MonoBehaviour {
	Vector3 player_position;
	private GameObject Player;
	private int status;
	Vector3 target_position;
	private bool position_picked = false;
	float distance_to_destination;
	public GameObject Bullet;
	private Vector3 vec;
	private bool attack = true;
	private float speed = 3.0f;
	private float distance;
	private bool chase = true;
	private bool restart = true;



// Use this for initialization
	void Start () {
		Player = GameObject.FindWithTag("Player");
	}

	IEnumerator WaitMethod() {
		yield return new WaitForSeconds(10*Time.deltaTime);
		chase = true;

	}
	IEnumerator SecondWait() {
		yield return new WaitForSeconds(200*Time.deltaTime);
		position_picked=false;
	}
	void Attack (){
		var weapon = GameObject.Instantiate(Bullet, this.transform.position,Quaternion.identity) as GameObject;
		Physics.IgnoreCollision(weapon.GetComponent<Collider>(),this.GetComponent<Collider>());	
		var rb = weapon.GetComponent<Rigidbody>();
		rb.velocity = new Vector3 (0, -5, 0);
		StartCoroutine (WaitMethod());
	}
	void turbo_chase(){
		transform.position = Vector3.MoveTowards (transform.position, target_position, 10f * Time.deltaTime);
	}

	Vector3 pick_position(){
		Vector3 tempVec = new Vector3 (Random.Range (player_position.x - 2f, player_position.x + 2f), 3f,
		                              Random.Range (player_position.z - 2f, player_position.z + 2f));
		return  tempVec;
	}


	int getStatus( float distance) {
		if ((distance < 30) && (distance > 10))
			return 1;
		else if ( (distance < 10) && ( distance > 1))
			return 2;
		else
			return 0;
	}
	// Update is called once per frame
	void Update () {
		distance = Vector3.Distance (Player.transform.position, transform.position);
		distance_to_destination= Vector3.Distance ( target_position, transform.position);
		status = getStatus (distance);
		player_position = Player.transform.position;
		player_position.y = 3.0f;
	
		if (position_picked == false) {
			position_picked = true;
			target_position = pick_position ();
			StartCoroutine(SecondWait());
		}

		if (status == 1) {
			turbo_chase ();
		}
		else if (status == 2)  {
			if (chase == true) {
				chase = false;
				Attack ();
			}
			turbo_chase ();
		} 
	}
}