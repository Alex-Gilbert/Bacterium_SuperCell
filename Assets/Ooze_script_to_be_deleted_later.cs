using UnityEngine;
using System.Collections;

public class Ooze_script_to_be_deleted_later : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y < 1)
			Destroy (gameObject);
	}
}
