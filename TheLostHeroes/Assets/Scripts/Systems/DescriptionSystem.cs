using System.Collections.Generic;
using System.IO;
using Leopotam.Ecs;
using UnityEngine;

public struct DescriptionSystem : IEcsRunSystem, IEcsInitSystem
{
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject
    private RuntimeData runtimeData;    // подтягивается из Inject

    EcsFilter<DescriptionBeholder> filter;

    private DescriberBehavour prevDescriber;

    public void Init()
    {
        prevDescriber = null;
    }

    // TODO: добавить в описание игрока-владельца
    // TODO: добавить описание системы опыта
    public void Run()
    {
        bool updated = false;

        foreach (var id in filter)
        {
            ref var beholdedEntity = ref filter.GetEntity(id);
            ref var beholder = ref filter.Get1(id);

            if (beholdedEntity.Has<Room>())
            {
                string name = "";
                string description = "";

                var actionButtons = new List<DescriberBehavour.IActionButton>();
                if (beholdedEntity.Has<Barrack>())
                {
                    name = "Barrack";
                    description = "Two hundred thousand units are ready, and a million are on the way.";

                    actionButtons.Add(
                        new DescriberBehavour.SimpleActionButton("Recruit", delegate
                        {
                            EcsEntity self = NetEntitySyncronizer.GetEntity(id);
                            self.Get<RecruitRequest>();
                        })
                    );
                }
                else if (beholdedEntity.Has<Tavern>())
                {
                    name = "Tavern";
                    description = "Будь как дома, путник";
                }
                else if (beholdedEntity.Has<Mine>())
                {
                    name = "Mine";
                    description = "Eins, zwei, drei, vier, fünf, sechs, sieben, acht, neun, aus";
                }
                actionButtons.Add(
                    new DescriberBehavour.SimpleActionButton("Upgrade", delegate
                    {
                        EcsEntity self = NetEntitySyncronizer.GetEntity(id);
                        self.Get<UpgradeRequest>();
                    })
                );

                beholder.describer.SetDescription(new DescriberBehavour.Description
                {
                    entityName = name,
                    entityDescription = description,
                    actionButtons = actionButtons.ToArray(),
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
                descriptionWriter.WriteLine("interested: {0}", taskComponent.workers.Count);
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

            prevDescriber = beholder.describer;
            updated = true;
            break;
        }

        if (!updated)
        {
            prevDescriber?.SetDescription(new DescriberBehavour.Description
            {
                entityName = "The emptiness of the dungeon",
                entityDescription = "There was something here once, but the passage of time is merciless to all things.",
                actionButtons = new DescriberBehavour.IActionButton[] { },
            });
        }
    }
}