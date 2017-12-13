using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSpace : MonoBehaviour {

	public float dist;
	Camera mainCamera;
	GameObject ivy;
	bool down;
	Vector3 initialPos;

	// Use this for initialization
	void Start () {
		down = false;
		mainCamera = Camera.main;
		mainCamera.enabled = true;
		ivy = GameObject.FindWithTag ("Ivy");
		if (ivy == null) {
			Debug.Log ("No RT Ivy in scene!");
		}
		initialPos = mainCamera.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			if (down) {
				mainCamera.transform.position = initialPos;
				mainCamera.transform.LookAt (new Vector3 (2, 3.5f, 10));
			} else {
				mainCamera.transform.LookAt (ivy.transform);
				Vector3 dir = ivy.transform.position - initialPos;
				dir = dir.normalized;
				mainCamera.transform.Translate (dir * dist, Space.World);
			}
			down = !down;
		}
	}
}
