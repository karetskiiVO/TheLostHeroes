using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;
using UnityEngine.AI;

public struct WorldInitSystem : IEcsInitSystem
{
    private EcsWorld ecsWorld; //подтягивается автоматически
    private StaticData staticData;
    public void Init()
    {
        EcsEntity pawnEntity = ecsWorld.NewEntity();

        ref var pawn = ref pawnEntity.Get<Pawn>();
        ref var pawnPF = ref pawnEntity.Get<PawnPathfinding>();

        GameObject pawnObject = Object.Instantiate(staticData.pawnPrefab, Vector3.zero, Quaternion.identity);
        pawnPF.agent = pawnObject.GetComponent<NavMeshAgent>();
    }
}
