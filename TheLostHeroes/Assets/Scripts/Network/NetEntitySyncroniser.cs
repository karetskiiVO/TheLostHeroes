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

public class NetEntitySyncroniser : MonoBehaviour
{
    public int nextID;
    public static NetEntitySyncroniser instance;
    public EcsWorld ecsWorld;
    public StaticData staticData;
    public Dictionary<int, EcsEntity> entities = new Dictionary<int, EcsEntity>();

    public static EcsEntity GetEntity(int id)
    {
        return instance.entities[id];
    }

    public static bool Alive(int id)
    {
        return instance.entities.ContainsKey(id);
    }

    public static ref T MustGetComponent<T>(int id) where T : struct
    {
        EcsEntity entity = instance.entities[id];
        if (!entity.Has<T>())
        {
            throw new Exception("entity with id " + id.ToString() + " expected to have component " + typeof(T).ToString() + ", but does not");
        }
        ref var res = ref instance.entities[id].Get<T>();
        return ref res;
    }

    void Awake()
    {
        instance = this;
        NetEntitySerializer.Register();
    }

    public void addComponent<T>(EcsEntity entity, object component, int id) where T : struct
    {
        ref T comp = ref entity.Get<T>();
        comp = (T)component;
        if (comp.GetType() == typeof(Room))
        {
            Room room = (Room)(object)comp;
            var roomCollider = Instantiate(staticData.roomPrefab, new Vector3(room.netFields.posx, room.netFields.posy), Quaternion.identity).GetComponent<BoxCollider2D>();
            room.collider = roomCollider;
            roomCollider.gameObject.GetComponent<NetIDHolder>().ID = id;
            roomCollider.size = new Vector2(room.netFields.sizex, room.netFields.sizey);
            comp = (T)(object)room;
        }
        if (comp.GetType() == typeof(Pawn))
        {
            Pawn pawn = (Pawn)(object)comp;
            var pawnObject = Instantiate(staticData.pawnPrefab, new Vector3(pawn.netFields.x, pawn.netFields.y, -1), Quaternion.identity);
            pawnObject.GetComponent<NetIDHolder>().ID = id;
            pawn.self = pawnObject;
            PawnNavigationAgent.Initialize(pawn);
            comp = (T)(object)pawn;
        }
        if (comp.GetType() == typeof(Task))
        {
            Task task = (Task)(object)comp;
            Room room = MustGetComponent<Room>(task.netFields.targetID);
            Vector3 pos = new Vector3(
                UnityEngine.Random.Range(room.netFields.posx - room.netFields.sizex / 2, room.netFields.posx + room.netFields.sizex / 2),
                UnityEngine.Random.Range(room.netFields.posy - room.netFields.sizey / 2, room.netFields.posy + room.netFields.sizey / 2),
                -2
            );
            var taskObject = Instantiate(staticData.taskPrefab, pos, Quaternion.identity);
            taskObject.GetComponent<NetIDHolder>().ID = id;
            taskObject.GetComponent<SpriteRenderer>().color = StaticData.GetColor(task.netFields.ownerID);
            task.instance = taskObject.GetComponent<NetIDHolder>();
            comp = (T)(object)task;
        }
    }

    public void updateComponent<T>(EcsEntity entity, object component) where T : struct
    {
        ref T comp = ref entity.Get<T>();
        T newComp = (T)component;
        typeof(T).GetField("netFields").SetValue(comp, typeof(T).GetField("netFields").GetValue(newComp));
        if (comp.GetType() == typeof(Pawn))
        {
            Pawn pawn = (Pawn)(object)comp;
            pawn.self.transform.position = new Vector3(pawn.netFields.x, pawn.netFields.y);
            PawnNavigationAgent.UpdateTarget(pawn);
        }
    }
    public void EmitCreate(int id, object[] components)
    {
        PhotonView.Get(this).RPC("CreateWithComponents", RpcTarget.All, new object[] { id, components });
    }

    public void EmitUpdate(int id, object[] components)
    {
        PhotonView.Get(this).RPC("UpdateComponents", RpcTarget.All, new object[] { id, components });
    }
    public void EmitDestroy(int id)
    {
        PhotonView.Get(this).RPC("DestroyEntity", RpcTarget.All, new object[] { id });
    }

    [PunRPC]
    public void CreateWithComponents(int id, object[] components)
    {
        EcsEntity entity = ecsWorld.NewEntity();

        for (int i = 0; i < components.Length; i++)
        {
            typeof(NetEntitySyncroniser).GetMethod("addComponent").MakeGenericMethod(components[i].GetType()).Invoke(this, new object[] { entity, components[i], id });
        }
        entities.Add(id, entity);
    }

    [PunRPC]
    public void UpdateComponents(int id, object[] components)
    {
        EcsEntity entity = entities[id];

        for (int i = 0; i < components.Length; i++)
        {
            typeof(NetEntitySyncroniser).GetMethod("updateComponent").MakeGenericMethod(components[i].GetType()).Invoke(this, new object[] { entity, components[i] });
        }

    }

    [PunRPC]
    public void DestroyEntity(int id)
    {
        EcsEntity entity = entities[id];
        entities.Remove(id);
        if (entity.Has<Task>())
        {
            Destroy(entity.Get<Task>().instance.gameObject);
        }
        entity.Destroy();
    }

    public void removeTag<T>(EcsEntity entity) where T : struct
    {
        entity.Del<T>();
    }
    public void EmitRemoveTags(int id, object[] tags)
    {
        PhotonView.Get(this).RPC("RemoveTags", RpcTarget.All, new object[] { id, tags });
    }
    [PunRPC]
    public void RemoveTags(int id, object[] tags)
    {
        EcsEntity entity = entities[id];

        for (int i = 0; i < tags.Length; i++)
        {
            typeof(NetEntitySyncroniser).GetMethod("removeTag").MakeGenericMethod(tags[i].GetType()).Invoke(this, new object[] { entity });
        }
    }

    public void addTag<T>(EcsEntity entity) where T : struct
    {
        entity.Get<T>();
    }
    public void EmitAddTags(int id, object[] tags)
    {
        PhotonView.Get(this).RPC("AddTags", RpcTarget.All, new object[] { id, tags });
    }
    [PunRPC]
    public void AddTags(int id, object[] tags)
    {
        EcsEntity entity = entities[id];

        for (int i = 0; i < tags.Length; i++)
        {
            typeof(NetEntitySyncroniser).GetMethod("addTag").MakeGenericMethod(tags[i].GetType()).Invoke(this, new object[] { entity });
        }

    }
}
