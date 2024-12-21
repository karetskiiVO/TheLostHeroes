using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

using Photon.Pun;
public struct PawnChooseTaskSystem : IEcsRunSystem
{
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject

    private RuntimeData runtimeData;    // подтягивается из Inject

    EcsFilter<Pawn, PawnIdle> pawnFilter;//TODO: пока что ищут работу только idle пешки, по факту другие в теории могут бросить текущую работу
    EcsFilter<Task> taskFilter;
    public void Run()
    {
        foreach (int i in pawnFilter)
        {

            ref var pawn = ref pawnFilter.Get1(i);
            foreach (int j in taskFilter)
            {
                ref var task = ref taskFilter.Get1(j);
                if (task.netFields.ownerID != pawn.netFields.ownerID && task.netFields.ownerID != -1 && pawn.netFields.ownerID != -1)
                    continue;
                if (Random.Range(0, 5) == 0)
                {
                    Debug.Log("pawn found task");
                    pawn.netFields.taskID = task.netFields.ID;
                    NetEntitySyncronizer.instance.EmitRemoveTags(pawn.netFields.ID, new object[] { new PawnIdle() });
                    NetEntitySyncronizer.instance.EmitAddTags(pawn.netFields.ID, new object[] { new PawnGoing() });
                    NetEntitySyncronizer.instance.EmitUpdate(pawn.netFields.ID, new object[] { pawn });
                    break;
                }
            }
        }
    }
}
