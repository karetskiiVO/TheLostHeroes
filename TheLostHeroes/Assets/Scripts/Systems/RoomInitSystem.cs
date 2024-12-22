using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

public struct RoomInitSystem : IEcsInitSystem {
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject
    private RuntimeData runtimeData;    // подтягивается из Inject
    
    EcsFilter<Room> roomFilter;

    public void Init () {
        var roomIndices = new List<int>();

        foreach (var idx in roomFilter) {
            roomIndices.Add(idx);
        }

        // заспавним три казармы и раздазим их игрокам

        int playersCnt = 2;
        Debug.Log("ДЛЯ RoomInitSystem ВВЕСТИ НОРМАЛЬНОЕ КОЛИЧЕСТВО");

        var playersIndices = new List<int>();
        while (playersIndices.Count != playersCnt) {
            var idx = roomIndices[runtimeData.randomConfiguration.Next(roomIndices.Count)];
            
            if (!playersIndices.Contains(idx)) playersIndices.Add(idx);
        }

        for (var playerIndex = 0; playerIndex < playersIndices.Count; playerIndex++) {
            // TODO: выставить id игроков
            var idx = roomIndices[runtimeData.randomConfiguration.Next(roomIndices.Count)];
            ref var roomComponent = ref roomFilter.Get1(idx);
            ref var roomEntity = ref roomFilter.GetEntity(idx);

            roomEntity.Get<Barrack>();
        }

        // оставшиемы комнаты как-нибудь разделим по типам

        foreach (var idx in roomFilter) {
            if (playersIndices.Contains(idx)) continue;
        
            ref var roomComponent = ref roomFilter.Get1(idx);
            ref var roomEntity = ref roomFilter.GetEntity(idx);

            switch (runtimeData.randomConfiguration.Next(3)) {
            case 0:
                roomEntity.Get<Barrack>();
                break;
            case 1:
                roomEntity.Get<Mine>();
                break;
            case 2:
                roomEntity.Get<Tavern>();
                break;
            }
        }
    }
}