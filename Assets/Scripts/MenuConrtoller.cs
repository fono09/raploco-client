using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuConrtoller : MonoBehaviour {

	public GameObject Light;
	public GameObject TimeLabel;

	public GameObject target; 

	private string datetimeStr,datetimeStrY,datetimeStrMo,datetimeStrD,datetimeStrH;
	private int datetimeStrS,datetimeStrM;
	float pr = 0;

	private DateTime now_time;

	List<Task> tasks = new List<Task> ();
	List<GameObject> fishList;


	// Use this for initialization
	void Start () {
		now_time = System.DateTime.Now;
        StartCoroutine (applyTaskDatas ());
	}


    public IEnumerator applyTaskDatas() {
        yield return HTTPManager.instance.GetTasks ((result) => {
            Debug.Log(result);
            tasks = new List<Task>(JsonUtility.FromJson<HTTPManager.TaskListPacket>(result).tasks);
        });

       
        int s = 0;
        foreach(Task n in tasks)
        {   
            s += 1;

            TimeSpan kk = n.DeadlineTime - now_time;
            //Debug.Log(kk.TotalSeconds);

            GameObject new_fish = Instantiate(target, new Vector3 (0.0f, 0.00001f*((float)kk.TotalSeconds), -1.0f), Quaternion.Euler(0, 0, 0));
            GameObject new_fish_text = new_fish.transform.Find("TaskName").gameObject;
            Debug.Log(new_fish_text);
            new_fish_text.GetComponent<TextMesh>().text = n.name;
            //fishList.Add(new_fish);
        }
    }

	
	// Update is called once per frame
	void Update () {
		pr += 0.5f + pr/20;
		if(pr > 200){
			pr = 0;
		}

		datetimeStrY = System.DateTime.Now.Year.ToString();
		datetimeStrMo = System.DateTime.Now.Month.ToString();
		datetimeStrD = System.DateTime.Now.Day.ToString();
		datetimeStrH = System.DateTime.Now.Hour.ToString();
		datetimeStrM = System.DateTime.Now.Minute;
		datetimeStrS = System.DateTime.Now.Second;

		Light.GetComponent<Light>().range = pr;
		datetimeStr = System.DateTime.Now.ToString();
		TimeLabel.GetComponent<Text>().text = datetimeStrY+"/"+datetimeStrMo+"/"+datetimeStrD+"\n"+datetimeStrH+":"+String.Format("{0:D2}", datetimeStrM)+":"+String.Format("{0:D2}", datetimeStrS);
	}
}
