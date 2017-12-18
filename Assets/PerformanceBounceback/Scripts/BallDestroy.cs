using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDestroy : MonoBehaviour {

	// Use this for initialization
	void OnEnable () {
		Invoke ("Destroy", 5f);
		
	}
	
	// Update is called once per frame
	void Destroy () {
		gameObject.SetActive (false);
	}

	void OnDisable(){
		CancelInvoke ();
	}
}
