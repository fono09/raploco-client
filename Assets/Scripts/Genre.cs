using UnityEngine;
using System;

[Serializable]
public class Genre
{
    public int id;
    public string name;

    public Genre (int id, string name) {
        this.id = id;
        this.name = name;
    }
}

[Serializable]
public class Genres
{
    public Genre[] genres;
}


[Serializable]
public class PostGenreCreatePacket 
{
    public string name;
    public PostGenreCreatePacket(string name) {
        this.name = name;
    }
}