﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wandering : MonoBehaviour {

	private float speed = 1.0f;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		Vector3 move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
		transform.position += move * speed * Time.deltaTime;
	}
}
