using Leopotam.Ecs;

public struct MiningSystem : IEcsRunSystem, IEcsInitSystem {
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject
    private RuntimeData runtimeData;    // подтягивается из Inject

    private EcsFilter<PlayerClick, Mine, Room> miningFilter;
    private PlayerMoney playerMoney;

    public void Init () {
        playerMoney = runtimeData.playerMoney;
    }

    public void Run () {
        foreach (var idx in miningFilter) {
            // TODO: проверить что это клик владельца и учесть уровень

            var mineComponent = miningFilter.Get2(idx);
            var roomComponent = miningFilter.Get2(idx);

            playerMoney.money += 97;
        }
    }
}