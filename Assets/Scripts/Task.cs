using System;
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

    public Task (int id, string name, int cost, string deadline) {
        this.id = id;
        this.name = name;
        this.cost = cost;
        this.deadline = deadline;
    }
}