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
        // foreach (int i in roomFilter)
        // {
        //     ref EcsEntity entity = ref roomFilter.GetEntity(i);
        //     ref Room room = ref roomFilter.Get1(i);

        //     RequestSystem.makePawn(-1, 1, new Vector2(room.netFields.posx, room.netFields.posy));
        // }
    }
}
