using Leopotam.Ecs;
using UnityEditor.Search;

public struct DescriptionSystem : IEcsRunSystem {
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject
    private RuntimeData runtimeData;    // подтягивается из Inject

    public void Run () {
        // TODO: сделать отправку данных
    }
}