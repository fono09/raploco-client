using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Task {
    public int id;
    public string name;
    public int cost;
    public string deadline;
    public DateTime DeadlineTime {
        get {
            return Convert.ToDateTime (deadline);
        }
    }

    public DateTime DateTime { get; internal set; }

    public Task (int id, string name, int cost, string deadline) {
        this.id = id;
        this.name = name;
        this.cost = cost;
        this.deadline = deadline;
    }
}

[Serializable]
public class PostTaskObj {
    public string name;
    public int cost;
    public string deadline;
    public PostTaskObj (string name, int cost, string deadline) {
        this.name = name;
        this.cost = cost;
        this.deadline = deadline;
    }
}

[Serializable]
public class PutTaskObj {
    public int id;
    public string name;
    public int cost;
    public string deadline;
    public int user_id;
    public PutTaskObj (int id, string name, int cost, string deadline, int userid) {
        this.name = name;
        this.cost = cost;
        this.deadline = deadline;
        this.user_id = userid;
    }
}
