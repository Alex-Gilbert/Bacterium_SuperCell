using UnityEngine;
using System.Collections;

public class Burrower : MonoBehaviour {
	private GameObject Player;
	private float distance;
	private Vector3 player_position;
	private bool visible = true;
	public Component leg1;
	public Component leg2;
	public Component leg3;
	public Component body;
	public Component head;
	bool chasing = false;


	// Use this for initialization
	void Start () {
		Player = GameObject.FindWithTag("Player");
		Going_underground ();
	}

	IEnumerator WaitMethod() {
		yield return new WaitForSeconds(10*Time.deltaTime);
		chasing = true;
	
	}

	void Going_underground () {
		leg1.GetComponent<Renderer> ().enabled = false;
		leg2.GetComponent<Renderer> ().enabled = false;
		leg3.GetComponent<Renderer> ().enabled = false;
		body.GetComponent<Renderer> ().enabled = false;
		head.GetComponent<Renderer> ().enabled = false;
	}

	void Going_to_player(){
		leg1.GetComponent<Renderer> ().enabled = true;
		leg2.GetComponent<Renderer> ().enabled = true;
		leg3.GetComponent<Renderer> ().enabled = true;
		body.GetComponent<Renderer> ().enabled = true;
		head.GetComponent<Renderer> ().enabled = true;
	}

	// Update is called once per frame
	void Update () {

		distance = Vector3.Distance (Player.transform.position, transform.position);
		player_position = Player.transform.position;
		var step = 4f * Time.deltaTime;

		if ((distance < 15) && (distance > 3) && ( chasing == true) ) {
			if (visible ==true ) {
				Going_underground ();
				visible=false;
			}
			gameObject.transform.LookAt(player_position);
			gameObject.transform.position=Vector3.MoveTowards(transform.position, player_position,step);

		}
		else if ( (distance<3) && (distance > 1) && (chasing == true) ){
			if ( visible ==false)
				Going_to_player ();
			chasing=false;
			StartCoroutine (WaitMethod());
			gameObject.transform.LookAt(player_position);
			gameObject.transform.position=Vector3.MoveTowards(transform.position, player_position,step);
		}	
	}
}
