using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;
using Photon.Pun;

public struct RoomInitSystem : IEcsRunSystem, IEcsInitSystem {
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject
    private RuntimeData runtimeData;    // подтягивается из Inject
    
    EcsFilter<Room> roomFilter;

    bool initiaised;

    public void Init () {
        initiaised = false;
    }

    public void Run () {
        if (initiaised) return;

        var roomIndices = new List<int>();

        foreach (var idx in roomFilter) {
            roomIndices.Add(idx);
        }

        if (roomIndices.Count == 0) return;

        // заспавним три казармы и раздазим их игрокам

        int playersCnt = PhotonNetwork.PlayerList.Length;

        var playersIndices = new List<int>();
        while (playersIndices.Count != playersCnt) {
            var buf = runtimeData.randomConfiguration.Next(roomIndices.Count);
            Debug.Log(roomIndices.Count);
            var idx = roomIndices[buf];
            
            if (!playersIndices.Contains(idx)) playersIndices.Add(idx);
        }

        for (var playerIndex = 0; playerIndex < playersIndices.Count; playerIndex++) {
            var idx = roomIndices[playersIndices[playerIndex]];
            ref var roomComponent = ref roomFilter.Get1(idx);
            ref var roomEntity = ref roomFilter.GetEntity(idx);

            roomComponent.netFields.ownerID = PhotonNetwork.PlayerList[playerIndex].ActorNumber;
            NetEntitySyncronizer.instance.EmitUpdate(roomComponent.netFields.ID, new object [] {roomComponent});

            roomEntity.Get<Barrack>();

            var roomPreset = RoomPresets.GetBarrack();
            roomPreset.transform.position = roomComponent.collider.bounds.center;
        }

        // оставшиемы комнаты как-нибудь разделим по типам

        foreach (var idx in roomFilter) {
            if (playersIndices.Contains(idx)) continue;

            ref var roomComponent = ref roomFilter.Get1(idx);
            ref var roomEntity = ref roomFilter.GetEntity(idx);

            GameObject roomPreset = null;
            switch (runtimeData.randomConfiguration.Next(3)) {
            case 0:
                roomPreset = RoomPresets.GetBarrack();
                roomEntity.Get<Barrack>();
                break;
            case 1:
                roomPreset = RoomPresets.GetMine();
                roomEntity.Get<Mine>();
                break;
            case 2:
                roomPreset = RoomPresets.GetTavern();
                roomEntity.Get<Tavern>();
                break;
            }

            roomPreset.transform.position = roomComponent.collider.bounds.center;
        }

        initiaised = true;
    }
}