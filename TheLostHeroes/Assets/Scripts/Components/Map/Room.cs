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
        public float posx;
        public float posy;
        public float sizex;
        public float sizey;
    }
    public Networked netFields;
    public Collider2D collider;


    public static byte[] Serialize(object obj)
    {
        byte[] bytes;
        IFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, ((Room)obj).netFields);
            bytes = stream.ToArray();
        }
        return bytes;
    }

    public static object Deserialize(byte[] data)
    {
        IFormatter formatter = new BinaryFormatter();
        Room res = new Room();
        using (MemoryStream stream = new MemoryStream(data))
        {
            res.netFields = (Networked)formatter.Deserialize(stream);
        }
        return res;
    }
}
