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

    EcsFilter<Pawn> pawnFilter;
    public void Run()
    {
        foreach (int i in pawnFilter)
        {
            MovePawn(ref pawnFilter.Get1(i));
        }
    }

    public void MovePawn(ref Pawn pawn)
    {
        if (pawn.netFields.taskID != -1)
        {
            Vector3 TargetPos = NetEntitySyncroniser.MustGetComponent<Task>(pawn.netFields.taskID).instance.transform.position;
            Vector2 move = new Vector3(TargetPos.x - pawn.netFields.x, TargetPos.y - pawn.netFields.y, 0).normalized * pawn.netFields.speed;
            pawn.netFields.x += move.x;
            pawn.netFields.y += move.y;
            pawn.self.transform.Translate(move);
        }
    }
}
