using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;
using UnityEngine.AI;

public struct WorldGenSystem : IEcsInitSystem {
    private EcsWorld ecsWorld;          // подтягивается автоматически, так как наследует EcsWorld
    private StaticData staticData;      // подтягивается из Inject
    private RuntimeData runtimeData;    // подтягивается из Inject

    public EcsFilter<Map> filter;

    public void Init() {
        foreach (var i in filter) {
            ref var mapEntity = ref filter.GetEntity(i);
            ref var mapComponent = ref filter.Get1(i);
            var mapRenderer = mapComponent.renderer;

            mapRenderer.sprite = Sprite.Create(
                staticData.gameResources.dungeonTiles,
                new Rect(0, 0, staticData.gameResources.dungeonTiles.width, staticData.gameResources.dungeonTiles.height),
                new Vector2()
            );

            Debug.Log("aboba");
        }
    }
}
