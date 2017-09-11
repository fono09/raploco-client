using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishRotation3DController : MonoBehaviour {

	// Use this for initialization

	public GameObject obj1;
    private object ll;

    public bool reverse = false;
	

    void Start () {
		obj1 = GameObject.Find("Main Camera");
       
        Animator a = transform.GetComponent<Animator> ();
        if (a != null) {
            a.speed = Random.Range (0.6f, 1.0f);
        }
        Animation a2 = transform.GetComponent<Animation>();
        float r = Random.Range (0.6f, 1.0f);
        if (a2 != null) {
            foreach (AnimationState s in a2) {
                s.speed = r;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		this.transform.LookAt(obj1.transform);
        if (reverse) {
            transform.Rotate (new Vector3 (0, 180, 0));
        }
	}
}
