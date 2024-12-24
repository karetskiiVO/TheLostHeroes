


using Leopotam.Ecs;
using Photon.Pun;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public struct WinSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject
    private RuntimeData runtimeData;    // подтягивается из Inject

    EcsFilter<Room> roomFilter;
    bool finished;

    public void Init () {
        finished = false;
    }

    public void Run () {
        if (finished) return;

        var ownedByPlayer = 0;
        var ownedByOthers = 0;
        var roomCnt = 0;
        foreach (var idx in roomFilter) {
            var ownerID = roomFilter.Get1(idx).netFields.ownerID;

            if (ownerID == PhotonNetwork.LocalPlayer.ActorNumber) {
                ownedByPlayer++;
            } else if (ownerID != -1) {
                ownedByOthers++;
            }

            roomCnt++;
        }

        if (roomCnt == 0) return;

        if (ownedByPlayer == 0) {
            VictoryManager.Lose();
            finished = true;
            return;
        }

        if (ownedByOthers == 0) {
            VictoryManager.Win();
            finished = true;
            return;
        }
    }
}
