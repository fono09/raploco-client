using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Networking;

public class HTTPManager : SingletonMonoBehaviour<HTTPManager>
{

    private static readonly string baseUrl = "https://raploco-server.herokuapp.com/";
    private static readonly string testUrl = "ok";
    private static readonly string userUrl = "users";
    public static readonly string taskUrl = "tasks";
    private static readonly string favoriteUrl = "users/favorites";
    public static readonly string genreUrl = "genres";

    private string uuid;

    private Dictionary<string, string> headers = new Dictionary<string, string> ();

    private bool initializedFlag = false;
    private bool locked = false;

    public delegate void HttpHandler (string result);

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
    private class PostFavoritePacket
    {
        public int id; 
        public PostFavoritePacket(int id) {
            this.id = id;
        }
    }

    [Serializable]
    private class PostGenreCreatePacket 
    {
        public string name;
        public PostGenreCreatePacket(string name) {
            this.name = name;
        }
    }
   
    private IEnumerator onAwake ()
    {
        UnityEngine.Debug.Log ("onAwake");
        /* TEST CODE */
        yield return GetData (testUrl,
            (result) => {
                TestResponse res = JsonUtility.FromJson<TestResponse> (result);
                UnityEngine.Debug.LogFormat ("message: {0}", res.message);
            }, true);

        /* TEST CODE END*/
        this.uuid = getUUID ();
        if (this.uuid == null) {
            //TODO 下のMockをLogin要求ダイアログにおきかえる
            string name = "a3128113ab922a093fa15971cad743f9";
            NameData post = new NameData (name);
            string body = JsonUtility.ToJson (post);
            yield return PostData (userUrl, body,
                (result) => {
                    Debug.Log(result);
                    UserData res = JsonUtility.FromJson<UserData> (result);
                    this.uuid = res.token;
                    SaveUUID(this.uuid);
                    this.headers.Add ("X-Token", this.uuid);
                }, true);
        } else {
            this.headers.Add("X-Token", this.uuid);
        }

        initializedFlag = true;
    }

    private IEnumerator PostTask(string name, int cost, DateTime time, HttpHandler handler) {
        PostTaskObj obj = new PostTaskObj (name, cost, time.ToString("o"));
        string body = JsonUtility.ToJson (obj);
        yield return PostData (taskUrl, body, handler);
    }

    public IEnumerator GetTasks(HttpHandler handler) {
        yield return GetData (taskUrl, handler);
    }

    public IEnumerator UpdateTask(int id, string name, int cost, DateTime time, int user_id, HttpHandler handler) {
        PutTaskObj obj = new PutTaskObj (id, name, cost, time.ToString ("o"), user_id);
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
