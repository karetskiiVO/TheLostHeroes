using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

using Photon.Pun;
public struct PawnTaskManagementSystem : IEcsRunSystem
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
                    AssignTask(ref pawn, ref task);
                    break;
                }
            }
        }
    }

    public static void AssignTask(ref Pawn pawn, ref Task task)
    {
        Debug.LogFormat("pawn {0} found task {1}", pawn.netFields.ID, task.netFields.ID);
        pawn.netFields.taskID = task.netFields.ID;
        task.workers.Add(pawn.netFields.ID);
        NetEntitySyncronizer.instance.EmitRemoveTags(pawn.netFields.ID, new object[] { new PawnIdle() });
        NetEntitySyncronizer.instance.EmitAddTags(pawn.netFields.ID, new object[] { new PawnGoing() });
        NetEntitySyncronizer.instance.EmitUpdate(pawn.netFields.ID, new object[] { pawn });
    }

    public static void AbandonTask(ref Pawn pawn, ref Task task)
    {
        task.workers.Remove(pawn.netFields.ID);
        AbandonTaskImpl(ref pawn, ref task);
    }

    private static void AbandonTaskImpl(ref Pawn pawn, ref Task task)
    {
        pawn.netFields.taskID = -1;
        pawn.netFields.x = pawn.self.transform.position.x;
        pawn.netFields.y = pawn.self.transform.position.y;
        PawnNavigationAgent.UpdateTarget(pawn);

        NetEntitySyncronizer.instance.EmitRemoveTags(pawn.netFields.ID, new object[] {
            new PawnAttacking(),
            new PawnDefending(),
            new PawnWorking(),
        });
        NetEntitySyncronizer.instance.EmitAddTags(pawn.netFields.ID, new object[] { new PawnIdle() });

        NetEntitySyncronizer.instance.EmitUpdate(pawn.netFields.ID, new object[] { pawn });
        Debug.LogFormat("pawn {0} abandoned task {1}", pawn.netFields.ID, task.netFields.ID);
    }

    public static void FinishTask(ref Task task)
    {
        foreach (int workerID in task.workers)
        {
            ref var pawn = ref NetEntitySyncronizer.GetEntity(workerID).Get<Pawn>();
            AbandonTaskImpl(ref pawn, ref task);
        }
        Debug.LogFormat("task {0} is finished", task.netFields.ID);
        NetEntitySyncronizer.instance.EmitDestroy(task.netFields.ID);
    }
}
