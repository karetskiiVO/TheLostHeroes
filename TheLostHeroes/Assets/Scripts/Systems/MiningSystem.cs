using Leopotam.Ecs;
using Photon.Pun;
using UnityEngine;

public struct MiningSystem : IEcsRunSystem
{
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject
    private RuntimeData runtimeData;    // подтягивается из Inject

    private EcsFilter<PlayerClick, Mine, Room> miningFilter;
    private EcsFilter<Mine, Room> mineFilter;
    private EcsFilter<Barrack, Room> barrackFilter;
    private EcsFilter<Tavern, Room> tavernFilter;
    private EcsFilter<NewFrame> updateFilter;

    public void Run()
    {
        foreach (var idx in miningFilter)
        {
            var roomComponent = miningFilter.Get3(idx);
            if (roomComponent.netFields.ownerID != PhotonNetwork.LocalPlayer.ActorNumber)
                continue;

            var bounds = roomComponent.collider.bounds;
            AddMoney(bounds, Random.Range(2, 5));
        }

        foreach (var _ in updateFilter) {
            var val = 12;

            foreach (var idx in mineFilter) {
                var roomComponent = mineFilter.Get2(idx);
                if (roomComponent.netFields.ownerID != PhotonNetwork.LocalPlayer.ActorNumber)
                    continue;
                AddMoney(roomComponent.collider.bounds, val + Random.Range(0, (int)(val / 5)));
            }

            foreach (var idx in barrackFilter) {
                var roomComponent = barrackFilter.Get2(idx);
                if (roomComponent.netFields.ownerID != PhotonNetwork.LocalPlayer.ActorNumber)
                    continue;
                AddMoney(roomComponent.collider.bounds, val + Random.Range(0, (int)(val / 5)));
            }

            foreach (var idx in tavernFilter) {
                var roomComponent = tavernFilter.Get2(idx);
                if (roomComponent.netFields.ownerID != PhotonNetwork.LocalPlayer.ActorNumber)
                    continue;
                AddMoney(roomComponent.collider.bounds, 5 * val + Random.Range(0, val));
            }

            break;
        }
    }

    private void AddMoney (Bounds bounds, int val) {
        while (val > 0) {
            var buf = Random.Range(80, 100);

            MoneyCounter.AddMoney(
                System.Math.Min(val, buf),
                new Vector2 (
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y)
                )
            );
            val -= buf;
        }        
    }
}