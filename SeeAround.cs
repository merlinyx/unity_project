using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeAround : MonoBehaviour {

	public GameObject preset;
	private float startTime;
	Camera mainCamera;

	// Use this for initialization
	void Start () {
		startTime = Time.time;
		mainCamera = Camera.main;
		mainCamera.enabled = true;
		mainCamera.transform.position = new Vector3 (0, 4, -1);
	}

	// Update is called once per frame
	void Update () {
		int elapsed = Mathf.FloorToInt(Time.time - startTime);
		if (elapsed >= 4 && elapsed < 15) {
			mainCamera.transform.LookAt (preset.transform.GetChild(elapsed-4).transform);
		} 
	}
}
