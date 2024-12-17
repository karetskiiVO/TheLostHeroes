using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;
using UnityEngine.AI;

using Photon.Pun;
public struct MasterInitSystem : IEcsInitSystem
{
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject

    private RuntimeData runtimeData;    // подтягивается из Inject

    EcsFilter<Room> roomFilter;
    public void Init()
    {
        foreach (int i in roomFilter)
        {
            ref EcsEntity entity = ref roomFilter.GetEntity(i);
            ref Room room = ref roomFilter.Get1(i);

            Pawn pawn = new Pawn();
            pawn.netFields.id = NetEntitySyncroniser.instance.nextID;
            pawn.netFields.speed = 1;
            pawn.netFields.atk = 10;
            pawn.netFields.x = room.netFields.posx;
            pawn.netFields.y = room.netFields.posy;
            Health health = new Health();
            health.hp = 100;
            Owned owned = new Owned();
            owned.owner = -1;
            KnightPawn tag = new KnightPawn();

            NetEntitySyncroniser.instance.EmitCreate(pawn.netFields.id, new object[] { pawn, health, owned, tag });
            NetEntitySyncroniser.instance.nextID++;
        }
    }
}
