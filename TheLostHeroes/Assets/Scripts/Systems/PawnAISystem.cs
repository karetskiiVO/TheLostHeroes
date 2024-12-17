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
        foreach (int i in pawnFilter)
        {
            ref EcsEntity entity = ref pawnFilter.GetEntity(i);
            ref Pawn pawn = ref pawnFilter.Get1(i);
            pawn.netFields.x += Random.Range(-0.1f, 0.1f);
            pawn.netFields.y += Random.Range(-0.1f, 0.1f);

            PhotonView.Get(NetEntitySyncroniser.instance).RPC("UpdateComponents", RpcTarget.All, new object[] { pawn.netFields.id,
                    new object[] { pawn} });
        }
    }
}
