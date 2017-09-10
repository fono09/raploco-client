using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishRotationController : MonoBehaviour {

	// Use this for initialization
	public GameObject obj1;
    private object ll;

    void Start () {
		obj1 = GameObject.Find("lights/player");
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.LookAt(obj1.transform, -Vector3.forward);
	}
}
