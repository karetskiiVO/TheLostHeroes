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
            if (!NetEntitySyncronizer.Alive(pawn.netFields.taskID))
            {
                pawn.netFields.taskID = -1;
                NetEntitySyncronizer.instance.EmitRemoveTags(pawn.netFields.ID, new object[] { new PawnGoing() });
                NetEntitySyncronizer.instance.EmitAddTags(pawn.netFields.ID, new object[] { new PawnIdle() });
                NetEntitySyncronizer.instance.EmitUpdate(pawn.netFields.ID, new object[] { pawn });
                continue;
            }
            ref var task = ref NetEntitySyncronizer.MustGetComponent<Task>(pawn.netFields.taskID);
            ref var room = ref NetEntitySyncronizer.MustGetComponent<Room>(task.netFields.targetID);
            if (room.collider.bounds.Contains(new Vector3(pawn.self.transform.position.x, pawn.self.transform.position.y, room.collider.gameObject.transform.position.z)))
            {
                ReachedTask(ref pawn);
            }
        }
    }

    public void ReachedTask(ref Pawn pawn)
    {
        pawn.netFields.x = pawn.self.transform.position.x;
        pawn.netFields.y = pawn.self.transform.position.y;

        NetEntitySyncronizer.instance.EmitRemoveTags(pawn.netFields.ID, new object[] { new PawnGoing() });
        EcsEntity taskEntity = NetEntitySyncronizer.GetEntity(pawn.netFields.taskID);
        Debug.LogFormat("pawn {0} reached it's task", pawn.netFields.ID);
        if (taskEntity.Has<TaskAttack>())
        {
            NetEntitySyncronizer.instance.EmitAddTags(pawn.netFields.ID, new object[] { new PawnAttacking() });
        }
        else if (taskEntity.Has<TaskDefend>())
        {
            NetEntitySyncronizer.instance.EmitAddTags(pawn.netFields.ID, new object[] { new PawnDefending() });
        }
        else if (taskEntity.Has<TaskWork>())
        {
            NetEntitySyncronizer.instance.EmitAddTags(pawn.netFields.ID, new object[] { new PawnWorking() });
        }
        else
        {
            throw new System.Exception("task has no specification tag");
        }
        NetEntitySyncronizer.instance.EmitUpdate(pawn.netFields.ID, new object[] { pawn });
    }
}
