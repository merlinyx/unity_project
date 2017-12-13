using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour {

	Camera mainCamera;
	Vector3 initialPos;
	float speed = 3;

	// Use this for initialization
	void Start () {
		mainCamera = Camera.main;
		mainCamera.enabled = true;
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
		} 
	}
}
