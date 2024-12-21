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
        var gridTransform = GameObject.Find("Grid").transform;
        map.walkable_tilemap = gridTransform.Find("map_walkable").GetComponent<Tilemap>();
        map.obstacle_tilemap_front = gridTransform.Find("map_obstacle_front").GetComponent<Tilemap>();
        map.obstacle_tilemap_back = gridTransform.Find("map_obstacle_back").GetComponent<Tilemap>();
        map.sprites = gridTransform.GetComponent<SpriteContainer>().sprites;
    }
}
