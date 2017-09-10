using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuConrtoller : MonoBehaviour {

	public GameObject Light;
	public GameObject TimeLabel;

	private string datetimeStr,datetimeStrY,datetimeStrMo,datetimeStrD,datetimeStrH;
	private int datetimeStrS,datetimeStrM;
	float pr = 0;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		pr += 0.5f + pr/20;
		if(pr > 200){
			pr = 0;
		}


		datetimeStrY = System.DateTime.Now.Year.ToString();
		// 取得する値: 月
		datetimeStrMo = System.DateTime.Now.Month.ToString();
		// 取得する値: 日
		datetimeStrD = System.DateTime.Now.Day.ToString();
		// 取得する値: 時
		datetimeStrH = System.DateTime.Now.Hour.ToString();
		// 取得する値: 分
		datetimeStrM = System.DateTime.Now.Minute;
		// 取得する値: 秒
		datetimeStrS = System.DateTime.Now.Second;
		// 取得する値: コンマミリ秒

		Light.GetComponent<Light>().range = pr;
		datetimeStr = System.DateTime.Now.ToString();
		TimeLabel.GetComponent<Text>().text = datetimeStrY+"/"+datetimeStrMo+"/"+datetimeStrD+"\n"+datetimeStrH+":"+String.Format("{0:D2}", datetimeStrM)+":"+String.Format("{0:D2}", datetimeStrS) ;
	}
}
