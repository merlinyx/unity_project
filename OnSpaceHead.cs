using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSpaceHead : MonoBehaviour {

	public float dist;
	Camera mainCamera;
	GameObject head;
	bool down;
	Vector3 initialPos;
	float speed = 3;

	// Use this for initialization
	void Start () {
		down = false;
		mainCamera = Camera.main;
		mainCamera.enabled = true;
		head = GameObject.FindWithTag ("Head");
		if (head == null) {
			Debug.Log ("No Head in scene!");
		}
		initialPos = mainCamera.transform.position;
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.RightArrow)) {
			mainCamera.transform.Translate(new Vector3(speed * Time.deltaTime,0,0));
		} else if(Input.GetKey(KeyCode.LeftArrow)) {
			mainCamera.transform.Translate(new Vector3(-speed * Time.deltaTime,0,0));
		} else if(Input.GetKey(KeyCode.DownArrow)) {
			mainCamera.transform.Translate(new Vector3(0,-speed * Time.deltaTime,0));
		} else if(Input.GetKey(KeyCode.UpArrow)) {
			mainCamera.transform.Translate(new Vector3(0,speed * Time.deltaTime,0));
		} else if(Input.GetKey(KeyCode.Z)) {
			mainCamera.transform.Translate(new Vector3(0,0,-speed * Time.deltaTime));
		} else if(Input.GetKey(KeyCode.X)) {
			mainCamera.transform.Translate(new Vector3(0,0,speed * Time.deltaTime));
		} else if (Input.GetKeyDown (KeyCode.Space)) {
			if (down) {
				mainCamera.transform.position = initialPos;
				mainCamera.transform.LookAt (new Vector3 (initialPos.x, initialPos.y, initialPos.z-2));
			} else {
				mainCamera.transform.LookAt (head.transform);
				Vector3 dir = head.transform.position - initialPos;
				dir = dir.normalized;
				mainCamera.transform.Translate (dir * dist, Space.World);
			}
			down = !down;
		}
	}
}
