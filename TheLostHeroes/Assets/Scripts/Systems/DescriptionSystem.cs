using System.IO;
using Leopotam.Ecs;
using UnityEngine;

public struct DescriptionSystem : IEcsRunSystem
{
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject
    private RuntimeData runtimeData;    // подтягивается из Inject

    EcsFilter<DescriptionBeholder> filter;

    public void Run()
    {
        foreach (var id in filter)
        {
            ref var beholdedEntity = ref filter.GetEntity(id);
            ref var beholder = ref filter.Get1(id);

            if (beholdedEntity.Has<Room>())
            {
                beholder.describer.SetDescription(new DescriberBehavour.Description
                {
                    entityName = "Room",
                    entityDescription = "It's not very comfortable here",
                    actionButtons = new DescriberBehavour.IActionButton[] { },
                });
            }
            else if (beholdedEntity.Has<Pawn>())
            {
                var descriptionWriter = new StringWriter();

                descriptionWriter.WriteLine("Status: {0}", "ready to serve");

                if (beholdedEntity.Has<Health>())
                {
                    var hpComponent = beholdedEntity.Get<Health>();
                    descriptionWriter.WriteLine("HP: ({0}/{1})", hpComponent.hp, hpComponent.maxhp);
                }

                if (beholdedEntity.Has<Attack>())
                {
                    var attackComponent = beholdedEntity.Get<Attack>();
                    descriptionWriter.WriteLine("ATK: {0}", attackComponent.atk);
                }

                if (beholdedEntity.Has<Money>())
                {
                    var moneyComponent = beholdedEntity.Get<Money>();
                    if (moneyComponent.money < 800)
                    {
                        descriptionWriter.WriteLine("gold: {0}", moneyComponent.money);
                    }
                    else
                    {
                        descriptionWriter.WriteLine("gold: {0}K{1}", moneyComponent.money / 1000, (moneyComponent.money % 1000) / 100);
                    }

                }

                beholder.describer.SetDescription(new DescriberBehavour.Description
                {
                    entityName = "Sir Knight",
                    entityDescription = descriptionWriter.ToString(),
                    actionButtons = new DescriberBehavour.IActionButton[] { },
                });
            }
            else if (beholdedEntity.Has<Task>())
            {
                ref var taskComponent = ref beholdedEntity.Get<Task>();
                var descriptionWriter = new StringWriter();

                descriptionWriter.WriteLine("Reward: {0}", taskComponent.netFields.reward);
                descriptionWriter.WriteLine("interested: {0}", -1);
                int taskID = taskComponent.netFields.ID;

                // TODO: ограничить ресурсы игрока, вообще прописать изменение награды

                beholder.describer.SetDescription(new DescriberBehavour.Description
                {
                    entityName = "Task",
                    entityDescription = descriptionWriter.ToString(),
                    actionButtons = new DescriberBehavour.IActionButton[] {
                        new DescriberBehavour.SimpleActionButton("+100", delegate {
                            ref Task task = ref NetEntitySyncronizer.MustGetComponent<Task>(taskID);
                            task.netFields.reward += 100;
                            NetEntitySyncronizer.instance.EmitUpdate(taskID, new object[]{ task });
                        }),
                        new DescriberBehavour.SimpleActionButton("+500", delegate {
                            ref Task task = ref NetEntitySyncronizer.MustGetComponent<Task>(taskID);
                            task.netFields.reward += 500;
                            NetEntitySyncronizer.instance.EmitUpdate(taskID, new object[]{ task });
                        }),
                        new DescriberBehavour.SimpleActionButton("Remove", delegate {
                            ref Task task = ref NetEntitySyncronizer.MustGetComponent<Task>(taskID);
                            task.netFields.reward = 0;
                            NetEntitySyncronizer.instance.EmitUpdate(taskID, new object[]{ task });
                        }),
                    },
                });
            }

            break;
        }
    }
}