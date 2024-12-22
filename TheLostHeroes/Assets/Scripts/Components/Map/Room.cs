using System.Collections;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;

public struct Spawner
{
    // TODO:spawner properties
}
public struct Room
{
    [System.Serializable]
    public struct Networked
    {
        public int ID;
        public int ownerID;
        public float posx;
        public float posy;
        public float sizex;
        public float sizey;
    }
    public Networked netFields;
    public Collider2D collider;
}
