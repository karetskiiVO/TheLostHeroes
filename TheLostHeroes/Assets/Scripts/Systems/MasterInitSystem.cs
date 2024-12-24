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

            var pawn = new Pawn();
            pawn.netFields.speed = 0.01f;
            pawn.netFields.x = room.netFields.posx;
            pawn.netFields.y = room.netFields.posy;
            pawn.netFields.ownerID = -1;
            pawn.netFields.taskID = -1;
            pawn.netFields.ID = NetEntitySyncronizer.instance.nextID;
            var health = new Health
            {
                hp = 5,
                maxhp = 5,
            };
            var attack = new Attack
            {
                atk = 1,
            };
            var money = new Money {
                money = 0,
            };
            var state = new PawnIdle();

            NetEntitySyncronizer.instance.EmitCreate(NetEntitySyncronizer.instance.nextID++, new object[] { pawn, health, attack, money, state });
        }
    }
}
