using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Task {
    public int id;
    public string name;
    public int cost;
    public string deadline;
    public int genre_id;
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
    public int genre_id;
    public PostTaskObj (string name, int cost, string deadline, int genre_id) {
        this.name = name;
        this.cost = cost;
        this.deadline = deadline;
        this.genre_id = genre_id;
    }
    public PostTaskObj (string name, int cost, DateTime date, int genre_id) {
        this.name = name;
        this.cost = cost;
        this.deadline = date.ToString("o");
        this.genre_id = genre_id;
    }
}

[Serializable]
public class PutTaskObj {
    public int id;
    public string name;
    public int cost;
    public string deadline;
    public int user_id;
    public int genre_id;
    public PutTaskObj (int id, string name, int cost, string deadline, int user_id, int genre_id) {
        this.id = id;
        this.name = name;
        this.cost = cost;
        this.deadline = deadline;
        this.user_id = this.user_id;
        this.genre_id = this.genre_id;
    }
}
