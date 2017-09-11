using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {
    private GUIStyle labelStyle;
    Quaternion start_gyro;
    Quaternion gyro;

    GameObject last_object;

    int ShowTimeCount;




    public float speed = 0.001F;
    private float startTime;
    private float journeyLength;



    GameObject Now_hand_fish;
    
    Vector3 First_Position;
    Quaternion First_Rotation;

    int move_hand_status;
    
	public GameObject DeadLabel;

    public GameObject kuma;
    public float throwTime = 2.0f;
    private float currentTime = 0.0f;


    void Start()
    {
        //this.labelStyle = new GUIStyle();
        //this.labelStyle.fontSize = Screen.height / 22;
        //this.labelStyle.normal.textColor = Color.white;
        //後述するがここで「Start」シーンのジャイロの値を取っている
　　　　　start_gyro = StartCameraController.ini_gyro;
        move_hand_status = 0;

    }


    void Update () {
        Ray ray = new Ray(this.transform.position, this.transform.forward);
        RaycastHit hit;
        GameObject tn = GameObject.Find("TaskName");
        if (tn != null){
            tn.SetActive(false);
        }
        GameObject dt = GameObject.Find("DeadTime");
        if (dt != null){
            dt.SetActive(false);
        }

		if (Physics.Raycast(ray,out hit, Mathf.Infinity)){
            Debug.Log(hit.transform.name);
            if (hit.transform.name == "3DFishPoint(Clone)") {
                hit.transform.Find ("TaskName").gameObject.SetActive (true);
                hit.transform.Find ("DeadTime").gameObject.SetActive (true);
                if (last_object == hit.transform.gameObject) {
                    ShowTimeCount += 1; 
                } else {
                    ShowTimeCount = 0;
                }

                if (ShowTimeCount > 50 && move_hand_status == 0) {
                    move_hand_status = 1;
                    First_Position = hit.transform.position;
                    First_Rotation = hit.transform.rotation;
                    Now_hand_fish = hit.transform.gameObject;
                    startTime = Time.time;
                    journeyLength = Vector3.Distance (hit.transform.position, this.transform.position);
                } else {
                    if (move_hand_status == 0) {
                        hit.transform.position = new Vector3 (hit.transform.position.x + (Mathf.Sin (ShowTimeCount) * 0.05f), hit.transform.position.y, hit.transform.position.z);
                    }
                }
                last_object = hit.transform.gameObject;
            } else if (hit.transform.name == "back") {
                if (move_hand_status == 2) {
                    move_hand_status = 0;
                    Now_hand_fish.transform.parent = null;
                    Now_hand_fish.transform.position = First_Position;
                    Now_hand_fish.transform.rotation = First_Rotation;
                    Now_hand_fish.transform.GetComponent<Collider> ().enabled = true;
                }
            } else if (hit.transform.name == "del") {
                if (move_hand_status == 2) {
                    move_hand_status = 0;
                    StartCoroutine (HTTPManager.instance.DeleteTask (Now_hand_fish.GetComponent<TaskHolder> ().task.id));
                    Destroy (Now_hand_fish);
                }
            } else if (hit.transform.tag == "kuma") {
                if (move_hand_status == 2) {
                    move_hand_status = 3;
                    Task t = Now_hand_fish.GetComponent<TaskHolder> ().task;
                    User u = hit.transform.GetComponent<UserHolder> ().user;
                    kuma = hit.transform.gameObject;
                    currentTime = 0.0f;
                    hit.transform.LookAt (kuma.transform.position);
                    StartCoroutine (HTTPManager.instance.UpdateTask(t.id, t.name, t.cost, t.DateTime, u.id, t.genre_id, ((result) => {
                        Debug.Log(result);
                    })));
                }
            }
        }

        if (move_hand_status == 3) {
            if (currentTime > throwTime) {
                move_hand_status = 0;
                currentTime = 0;
                Destroy (Now_hand_fish);
            }
            Now_hand_fish.transform.position = Vector3.Lerp (Camera.main.transform.position, kuma.transform.position, currentTime / throwTime);
            currentTime += Time.deltaTime;
        } 


        if (move_hand_status == 1){
            float distCovered = (Time.time - startTime) * speed;
            float fracJourney = distCovered / journeyLength;
            Now_hand_fish.transform.position = Vector3.Lerp(Now_hand_fish.transform.position,this.transform.position, fracJourney);
            float dis = Vector3.Distance(Now_hand_fish.transform.position,this.transform.position);
            if (dis < 3.0f){
                move_hand_status = 2;
            }
        }

        if (move_hand_status == 2){
            //Debug.Log(GetPositionOnSphere(5.0f,5.0f,1.0f));
            //Now_hand_fish.transform.position = GetPositionOnSphere(Quaternion.AngleAxis(this.transform.rotation,Vector3.right),0.0f,1.0f);
            
            Now_hand_fish.transform.parent = this.transform;
            //Now_hand_fish.transform.position = new Vector3(-3,0,0);
            Now_hand_fish.transform.Find("TaskName").gameObject.SetActive(true);
            Now_hand_fish.transform.Find("DeadTime").gameObject.SetActive(true);
            Now_hand_fish.transform.GetComponent<Collider>().enabled = false;

            Debug.Log (Now_hand_fish.transform.Find ("DeadTime").gameObject.GetComponent<TextMesh> ().text);
            DateTime endtime = Convert.ToDateTime (Now_hand_fish.transform.Find("DeadTime").gameObject.GetComponent<TextMesh>().text); 
            DateTime now_time = System.DateTime.Now;
			TimeSpan kk = endtime - now_time;
            DeadLabel = GameObject.Find("Nokori");
            DeadLabel.GetComponent<Text>().text = string.Format("{0:D2}:{1:D2}:{2:D2}:{3:D2}", kk.Days, kk.Hours, kk.Minutes, kk.Seconds);

            //Now_hand_fish.transform.rotation = Quaternion.Euler(10,10,10);
        }


        Input.gyro.enabled = true;
        if (Input.gyro.enabled)
        {
            gyro = Input.gyro.attitude;
            gyro = Quaternion.Euler(90, 0, 0) * (new Quaternion(-gyro.x,-gyro.y, gyro.z, gyro.w));
            this.transform.localRotation = gyro;
            //最初に見ていた向きとゲームの進行方向を合わせる
            //this.transform.localRotation = Quaternion.Euler(0, -start_gyro.y, 0);
        }

    }

    public Vector3 GetPositionOnSphere(float angle1, float angle2, float r)
	{
	        float x = r * Mathf.Sin(angle1) * Mathf.Cos(angle2);
	        float y = r * Mathf.Sin(angle1) * Mathf.Sin(angle2);
	        float z = r * Mathf.Cos(angle1);
	        return new Vector3(x, y, z);
	}

    //ジャイロセンサの値を表示するプログラム
    void OnGUI()
    {
        if (Input.gyro.enabled)
        {
            float x = Screen.width / 10;
            float y = 0;
            float w = Screen.width * 8 / 10;
            float h = Screen.height / 20;

            for (int i = 0; i < 3; i++)
            {
                y = Screen.height / 10 + h * i;
                string text = string.Empty;

                switch (i)
                {
                case 0://X
                    text = string.Format("gyro-X:{0}", gyro.x);
                    break;
                case 1://Y
                    text = string.Format("gyro-Y:{0}", gyro.y);
                    break;
                case 2://Z
                    text = string.Format("gyro-Z:{0}", gyro.z);
                    break;
                default:
                    throw new System.InvalidOperationException();
                }

                //GUI.Label(new Rect(x, y, w, h), text, this.labelStyle);
            }
        }
    }

}
