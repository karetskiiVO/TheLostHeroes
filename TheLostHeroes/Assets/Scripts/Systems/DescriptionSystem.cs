using System.IO;
using Leopotam.Ecs;
using UnityEditor.Search;

public struct DescriptionSystem : IEcsRunSystem {
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject
    private RuntimeData runtimeData;    // подтягивается из Inject

    EcsFilter<DescriptionBeholder> filter;

    public void Run () {
        foreach (var idx in filter) {
            ref var beholdedEntity = ref filter.GetEntity(idx);
            ref var beholder = ref filter.Get1(idx);
            
            if (beholdedEntity.Has<Room>()) {
                beholder.describer.SetDescription(new DescriberBehavour.Description{
                    entityName = "Room",
                    entityDescription = "It's not very comfortable here",
                    actionButtons = new DescriberBehavour.IActionButton[] {},
                });
            } else if (beholdedEntity.Has<Pawn>()) {
                // TODO: hp/morale
                var descriptionWriter = new StringWriter();

                descriptionWriter.WriteLine("Status: {0}", "ready to serve");

                if (beholdedEntity.Has<Health>()) {
                    var hpComponent = beholdedEntity.Get<Health>();
                    descriptionWriter.WriteLine("HP: ({0}/{1})", hpComponent.hp, hpComponent.maxhp);
                }

                if (beholdedEntity.Has<Attack>()) {
                    var attackComponent = beholdedEntity.Get<Attack>();
                    descriptionWriter.WriteLine("ATK: {0}", attackComponent.atk);
                }


                beholder.describer.SetDescription(new DescriberBehavour.Description{
                    entityName = "Sir Knight",
                    entityDescription = descriptionWriter.ToString(),
                    actionButtons = new DescriberBehavour.IActionButton[] {},
                });
            } else if (beholdedEntity.Has<Task>()) {
                beholder.describer.SetDescription(new DescriberBehavour.Description{
                    entityName = "Task",
                    entityDescription = "Can't say anything",
                    actionButtons = new DescriberBehavour.IActionButton[] {},
                });
            }

            break;
        }
    }
}