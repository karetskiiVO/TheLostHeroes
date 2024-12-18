using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

using Photon.Pun;
public struct PawnFinishGoSystem : IEcsRunSystem
{

    EcsFilter<Pawn, PawnGoing> pawnFilter;
    public void Run()
    {
        foreach (int i in pawnFilter)
        {
            ref var pawn = ref pawnFilter.Get1(i);
            if (!NetEntitySyncroniser.Alive(pawn.netFields.taskID))
            {
                pawn.netFields.taskID = -1;
                NetEntitySyncroniser.instance.EmitRemoveTags(pawn.netFields.ID, new object[] { new PawnGoing() });
                NetEntitySyncroniser.instance.EmitAddTags(pawn.netFields.ID, new object[] { new PawnIdle() });
                NetEntitySyncroniser.instance.EmitUpdate(pawn.netFields.ID, new object[] { pawn });
                continue;
            }
            ref var task = ref NetEntitySyncroniser.MustGetComponent<Task>(pawn.netFields.taskID);
            ref var room = ref NetEntitySyncroniser.MustGetComponent<Room>(task.netFields.targetID);
            if (room.collider.bounds.Contains(new Vector3(pawn.netFields.x, pawn.netFields.y, room.collider.gameObject.transform.position.z)))
            {
                ReachedTask(ref pawn);
            }
        }
    }

    public void ReachedTask(ref Pawn pawn)
    {
        NetEntitySyncroniser.instance.EmitRemoveTags(pawn.netFields.ID, new object[] { new PawnGoing() });
        EcsEntity taskEntity = NetEntitySyncroniser.GetEntity(pawn.netFields.taskID);
        if (taskEntity.Has<TaskAttack>())
        {
            NetEntitySyncroniser.instance.EmitAddTags(pawn.netFields.ID, new object[] { new PawnAttacking() });
        }
        else if (taskEntity.Has<TaskDefend>())
        {
            NetEntitySyncroniser.instance.EmitAddTags(pawn.netFields.ID, new object[] { new PawnDefending() });
        }
        else if (taskEntity.Has<TaskWork>())
        {
            NetEntitySyncroniser.instance.EmitAddTags(pawn.netFields.ID, new object[] { new PawnWorking() });
        }
        else
        {
            throw new System.Exception("task has no specification tag");
        }
    }
}
