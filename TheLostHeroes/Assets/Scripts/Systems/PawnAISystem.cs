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
    EcsFilter<Task> taskFilter;
    public void Run()
    {
        foreach (int i in pawnFilter)
        {
            ref var pawn = ref pawnFilter.Get1(i);
            if (pawn.netFields.taskID != -1)
            {
                ref var task = ref NetEntitySyncroniser.MustGetComponent<Task>(pawn.netFields.taskID);
                ref var room = ref NetEntitySyncroniser.MustGetComponent<Room>(task.netFields.targetID);
                if (room.collider.bounds.Contains(new Vector3(pawn.netFields.x, pawn.netFields.y, room.collider.gameObject.transform.position.z)))
                {
                    ReachedTask(ref pawn);
                }
            }
            else
            {
                foreach (int j in taskFilter)
                {
                    ref var task = ref taskFilter.Get1(j);
                    if (task.netFields.ownerID != pawn.netFields.ownerID && task.netFields.ownerID != -1 && pawn.netFields.ownerID != -1)
                        continue;
                    if (Random.Range(0, 5) == 0)
                    {
                        pawn.netFields.taskID = task.netFields.ID;
                        NetEntitySyncroniser.instance.EmitUpdate(pawn.netFields.ID, new object[] { pawn });
                        break;
                    }
                }
            }
        }
    }

    public void ReachedTask(ref Pawn pawn)
    {
        //TODO:implement
        pawn.netFields.taskID = -1;
        NetEntitySyncroniser.instance.EmitUpdate(pawn.netFields.ID, new object[] { pawn });
    }
}
