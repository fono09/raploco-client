using System;
using System.Collections;

[Serializable]
public class User
{
    public int id;
    public string name;
    public User (int id, string name) {
        this.id = id;
        this.name = name;
    }
}

