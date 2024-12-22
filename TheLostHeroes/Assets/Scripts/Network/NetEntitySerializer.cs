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

    private class RegisterArg {
        public Type type;
        public SerializeMethod serializeMethod;
        public DeserializeMethod deserializeMethod;
    }

    public static void Register()
    {   
        var registerArgs = new RegisterArg[] {
            new () {
                type              = typeof(Room),
                serializeMethod   = SerializePartial, 
                deserializeMethod = DeserializePartial<Room>,
            },
            new () {
                type              = typeof(Task),
                serializeMethod   = SerializePartial, 
                deserializeMethod = DeserializePartial<Task>,
            },
            new () {
                type              = typeof(Pawn),
                serializeMethod   = SerializePartial, 
                deserializeMethod = DeserializePartial<Pawn>,
            },
            new () {
                type              = typeof(PawnIdle),
                serializeMethod   = SerializeFull, 
                deserializeMethod = DeserializeFull,
            },
            new () {
                type              = typeof(PawnGoing),
                serializeMethod   = SerializeFull, 
                deserializeMethod = DeserializeFull,
            },
            new () {
                type              = typeof(PawnWorking),
                serializeMethod   = SerializeFull, 
                deserializeMethod = DeserializeFull,
            },
            new () {
                type              = typeof(PawnDefending),
                serializeMethod   = SerializeFull, 
                deserializeMethod = DeserializeFull,
            },
            new () {
                type              = typeof(TaskAttack),
                serializeMethod   = SerializeFull, 
                deserializeMethod = DeserializeFull,
            },
            new () {
                type              = typeof(TaskDefend),
                serializeMethod   = SerializeFull, 
                deserializeMethod = DeserializeFull,
            },
            new () {
                type              = typeof(TaskWork),
                serializeMethod   = SerializeFull, 
                deserializeMethod = DeserializeFull,
            },
            new () {
                type              = typeof(PawnAttacking),
                serializeMethod   = SerializeFull, 
                deserializeMethod = DeserializeFull,
            },
            new () {
                type              = typeof(Health),
                serializeMethod   = SerializeFull, 
                deserializeMethod = DeserializeFull,
            },
            new () {
                type              = typeof(Attack),
                serializeMethod   = SerializeFull, 
                deserializeMethod = DeserializeFull,
            },
            new () {
                type              = typeof(Money),
                serializeMethod   = SerializeFull, 
                deserializeMethod = DeserializeFull,
            },
        };

        byte id = 255;

        foreach (var registerArg in registerArgs) {
            PhotonPeer.RegisterType(registerArg.type, id, registerArg.serializeMethod, registerArg.deserializeMethod);
            id--;
        }
    }
}
