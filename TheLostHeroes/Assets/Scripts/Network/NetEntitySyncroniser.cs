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

    void Awake()
    {
        instance = this;
        NetEntitySerializer.Register();
    }

    public void addComponent<T>(EcsEntity entity, object component) where T : struct
    {
        ref T comp = ref entity.Get<T>();
        comp = (T)component;
        if (comp.GetType() == typeof(Room))
        {
            Room room = (Room)(object)comp;
            var roomCollider = Instantiate(staticData.roomPrefab, new Vector3(room.netFields.posx, room.netFields.posy), Quaternion.identity).GetComponent<BoxCollider2D>();
            room.collider = roomCollider;
            roomCollider.size = new Vector2(room.netFields.sizex, room.netFields.sizey);
            comp = (T)(object)room;
        }
        if (comp.GetType() == typeof(Pawn))
        {
            Pawn pawn = (Pawn)(object)comp;
            var pawnObject = Instantiate(staticData.pawnPrefab, new Vector3(pawn.netFields.x, pawn.netFields.y), Quaternion.identity);
            pawn.self = pawnObject;
            comp = (T)(object)pawn;
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
            typeof(NetEntitySyncroniser).GetMethod("addComponent").MakeGenericMethod(components[i].GetType()).Invoke(this, new object[] { entity, components[i] });
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
}
