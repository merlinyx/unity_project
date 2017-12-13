using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeStatesController : MonoBehaviour {

	GameObject states;
	Component[] trees;
	ParticleSystem psys;
	double timer;
	int index;

	// Use this for initialization
	void Start () {
		states = GameObject.FindWithTag ("TreeStates");
		trees = states.GetComponentsInChildren (typeof(Tree));
		psys = (ParticleSystem) states.GetComponentInChildren (typeof(ParticleSystem));
		if (trees == null) {
			Debug.Log ("got no tree children!");
		} else {
			Debug.Log (trees.Length);
			for (int i = 0; i < trees.Length; ++i) {
				MeshRenderer n = trees [i].GetComponent<MeshRenderer> ();
				n.enabled = false;
			}
			index = 0;
			MeshRenderer m = trees [index].GetComponent<MeshRenderer> ();
			m.enabled = true;
		}
		if (psys == null) {
			Debug.Log ("got no particle system children!");
		} else {
			Debug.Log ("has particle system");
			psys.Play ();
		}
		timer = 0.0;
	}
	
	// Update is called once per frame
	void Update () {
		if (timer >= 3.0) {
			psys.Clear ();
			timer = Time.deltaTime;
			for (int i = 0; i < trees.Length; ++i) {
				MeshRenderer n = trees [i].GetComponent<MeshRenderer> ();
				n.enabled = false;
			}
			index = (index + 1) % 3;
			MeshRenderer m = trees [index].GetComponent<MeshRenderer> ();
			m.enabled = true;
		} else {
			timer += Time.deltaTime;
		}
	}
//	void Update () {
//		if (timer >= 4.0) {
//			timer = Time.deltaTime;
//			for (int i = 0; i < trees.Length; ++i) {
//				MeshRenderer n = trees [i].GetComponent<MeshRenderer> ();
//				n.enabled = false;
//			}
//			index = (index + 1) % 3;
//			MeshRenderer m = trees [index].GetComponent<MeshRenderer> ();
//			m.enabled = true;
//		} else if (4.0 > timer && timer >= 2.0) {
//			if (count % 15 == 0) {
//				psys.Emit (20);
//			}
//			++count;
//			timer += Time.deltaTime;
//		} else {
//			count = 0;
//			timer += Time.deltaTime;
//			psys.Clear ();
//		}
//	}

//	void Update() {
//		if (Input.GetKeyDown (KeyCode.Space)) {
//			for (int i = 0; i < trees.Length; ++i) {
//				MeshRenderer n = trees [i].GetComponent<MeshRenderer> ();
//				n.enabled = false;
//			}
//			psys.Play ();
//			StartCoroutine(DoTheDance());
//			index = (index + 1) % 3;
//			MeshRenderer m = trees [index].GetComponent<MeshRenderer> ();
//			m.enabled = true;
//			psys.Pause ();
//		}
//	}
//
//	IEnumerator DoTheDance() {
//		yield return new WaitForSeconds(3f); // waits 1 second
//	}
}
