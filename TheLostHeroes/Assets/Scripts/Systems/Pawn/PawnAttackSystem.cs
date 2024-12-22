using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

using Photon.Pun;
public struct PawnAttackSystem : IEcsRunSystem
{
    EcsFilter<Pawn, PawnAttacking> pawnFilter;

    public void Run()
    {
        UpdateTasksStatus();
    }

    private void UpdateTasksStatus()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Dictionary<int, int> taskWorkersMapping = new Dictionary<int, int>();
            foreach (int i in pawnFilter)
            {
                ref var pawn = ref pawnFilter.Get1(i);
                if (!taskWorkersMapping.ContainsKey(pawn.netFields.taskID))
                {
                    taskWorkersMapping.Add(pawn.netFields.taskID, 1);
                }
                else
                {
                    ++taskWorkersMapping[pawn.netFields.taskID];
                }
            }
            foreach (var pair in taskWorkersMapping)
            {
                if (pair.Value > 1)
                {
                    ref var task = ref NetEntitySyncronizer.GetEntity(pair.Key).Get<Task>();
                    PawnTaskManagementSystem.FinishTask(ref task);
                }
            }
        }
    }
}
