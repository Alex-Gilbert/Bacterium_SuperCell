using UnityEngine;
using System.Collections;

public class Burrower : MonoBehaviour {
	private GameObject Player;
	private float distance;



	// Use this for initialization
	void Start () {
		Player = GameObject.FindWithTag("Player");

	}
	public void OnCollisionEnter(Collision collision)
	{
		if (collision.gameObject.name == "Plane")
			Physics.IgnoreCollision(this.GetComponent<Collider>(),collision.gameObject.GetComponent<Collider>());	

	}
	// Update is called once per frame
	void Update () {
		distance = Vector3.Distance (Player.transform.position, transform.position);
		if ((distance < 15) && (distance > 6)) {

			Vector3 player_position = Player.transform.position;
			player_position.y = -4.0f;
			transform.LookAt(player_position);
			transform.position = Vector3.MoveTowards (transform.position, player_position, 4f * Time.deltaTime);
		} else if (distance < 10) {
			transform.LookAt(Player.transform.position);
			transform.position = Vector3.MoveTowards (transform.position, Player.transform.position, 4f * Time.deltaTime);
		}
	}
	
}
