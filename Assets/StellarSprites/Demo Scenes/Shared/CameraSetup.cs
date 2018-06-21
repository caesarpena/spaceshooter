using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSetup : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.GetComponent<Camera>().orthographicSize = Screen.height / 1f / 100f;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
