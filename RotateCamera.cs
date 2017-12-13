using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateCamera : MonoBehaviour {

	public GameObject pivot;

	private Vector3 pivotPoint;
	private float startTime;
	Camera m_mainCamera;

	// Use this for initialization
	void Start () {
		pivotPoint = pivot.transform.position;
		startTime = Time.time;
		m_mainCamera = Camera.main;
		m_mainCamera.enabled = true;
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.time - startTime <= 36 && Time.time - startTime > 30) {
			m_mainCamera.transform.RotateAround (pivotPoint, Vector3.up, 1.0f);
			m_mainCamera.transform.LookAt (pivot.transform);
		} 
	}
}
