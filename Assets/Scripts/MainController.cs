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

    [SerializeField]
    private GameObject[] Teaddybears;

	List<Task> tasks = new List<Task> ();
	List<GameObject> fishList;



	public GameObject TimeLabel; 

	private string datetimeStr,datetimeStrY,datetimeStrMo,datetimeStrD,datetimeStrH;
	private int datetimeStrS,datetimeStrM;

	void Start () {
        StartCoroutine (applyTaskDatas());
        StartCoroutine (setKuma ());
	}

    private IEnumerator applyTaskDatas() {
        yield return HTTPManager.instance.GetTasks ((result) => {
            Debug.Log (result);
            tasks = new List<Task> (JsonUtility.FromJson<HTTPManager.TaskListPacket> (result).tasks);
        });

        now_time = System.DateTime.Now;

        int s = 0;
		yield return HTTPManager.instance.GetGenreList((result) => {
			foreach(Task n in tasks)
			{   
				s += 1;

				TimeSpan kk = n.DeadlineTime - now_time;
				GameObject new_fish = Instantiate(target, GetPositionOnSphere((2.0f / result.Count)*n.genre_id - 1.0f,0.0f,0.00005f*((float)kk.TotalSeconds)), Quaternion.Euler(1, 1, 1));
				new_fish.GetComponent<TaskHolder> ().task = n;
				GameObject new_fish_text = new_fish.transform.Find("TaskName").gameObject;
				new_fish_text.GetComponent<TextMesh>().text = n.name;
				GameObject new_dead_text = new_fish.transform.Find("DeadTime").gameObject;
				new_dead_text.GetComponent<TextMesh>().text = n.deadline;
				//fishList.Add(new_fish);
			}
		});
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
		/*
		if (Screen.orientation == ScreenOrientation.Portrait || Screen.orientation == ScreenOrientation.PortraitUpsideDown) {
			SceneManager.LoadScene ("Menu");
		}
  */      
	}

	public Vector3 GetPositionOnSphere(float angle1, float angle2, float r)
	{
	        float x = r * Mathf.Sin(angle1) * Mathf.Cos(angle2);
	        float y = r * Mathf.Sin(angle1) * Mathf.Sin(angle2);
	        float z = r * Mathf.Cos(angle1);
	        return new Vector3(x, y, z);
	}

    private IEnumerator setKuma() {
        List<User> favorites = new List<User> ();
        System.Random random = new System.Random();
        yield return HTTPManager.instance.GetData (HTTPManager.favoriteUrl,
            ((result) => {
                favorites = new List<User>(JsonUtility.FromJson<HTTPManager.Favorites>(result).users);
            }));
        List<int> usedList = new List<int> ();
        for (int i = 0; i < 3; i++) { 
            while (true) {
                int rand = random.Next (favorites.Count);
                if (!usedList.Contains (rand)) {
                    Teaddybears[i].GetComponent<UserHolder> ().user = favorites[rand];
                    usedList.Add (rand);
                    break;
                }
                if (favorites.Count <= i) {
                    Teaddybears [i].SetActive (false);
                    break;
                }
            }
        }
    }
}
