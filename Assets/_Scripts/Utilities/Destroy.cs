using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {

	double life;
	// Use this for initialization
	void Start () {
		life = Time.time + 10.0f;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time >= life)
			Destroy(gameObject);
	}

    void OnCollisionEnter(Collision collider)
    {
        Debug.Log(collider.gameObject.tag);
        Destroy(gameObject);
    }
}
