using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClick : MonoBehaviour {

	private bool chosen;
	Camera mainCamera;

	// Use this for initialization
	void Start () {
		chosen = false;
		mainCamera = Camera.main;
		mainCamera.enabled = true;
	}
	
	// Update is called once per frame
	void OnMouseDown () {
		if (chosen) {
			Debug.Log ("You deselected this word: " + gameObject.transform.name);
			Vector3 newp = gameObject.transform.position;
			newp.z -= 1;
			gameObject.transform.position = newp;
			mainCamera.transform.LookAt (new Vector3 (-2, 2.8f, 10));
		} else {
			Debug.Log ("You selected this word: " + gameObject.transform.name);
			Vector3 newp = gameObject.transform.position;
			newp.z += 1;
			gameObject.transform.position = newp;
			mainCamera.transform.LookAt (gameObject.transform);
		}
		chosen = !chosen;
	}
}
