using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskSphere : MonoBehaviour {

	public GameObject target;

	DateTime now = DateTime.Now;
	DateTime parallels_time = DateTime.Now;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
            //Instantiate( 生成するオブジェクト,  場所, 回転 );  回転はそのままなら↓
            Instantiate (target, new Vector3 (1.0f, 2.0f, 0.0f), Quaternion.identity);
        }
		
	}
}
