using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIManager : SingletonMonoBehaviour<MenuUIManager> {
    [SerializeField]
    private GameObject InsertTaskPanel;

    [SerializeField] 
    private Button TaskImgButton;

    [SerializeField]
    private GameObject AddGenere;

    private List<Genre> genreList;

    private bool genreAddMode = true;

    private Color buttonColor;

	// Use this for initialization
	void Start () {
        TaskImgButton.onClick.AddListener (showTaskPanel);
        InsertTaskPanel.transform.Find ("HorizontalLayout/Buttons/BackButton")
            .GetComponent<Button> ().onClick.AddListener (hideTaskPanel);
        InsertTaskPanel.transform.Find ("HorizontalLayout/Buttons/AssignButton")
            .GetComponent<Button> ().onClick.AddListener (transitToAssignScene);
        InsertTaskPanel.transform.Find ("HorizontalLayout/Genre/Button")
            .GetComponent<Button> ().onClick.AddListener (changeAddGenreField);

        ColorUtility.TryParseHtmlString("#2B7DECFF", out buttonColor);

        StartCoroutine (HTTPManager.instance.GetData(
            HTTPManager.genreUrl,
            (result) => {
                Genres gs = JsonUtility.FromJson<Genres>(result);
                this.genreList =new List<Genre>(gs.genres);
                updateGenres();
            }
        ));
    }

    private void showTaskPanel() {
        InsertTaskPanel.SetActive (true);
        TaskImgButton.gameObject.SetActive (false);
    }

    private void hideTaskPanel() {
        resetTaskPanelValue ();
        InsertTaskPanel.SetActive (false);
        TaskImgButton.gameObject.SetActive (true);
    }

    private void transitToAssignScene() {
        PostTaskObj task = new PostTaskObj(
            InsertTaskPanel.transform.Find ("HorizontalLayout/TaskName/InputField/Text").GetComponent<Text>().text,
            (int)InsertTaskPanel.transform.Find("HorizontalLayout/Cost/Slider").GetComponent<Slider>().value * 100,
            InsertTaskPanel.transform.Find("HorizontalLayout/Deadline/InputField/Text").GetComponent<Text>().text);
        //TO BE IMPLEMENTED
    }

    private void resetTaskPanelValue() {
        InsertTaskPanel.transform.Find ("HorizontalLayout/TaskName/InputField/Text").GetComponent<Text> ().text = "";
        InsertTaskPanel.transform.Find ("HorizontalLayout/Genre/Dropdown").GetComponent<Dropdown> ().value = 0;
        InsertTaskPanel.transform.Find ("HorizontalLayout/Cost/Slider").GetComponent<Slider> ().value = 0;
        InsertTaskPanel.transform.Find ("HorizontalLayout/Deadline/InputField/Text").GetComponent<Text> ().text = "";
    }

    private void changeAddGenreField() {
        if (genreAddMode) {
            AddGenere.SetActive (true);
           
            Button b = AddGenere.transform.GetComponentInChildren<Button> ();
          
            b.onClick.RemoveAllListeners();
            b.onClick.AddListener (addGenre);
            
            InsertTaskPanel.transform.Find ("HorizontalLayout/Genre/Button/Text")
                .GetComponent<Text> ().text = "-";
            InsertTaskPanel.transform.Find ("HorizontalLayout/Genre/Button")
                .GetComponent<Image> ().color = Color.gray;
            
            genreAddMode = false;
        } else {
            
            InsertTaskPanel.transform.Find ("HorizontalLayout/Genre/Button/Text")
                .GetComponent<Text> ().text = "+"; 
            InsertTaskPanel.transform.Find ("HorizontalLayout/Genre/Button")
                .GetComponent<Image> ().color = buttonColor;
            AddGenere.transform.Find ("InputField/Text")
                .GetComponent<Text> ().text = "";
            AddGenere.SetActive (false);

            genreAddMode = true;
        }
    }

    private void addGenre() {
        string newGenreTxt = AddGenere.transform.Find("InputField/Text").GetComponent<Text>().text;
        PostGenreCreatePacket p = new PostGenreCreatePacket (newGenreTxt);
        changeAddGenreField ();
        StartCoroutine(HTTPManager.instance.PostData(
            HTTPManager.genreUrl,
            JsonUtility.ToJson(p),
            (result) =>  {
                Genre g = JsonUtility.FromJson<Genre>(result);
                if(!alreadyHasGenre(g)) {
                    this.genreList.Add(g);
                    updateGenres();
                }
            }));
    }


    
    private bool alreadyHasGenre(Genre n) {
        foreach (Genre g in this.genreList) {
            if (g.id == n.id) {
                return true;
            }
        }
        return false;
    }
                   
                

    private void updateGenres() {
        Dropdown dropdown = 
            InsertTaskPanel.transform.Find("HorizontalLayout/Genre/Dropdown").GetComponent<Dropdown>();
        dropdown.ClearOptions();
        List<string> options = new List<string> ();
        foreach (Genre g in this.genreList) {
            options.Add (g.name);
        }
        dropdown.AddOptions(options);
    }
}
