using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;
using UnityEngine.AI;

public struct WorldInitSystem : IEcsInitSystem
{
    private EcsWorld ecsWorld; //подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;//подтягивается из Inject

    private RuntimeData runtimeData;//подтягивается из Inject
    public void Init()
    {
        //Create a player who owns bots
        runtimeData.envPlayer = ecsWorld.NewEntity();
        runtimeData.envPlayer.Get<Player>().name = "Environment";

        //Create the map
        runtimeData.Map = ecsWorld.NewEntity();
        runtimeData.Map.Get<Map>();


        //create a random pawn
        EcsEntity pawnEntity = ecsWorld.NewEntity();

        ref var pawn = ref pawnEntity.Get<Pawn>();
        pawn.speed = 1;
        pawnEntity.Get<Health>().hp = 100;
        pawnEntity.Get<Attack>().atk = 10;
        pawnEntity.Get<Owned>().owner = runtimeData.envPlayer;
        pawnEntity.Get<KnightPawn>();//и флаг что это рыцарь

        GameObject pawnObject = Object.Instantiate(staticData.pawnPrefab, Vector3.zero, Quaternion.identity);
        pawn.agent = pawnObject.GetComponent<NavMeshAgent>();
    }
}
