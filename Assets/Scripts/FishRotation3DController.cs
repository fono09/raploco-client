using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishRotation3DController : MonoBehaviour {

	// Use this for initialization

	public GameObject obj1;
    private object ll;
	

    void Start () {
		obj1 = GameObject.Find("Main Camera");
        transform.GetComponent<Animator> ().speed = Random.Range (0.6f, 1.0f);
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.LookAt(obj1.transform);
	}
}
