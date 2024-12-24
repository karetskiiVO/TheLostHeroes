using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.AI;
using System.Text;


using Photon.Pun;
using System;
using ExitGames.Client.Photon;
using System.Linq;

public class NetEntitySerializer
{
    public static byte[] SerializePartial(object obj)
    {
        return Encoding.ASCII.GetBytes(JsonUtility.ToJson(obj.GetType().GetField("netFields").GetValue(obj)));
    }

    public static object DeserializePartial<T>(byte[] data) where T : new()
    {
        object res = new T();
        var method = typeof(JsonUtility).GetMethods().Where(x => x.Name == "FromJson").Where(x => x.IsGenericMethod).FirstOrDefault().MakeGenericMethod(typeof(T).GetField("netFields").FieldType);
        var value = method.Invoke(null, new object[] { Encoding.ASCII.GetString(data) });
        typeof(T).GetField("netFields").SetValue(res, value);
        return (T)res;
    }

    public static byte[] SerializeFull(object obj)
    {
        return Encoding.ASCII.GetBytes(JsonUtility.ToJson(obj));
    }

    public static object DeserializeFull<T>(byte[] data)
    {
        return JsonUtility.FromJson<T>(Encoding.ASCII.GetString(data));
    }

    private class RegisterArg
    {
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
                deserializeMethod = DeserializeFull<PawnIdle>,
            },
            new () {
                type              = typeof(PawnGoing),
                serializeMethod   = SerializeFull,
                deserializeMethod = DeserializeFull<PawnGoing>,
            },
            new () {
                type              = typeof(PawnWorking),
                serializeMethod   = SerializeFull,
                deserializeMethod = DeserializeFull<PawnWorking>,
            },
            new () {
                type              = typeof(PawnDefending),
                serializeMethod   = SerializeFull,
                deserializeMethod = DeserializeFull<PawnDefending>,
            },
            new () {
                type              = typeof(TaskAttack),
                serializeMethod   = SerializeFull,
                deserializeMethod = DeserializeFull<TaskAttack>,
            },
            new () {
                type              = typeof(TaskDefend),
                serializeMethod   = SerializeFull,
                deserializeMethod = DeserializeFull<TaskDefend>,
            },
            new () {
                type              = typeof(TaskWork),
                serializeMethod   = SerializeFull,
                deserializeMethod = DeserializeFull<TaskWork>,
            },
            new () {
                type              = typeof(PawnAttacking),
                serializeMethod   = SerializeFull,
                deserializeMethod = DeserializeFull<PawnAttacking>,
            },
            new () {
                type              = typeof(Health),
                serializeMethod   = SerializeFull,
                deserializeMethod = DeserializeFull<Health>,
            },
            new () {
                type              = typeof(Attack),
                serializeMethod   = SerializeFull,
                deserializeMethod = DeserializeFull<Attack>,
            },
            new () {
                type              = typeof(Money),
                serializeMethod   = SerializeFull,
                deserializeMethod = DeserializeFull<Money>,
            },
        };

        byte id = 255;

        foreach (var registerArg in registerArgs)
        {
            PhotonPeer.RegisterType(registerArg.type, id, registerArg.serializeMethod, registerArg.deserializeMethod);
            id--;
        }
    }
}
