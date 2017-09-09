using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonMonoBehaviour<T>: MonoBehaviour where T : MonoBehaviour {
    private static T mInstance;

    public static T instance {
        get {
            if (SingletonMonoBehaviour<T>.mInstance == null) {
                SingletonMonoBehaviour<T>.mInstance = (T)((object)UnityEngine.Object.FindObjectOfType(typeof(T)));
            }
            return SingletonMonoBehaviour<T>.mInstance;
        }
    }

    public static bool IsEmpty() {
        return SingletonMonoBehaviour<T>.mInstance == null;
    }

    virtual public void Awake()
    {
        if (SingletonMonoBehaviour<T>.mInstance == null) {
            SingletonMonoBehaviour<T>.mInstance = (T)((object)UnityEngine.Object.FindObjectOfType (typeof(T)));
        }
        if (this != SingletonMonoBehaviour<T>.instance) {
            Destroy(this);
        }
    }
}
