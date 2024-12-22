using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

using Photon.Pun;
public struct PawnMoveSystem : IEcsRunSystem
{
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject

    private RuntimeData runtimeData;    // подтягивается из Inject

    EcsFilter<Pawn, PawnGoing> pawnFilter;
    public void Run()
    {
        foreach (int i in pawnFilter)
        {
            MovePawn(ref pawnFilter.Get1(i));
        }
    }

    public void MovePawn(ref Pawn pawn)
    {
        //TODO: поиск пути
        // if (!NetEntitySyncronizer.Alive(pawn.netFields.taskID))
        // {
        //     return;
        // }
        // pawn.netFields.x = pawn.self.transform.position.x;
        // pawn.netFields.y = pawn.self.transform.position.y;
    }
}
