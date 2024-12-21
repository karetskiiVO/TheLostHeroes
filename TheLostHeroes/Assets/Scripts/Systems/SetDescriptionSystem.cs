using Leopotam.Ecs;
using UnityEditor.Search;

public struct SetDesctriptionSystem : IEcsRunSystem {
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject
    private RuntimeData runtimeData;    // подтягивается из Inject

    EcsFilter<SetDescriptionBeholder> setupFilter;
    EcsFilter<DescriptionBeholder> beholderedFilter;

    public void Run () {
        foreach (var newBeholdIdx in setupFilter) {
            foreach (var beholdIdx in beholderedFilter) {
                ref var beholdedEntity = ref beholderedFilter.GetEntity(beholdIdx);

                beholdedEntity.Del<DescriptionBeholder>();
            }

            ref var newBeholdedEntity = ref beholderedFilter.GetEntity(newBeholdIdx);
            ref var descriptionBeholder = ref newBeholdedEntity.Get<DescriptionBeholder>();
            descriptionBeholder.describer = beholderedFilter.GetEntity(newBeholdIdx).Get<SetDescriptionBeholder>().describer;
            break;
        }
    }
}