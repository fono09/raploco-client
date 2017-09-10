using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : MonoBehaviour {
    [SerializeField]
    private GameObject InsertTaskPanel;

    [SerializeField]
    private Button TaskImgButton;


	// Use this for initialization
	void Start () {
        TaskImgButton.onClick.AddListener (showTaskPanel);
        InsertTaskPanel.transform.Find ("Buttons/BackButton").GetComponent<Button> ().onClick.AddListener (hideTaskPanel);
        InsertTaskPanel.transform.Find ("Buttons/AssignButton").GetComponent<Button> ().onClick.AddListener (transitToAssignScene);
    }

    private void showTaskPanel() {
        InsertTaskPanel.SetActive (true);
        TaskImgButton.gameObject.SetActive (false);
    }

    private void hideTaskPanel() {
        InsertTaskPanel.SetActive (false);
        TaskImgButton.gameObject.SetActive (true);
    }

    private void transitToAssignScene() {
        //TO BE IMPLEMENTED
    }

    private void resetTaskPanelValue() {
        InsertTaskPanel.transform.Find ("TaskName/inputField/Text").GetComponent<Text> ().text = "";
        InsertTaskPanel.transform.Find ("Genre/Dropdown").GetComponent<Dropdown> ().value = 0;
        InsertTaskPanel.transform.Find ("Cost/Slider").GetComponent<Slider> ().value = 0;
        InsertTaskPanel.transform.Find ("Deadline/inputField/Text").GetComponent<Text> ().text = "";
    }
}
