using System.Collections;
using System.Collections.Generic;
using Leopotam.Ecs;
using UnityEngine;
using UnityEngine.AI;

using Photon.Pun;
using System;
using ExitGames.Client.Photon;

public class NetEntitySyncroniser : MonoBehaviour
{
    public static NetEntitySyncroniser instance;
    public EcsWorld ecsWorld;
    public StaticData staticData;
    Dictionary<int, EcsEntity> entities = new Dictionary<int, EcsEntity>();

    void Awake()
    {
        instance = this;
        PhotonPeer.RegisterType(typeof(Room), 255, Room.Serialize, Room.Deserialize);
    }

    public void addComponent<T>(EcsEntity entity, object component) where T : struct
    {
        ref T comp = ref entity.Get<T>();
        comp = (T)component;
        if (comp.GetType() == typeof(Room))
        {
            Room room = (Room)Convert.ChangeType(comp, typeof(Room));
            var roomCollider = Instantiate(staticData.roomPrefab, new Vector3(room.netFields.posx, room.netFields.posy), Quaternion.identity).GetComponent<BoxCollider2D>();
            room.collider = roomCollider;
            roomCollider.size = new Vector2(room.netFields.sizex, room.netFields.sizey);
        }
    }

    [PunRPC]
    public void Create(int id, object[] components)
    {
        EcsEntity entity = ecsWorld.NewEntity();

        for (int i = 0; i < components.Length; i++)
        {
            typeof(NetEntitySyncroniser).GetMethod("addComponent").MakeGenericMethod(components[i].GetType()).Invoke(this, new object[] { entity, components[i] });
        }
        entities.Add(id, entity);

    }
}
