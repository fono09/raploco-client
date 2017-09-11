using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BearRotationController : MonoBehaviour {


	public GameObject obj1;
    private object ll;

    void Start () {
		obj1 = GameObject.Find("Main Camera");
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.LookAt(obj1.transform);
	}
}
