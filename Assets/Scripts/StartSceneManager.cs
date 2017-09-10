using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent (typeof(HTTPManager))]
public class StartSceneManager : SingletonMonoBehaviour<StartSceneManager> {

    [SerializeField]
    private GameObject registerPanel;

    private bool initialized = false;

    public void Update() {
        if (HTTPManager.instance.UuidIsNull ()) {
            if (!registerPanel.activeSelf) {
                registerPanel.SetActive (true);
                registerPanel.transform.Find ("Input/Button").GetComponent<Button> ().onClick.AddListener (() => {
                    string name = registerPanel.transform.Find("Input/InputField/Text").GetComponent<Text>().text;
                    registerName(name);
                });
            } 
        } else {
            initialized = true;
            if (registerPanel.activeSelf) {
                registerPanel.SetActive (false);
            }
        }

        if (initialized && (Input.touchCount > 0 || Input.GetMouseButtonDown(0))) {
            SceneManager.LoadScene ("Main");
        }
    }

    private void registerName(string name) {
        StartCoroutine (HTTPManager.instance.RegisterUser (name));
    }
}
