using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainController : MonoBehaviour {

	// Use this for initialization

	private DateTime now_time;

	public GameObject target; 

	List<Task> tasks = new List<Task> ();
	List<GameObject> fishList;



	public GameObject TimeLabel; 

	private string datetimeStr,datetimeStrY,datetimeStrMo,datetimeStrD,datetimeStrH;
	private int datetimeStrS,datetimeStrM;

	void Start () {
        StartCoroutine (applyTaskDatas());
	}

    private IEnumerator applyTaskDatas() {
        yield return HTTPManager.instance.GetTasks ((result) => {
            Debug.Log (result);
            tasks = new List<Task> (JsonUtility.FromJson<HTTPManager.TaskListPacket> (result).tasks);
        });

        now_time = System.DateTime.Now;

        int s = 0;
        foreach(Task n in tasks)
        {   
            s += 1;

            TimeSpan kk = n.DeadlineTime - now_time;
            GameObject new_fish = Instantiate(target, GetPositionOnSphere(n.id*0.1f,0.0f,0.00005f*((float)kk.TotalSeconds)), Quaternion.Euler(0, 0, 0));
            new_fish.GetComponent<TaskHolder> ().task = n;
            GameObject new_fish_text = new_fish.transform.Find("TaskName").gameObject;
            new_fish_text.GetComponent<TextMesh>().text = n.name;
            //fishList.Add(new_fish);
        }
    } 
	
	// Update is called once per frame
	void Update () {
		

		datetimeStrY = System.DateTime.Now.Year.ToString();
		datetimeStrMo = System.DateTime.Now.Month.ToString();
		datetimeStrD = System.DateTime.Now.Day.ToString();
		datetimeStrH = System.DateTime.Now.Hour.ToString();
		datetimeStrM = System.DateTime.Now.Minute;
		datetimeStrS = System.DateTime.Now.Second;
		datetimeStr = System.DateTime.Now.ToString();
		TimeLabel.GetComponent<Text>().text = datetimeStrY+"/"+datetimeStrMo+"/"+datetimeStrD+"\n"+datetimeStrH+":"+String.Format("{0:D2}", datetimeStrM)+":"+String.Format("{0:D2}", datetimeStrS);
		
		if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown) {
			SceneManager.LoadScene ("Menu");
		}
	}

	public Vector3 GetPositionOnSphere(float angle1, float angle2, float r)
	{
	        float x = r * Mathf.Sin(angle1) * Mathf.Cos(angle2);
	        float y = r * Mathf.Sin(angle1) * Mathf.Sin(angle2);
	        float z = r * Mathf.Cos(angle1);
	        return new Vector3(x, y, z);
	}
}
