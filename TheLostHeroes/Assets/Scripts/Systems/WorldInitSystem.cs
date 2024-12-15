using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public struct WorldInitSystem : IEcsInitSystem
{
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject

    private RuntimeData runtimeData;    // подтягивается из Inject

    public void Init()
    {
        //Create a player who owns bots
        // runtimeData.envPlayer = ecsWorld.NewEntity();
        // runtimeData.envPlayer.Get<Player>().name = "Environment";

        //Create the map
        runtimeData.map = ecsWorld.NewEntity();
        ref var map = ref runtimeData.map.Get<Map>();
        var mapObject = GameObject.Find("map");
        map.tilemap = mapObject.GetComponent<Tilemap>();
        map.renderer = mapObject.GetComponent<TilemapRenderer>();
        map.sprites = mapObject.GetComponent<SpriteContainer>().sprites;

        //create a random pawn
        var pawnEntity = ecsWorld.NewEntity();

        ref var pawn = ref pawnEntity.Get<Pawn>();
        pawn.speed = 1;
        pawnEntity.Get<Health>().hp = 100;
        pawnEntity.Get<Attack>().atk = 10;
        pawnEntity.Get<Owned>().owner = runtimeData.envPlayer;
        pawnEntity.Get<KnightPawn>(); // и флаг что это рыцарь

        var pawnObject = Object.Instantiate(staticData.pawnPrefab, Vector3.zero, Quaternion.identity);
        pawn.agent = pawnObject.GetComponent<NavMeshAgent>();
    }
}
