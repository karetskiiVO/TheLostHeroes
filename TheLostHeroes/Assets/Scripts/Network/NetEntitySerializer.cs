using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.AI;

using Photon.Pun;
using System;
using ExitGames.Client.Photon;

public class NetEntitySerializer
{
    public static byte[] SerializePartial(object obj)
    {
        byte[] bytes;
        IFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, obj.GetType().GetField("netFields").GetValue(obj));
            bytes = stream.ToArray();
        }
        return bytes;
    }

    public static object DeserializePartial<T>(byte[] data) where T : new()
    {
        IFormatter formatter = new BinaryFormatter();
        T res = new T();
        using (MemoryStream stream = new MemoryStream(data))
        {
            typeof(T).GetField("netFields").SetValue(res, formatter.Deserialize(stream));
        }
        return res;
    }

    public static byte[] SerializeFull(object obj)
    {
        byte[] bytes;
        IFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream())
        {
            formatter.Serialize(stream, obj);
            bytes = stream.ToArray();
        }
        return bytes;
    }

    public static object DeserializeFull(byte[] data)
    {
        IFormatter formatter = new BinaryFormatter();
        using (MemoryStream stream = new MemoryStream(data))
        {
            return formatter.Deserialize(stream);
        }
    }
    public static void Register()
    {
        PhotonPeer.RegisterType(typeof(Room), 255, SerializePartial, DeserializePartial<Room>);
        PhotonPeer.RegisterType(typeof(Health), 253, SerializeFull, DeserializeFull);
        PhotonPeer.RegisterType(typeof(Task), 252, SerializePartial, DeserializePartial<Task>);
        PhotonPeer.RegisterType(typeof(Pawn), 251, SerializePartial, DeserializePartial<Pawn>);

        PhotonPeer.RegisterType(typeof(PawnIdle), 254, SerializeFull, DeserializeFull);
        PhotonPeer.RegisterType(typeof(PawnGoing), 250, SerializeFull, DeserializeFull);
        PhotonPeer.RegisterType(typeof(PawnWorking), 249, SerializeFull, DeserializeFull);
        PhotonPeer.RegisterType(typeof(PawnDefending), 248, SerializeFull, DeserializeFull);
        PhotonPeer.RegisterType(typeof(TaskAttack), 247, SerializeFull, DeserializeFull);
        PhotonPeer.RegisterType(typeof(TaskDefend), 246, SerializeFull, DeserializeFull);
        PhotonPeer.RegisterType(typeof(TaskWork), 245, SerializeFull, DeserializeFull);
        PhotonPeer.RegisterType(typeof(PawnAttacking), 244, SerializeFull, DeserializeFull);
    }
}
