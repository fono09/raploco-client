using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class HTTPManager : SingletonMonoBehaviour<HTTPManager>
{

    private static readonly string baseUrl = "https://raploco-server.herokuapp.com/";
    private static readonly string testUrl = "ok";
    public static readonly string userUrl = "users";
    public static readonly string taskUrl = "tasks";
    public static readonly string favoriteUrl = "users/favorites";
    public static readonly string genreUrl = "genres";

    private string uuid;

    private Dictionary<string, string> headers = new Dictionary<string, string> ();

    private bool initializedFlag = false;
    private bool locked = false;

    public delegate void HttpHandler (string result);

    public delegate void GenreHandler (List<Genre> genreList);

    public delegate void ErrorHandler (Exception e);

    private event ErrorHandler OnError = (err) => {UnityEngine.Debug.Log(err);};

    // Use this for initialization
    public override void Awake ()
    {
        base.Awake ();
        StartCoroutine (onAwake ());
    }

    // Update is called once per frame
    void Update ()
    {

    }

    [Serializable]
    private class UserData {
        public string name;
        public string token;
        public int id;
    }
   
    [Serializable]
    private class TestResponse {
        public string message;
    }

    [Serializable]
    private class NameData
    {
        public string name;

        public NameData(string name) {
            this.name = name;
        }
    }

    [Serializable]
    public class TaskListPacket
    {
        public Task[] tasks;
    }

    [Serializable]
    public class PostFavoritePacket
    {
        public int id; 
        public PostFavoritePacket(int id) {
            this.id = id;
        }
    }

    [Serializable]
    public class Favorites
    {
        public User[] users;
    }

   
    private IEnumerator onAwake ()
    {
        this.uuid = getUUID ();
        if (this.uuid == null) {
            if (Application.loadedLevelName == "Start") {
                while (this.uuid == null) {
                    yield return null;
                }
            } else {
                SceneManager.LoadScene ("Start");
            }
        } else {
            this.headers.Add("X-Token", this.uuid);
        }

        initializedFlag = true;
    }

    public IEnumerator GetGenreList(GenreHandler handler) {
        List<Genre> genreList = new List<Genre> ();
        yield return HTTPManager.instance.GetData (
            HTTPManager.genreUrl,
            (result) => {
                Genres gs = JsonUtility.FromJson<Genres> (result);
                genreList = new List<Genre> (gs.genres);
            });
        handler (genreList);
    }

    public IEnumerator RegisterUser(string name) {
        NameData post = new NameData (name);
        yield return PostData (userUrl,JsonUtility.ToJson(post),
            (result) => {
                UserData res = JsonUtility.FromJson<UserData> (result);
                this.uuid = res.token;
                SaveUUID(this.uuid);
                this.headers.Add ("X-Token", this.uuid);
            }, true);
    }

    public IEnumerator GetTasks(HttpHandler handler) {
        yield return GetData (taskUrl, handler);
    }

    public IEnumerator UpdateTask(int id, string name, int cost, DateTime time, int user_id, int genre_id, HttpHandler handler) {
        PutTaskObj obj = new PutTaskObj (id, name, cost, time.ToString("o"), user_id, genre_id);
        string body = JsonUtility.ToJson (obj);
        UnityWebRequest request = UnityWebRequest.Put (baseUrl + taskUrl + "/" + id.ToString(), body);
        request.SetRequestHeader ("X-Token", this.uuid);
        yield return request.Send ();
        if (request.isNetworkError) {
            Debug.Log(request.error);
        } else {
            if (request.responseCode == 200) {
                string text = request.downloadHandler.text;
                handler (text);
            }
        }
    }

    public IEnumerator DeleteTask(int id) {
        UnityWebRequest request = UnityWebRequest.Delete (baseUrl + taskUrl + "/"+ id.ToString());
        request.SetRequestHeader ("X-Token", this.uuid);
        yield return request.Send ();
        if (request.isNetworkError) {
            Debug.Log(request.error);
        } else {
            Debug.Log (request.responseCode);
        }
    }


    public IEnumerator DeleteFavorite(int id) {
        UnityWebRequest request = UnityWebRequest.Delete (baseUrl + favoriteUrl + "/" + id.ToString ());
        request.SetRequestHeader ("X-Token", this.uuid);
        yield return request.Send ();
        if (request.isNetworkError) {
            Debug.Log(request.error);
        } else {
            Debug.Log (request.responseCode);
        }
    }

    public IEnumerator PostData (string url, string body, HttpHandler handler, bool initialize = false)
    {
        while (locked || (!initializedFlag && !initialize)) {
            yield return null;
        }
        UnityEngine.Debug.Log (url);
        locked = true;
        url = baseUrl + url;

        byte[] postBytes = Encoding.UTF8.GetBytes (body);
            
        using (UnityEngine.WWW www = new UnityEngine.WWW (url, postBytes, headers)) {
            yield return www;
            try {
                string text = www.text;
                handler (text);
            } catch (Exception e) {
                OnError (e);
            }
        }
        locked = false;
        yield return null;
    }
        
    public IEnumerator GetData (string url, HttpHandler handler, bool initialize = false)
    {
        while (locked || (!initializedFlag && !initialize)) {
            yield return null;
        }
 
        locked = true;
        UnityEngine.Debug.Log (url);
        url = baseUrl + url;
        using (UnityEngine.WWW www = new UnityEngine.WWW (url, null, headers)) {
            yield return www;
            UnityEngine.Debug.Log (www.text);
            try {
                string text = www.text;
                handler (text);
            } catch (Exception e) {
                OnError (e);
            }
        }
        locked = false;
        yield return null;
    }

    public bool UuidIsNull() {
        return this.uuid == null;
    }

    public bool IsInitialized ()
    {
        return this.initializedFlag;
    }

    public void SaveUUID (string UUID)
    {
        try {
            UnityEngine.PlayerPrefs.SetString ("uuid", UUID);
            UnityEngine.PlayerPrefs.Save ();
        } catch (Exception e) {
            OnError (e);
        }
    }

    private string getUUID ()
    {
        try {
            if (!UnityEngine.PlayerPrefs.HasKey ("uuid"))
                return null;
            string UUID = UnityEngine.PlayerPrefs.GetString ("uuid");
            UnityEngine.Debug.Log ("uuid: " + UUID);
            return UUID;
        } catch (Exception e) {
            OnError (e);
        }
        return null;
    }
        
    public void SetOnErrorHandler (ErrorHandler handler)
    {
        this.OnError += handler;
    }
}
