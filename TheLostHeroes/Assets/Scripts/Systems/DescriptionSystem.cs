using System.Diagnostics;
using System.IO;
using Leopotam.Ecs;
using UnityEditor.Search;

public struct DescriptionSystem : IEcsRunSystem {
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject
    private RuntimeData runtimeData;    // подтягивается из Inject

    EcsFilter<DescriptionBeholder> filter;

    private DescriberBehavour prevDescriber;

    public void Run () {
        bool updated = false;

        foreach (var id in filter) {
            ref var beholdedEntity = ref filter.GetEntity(id);
            ref var beholder = ref filter.Get1(id);
            
            if (beholdedEntity.Has<Room>()) {
                beholder.describer.SetDescription(new DescriberBehavour.Description{
                    entityName = "Room",
                    entityDescription = "It's not very comfortable here",
                    actionButtons = new DescriberBehavour.IActionButton[] {},
                });
            } 
            else if (beholdedEntity.Has<Pawn>()) {
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

                if (beholdedEntity.Has<Money>()) {
                    var moneyComponent = beholdedEntity.Get<Money>();
                    if (moneyComponent.money < 800) {
                        descriptionWriter.WriteLine("gold: {0}", moneyComponent.money);
                    } else {
                        descriptionWriter.WriteLine("gold: {0}K{1}", moneyComponent.money / 1000, (moneyComponent.money % 1000) / 100);
                    }
                    
                }

                beholder.describer.SetDescription(new DescriberBehavour.Description{
                    entityName = "Sir Knight",
                    entityDescription = descriptionWriter.ToString(),
                    actionButtons = new DescriberBehavour.IActionButton[] {},
                });
            }
            else if (beholdedEntity.Has<Task>()) {
                ref var taskComponent = ref beholdedEntity.Get<Task>();
                var descriptionWriter = new StringWriter();

                descriptionWriter.WriteLine("Reward: {0}", taskComponent.netFields.reward);
                descriptionWriter.WriteLine("interested: {0}", taskComponent.workers.Count);

                // TODO: ограничить ресурсы игрока, вообще прописать изменение награды

                beholder.describer.SetDescription(new DescriberBehavour.Description{
                    entityName = "Task",
                    entityDescription = descriptionWriter.ToString(),
                    actionButtons = new DescriberBehavour.IActionButton[] {
                        new DescriberBehavour.SimpleActionButton("+100", delegate {
                            
                        }),
                        new DescriberBehavour.SimpleActionButton("+500", delegate {
                            
                        }),
                        new DescriberBehavour.SimpleActionButton("Remove", delegate {
                            
                        }),
                    },
                });
            }

            prevDescriber = beholder.describer;
            updated = true;
            break;
        }

        if (!updated) {
            prevDescriber.SetDescription(new DescriberBehavour.Description{
                entityName = "The emptiness of the dungeon",
                entityDescription = "There was something here once, but the passage of time is merciless to all things.",
                actionButtons = new DescriberBehavour.IActionButton[] {},
            });
        }
    }
}