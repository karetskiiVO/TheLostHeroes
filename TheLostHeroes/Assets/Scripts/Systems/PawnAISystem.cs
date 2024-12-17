using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

using Photon.Pun;
public struct PawnAISystem : IEcsRunSystem
{
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject

    private RuntimeData runtimeData;    // подтягивается из Inject

    EcsFilter<Pawn> pawnFilter;
    public void Run()
    {
        //TODO:
    }
}
