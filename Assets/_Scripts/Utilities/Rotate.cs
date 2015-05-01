using UnityEngine;
using System.Collections;

public class Rotate : MonoBehaviour 
{
    public float RotateSpeed = 10f;

	// Update is called once per frame
	void Update () 
    {
        this.transform.Rotate(transform.up, RotateSpeed * Time.deltaTime);
	}
}
