using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;
using UnityEngine.AI;
using UnityEngine.Tilemaps;
using Photon.Pun;

public struct WorldInitSystem : IEcsInitSystem
{
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject

    private RuntimeData runtimeData;    // подтягивается из Inject

    public void Init()
    {
        NetEntitySyncroniser.instance.ecsWorld = ecsWorld;
        NetEntitySyncroniser.instance.staticData = staticData;
        //Create the map
        runtimeData.map = ecsWorld.NewEntity();
        ref var map = ref runtimeData.map.Get<Map>();
        var mapObject = GameObject.Find("map");
        map.tilemap = mapObject.GetComponent<Tilemap>();
        map.renderer = mapObject.GetComponent<TilemapRenderer>();
        map.sprites = mapObject.GetComponent<SpriteContainer>().sprites;
    }
}
