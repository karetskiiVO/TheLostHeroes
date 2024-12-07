using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;
using UnityEngine.AI;

public struct WorldGenSystem : IEcsInitSystem
{
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject
    private RuntimeData runtimeData;    // подтягивается из Inject

    public EcsFilter<Map> filter;

    public void Init() {
        foreach (var i in filter) {
            ref EcsEntity mapEntity = ref filter.GetEntity(i);

            // алгоритм генерации
        }
    }
}
