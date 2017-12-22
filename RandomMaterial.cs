using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomMaterial : MonoBehaviour {

	private int whichIvy;
	private int index;
	private string[] names = {"word_dark", "word_eyes", "word_hand", "word_heart", "word_light",
		"word_long", "word_love", "word_night", "word_sky", "word_time", "word_world"};
	private GameObject ivyList;
	private Component[] ivies;

	// Use this for initialization
	void Start () {
		whichIvy = -1;
		index = 0;
		ivyList = GameObject.FindWithTag ("IvyList");
		ivies = ivyList.GetComponentsInChildren (typeof(RuntimeIvy));
		Random.InitState (3104);
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Alpha1) || Input.GetKeyDown (KeyCode.Keypad1)) {
			whichIvy = 0;
		} else if (Input.GetKeyDown (KeyCode.Alpha2) || Input.GetKeyDown (KeyCode.Keypad2)) {
			whichIvy = 1;
		} else if (Input.GetKeyDown (KeyCode.Alpha3) || Input.GetKeyDown (KeyCode.Keypad3)) {
			whichIvy = 2;
		}
		if (whichIvy != -1) {
			if (Input.GetKeyDown (KeyCode.Space)) {
				index = (index + Random.Range (0, 11) - 1) % 11;
//				Debug.Log (index);
				Material mat = Resources.Load (names [index], typeof(Material)) as Material;
				ivies[whichIvy].GetComponent<RTIvyController>().SendLeafMaterial (mat);
			}
		}
	}
}
