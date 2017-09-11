using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuUIManager : SingletonMonoBehaviour<MenuUIManager> {
    [SerializeField]
    private GameObject InsertTaskPanel;

    [SerializeField]
    private GameObject SearchUserPanel;

    [SerializeField]
    private GameObject FavoritePanel;

    [SerializeField] 
    private Button TaskImgButton;

    [SerializeField]
    private GameObject AddGenere;

    [SerializeField]
    private GameObject UserPanelPrefab;

    [SerializeField]
    private GameObject CreateFavoriteButton;

    [SerializeField]
    private GameObject BackButton;

    private List<Genre> genreList;

    private List<User> users;

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

    public void Update() {
        //Debug.Log (Screen.orientation);
        if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight) {
            transitScene ();
        }
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
            InsertTaskPanel.transform.Find("HorizontalLayout/Deadline/InputField/Text").GetComponent<Text>().text,
            InsertTaskPanel.transform.Find("HorizontalLayout/Genre/Dropdown").GetComponent<Dropdown>().value
        );
        StartCoroutine (postTask (task));
    }

    private IEnumerator postTask(PostTaskObj task) { 
        yield return HTTPManager.instance.PostData (HTTPManager.taskUrl,
            JsonUtility.ToJson (task),
            ((result) => {
                Task t = JsonUtility.FromJson<Task>(result);
                MenuConrtoller.instance.CreateNewFish(t);
            }));
        hideTaskPanel ();
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

    public void ShowFavoritePanel() {
        FavoritePanel.SetActive (true);
        TaskImgButton.gameObject.SetActive (false);
        CreateFavoriteButton.SetActive (false);
        BackButton.SetActive (true);
        StartCoroutine(HTTPManager.instance.GetData(HTTPManager.favoriteUrl,
            (result) => {
                users = new List<User>(JsonUtility.FromJson<HTTPManager.Favorites>(result).users);
                adjustUsersToFavoriteView(users);
            }));
    }

    public void BackToMenu() {
        FavoritePanel.SetActive (false);
        TaskImgButton.gameObject.SetActive (true);
        CreateFavoriteButton.SetActive (true);
        BackButton.SetActive (false);
        FavoritePanel.transform.Find ("Input/InputField/Text").GetComponent<Text> ().text = "";
    } 


    public void CreateFavorite() {
        string text = FavoritePanel.transform.Find ("Input/InputField/Text").GetComponent<Text> ().text;
        FavoritePanel.transform.Find ("Input/InputField/Text").GetComponent<Text> ().text = "";
        try {
            int user_id = int.Parse(text);
            HTTPManager.PostFavoritePacket obj = new HTTPManager.PostFavoritePacket(user_id);
            StartCoroutine(postFavorite(obj));
        } catch (UnityException e) {
            //TODO Show Warn Invalid Input PopUp
            Debug.Log (e);
        }
    }

    public IEnumerator postFavorite(HTTPManager.PostFavoritePacket obj) {
        yield return HTTPManager.instance.PostData (HTTPManager.favoriteUrl,
            JsonUtility.ToJson (obj),
            (result) => {
                User u = JsonUtility.FromJson<User>(result);
                if(!isAlreadyFavorite(u)) {
                    users.Add(u);
                } 
                adjustUsersToFavoriteView(users);
            });
    }

    private bool isAlreadyFavorite(User u) {
        foreach (User i in users) {
            if (u.id == i.id) {
                return true;
            }
        }
        return false;
    } 
              
    public void transitScene() {
        SceneManager.LoadScene ("Main");   
    }

    private void adjustUsersToSearchView(List<User> users) {
        Transform contentRoot = SearchUserPanel.transform.Find ("ScrollView/Viewport/Content");
        foreach (User u in users) {
            GameObject prefab = Instantiate (UserPanelPrefab, contentRoot);
            setUserData (prefab, u);
        }
    }

    private void adjustUsersToFavoriteView(List<User> users) {
        Transform contentRoot = FavoritePanel.transform.Find ("ScrollView/Viewport/Content");
        Debug.Log (users);
        foreach ( Transform n in contentRoot.transform )
        {
            GameObject.Destroy(n.gameObject);
        }
        foreach (User u in users) {
            GameObject prefab = Instantiate (UserPanelPrefab, contentRoot);
            setUserData (prefab, u);
        }
    } 

    private void setUserData(GameObject userPanelPrefab, User user) {
        userPanelPrefab.transform.Find ("IdText").GetComponent<Text> ().text = string.Format("id: {0}", user.id);
        userPanelPrefab.transform.Find ("NameText").GetComponent<Text> ().text = string.Format ("name: {0}", user.name);
    } 
}
