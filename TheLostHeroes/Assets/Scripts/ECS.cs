using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

using Photon.Pun;

public class ECS : MonoBehaviour
{
    public StaticData configuration;
    public SceneData sceneData;
    private EcsWorld ecsWorld;
    private EcsSystems systems;

    private void Start()
    {
        ecsWorld = new EcsWorld();
        systems = new EcsSystems(ecsWorld);
        RuntimeData runtimeData = new RuntimeData
        {
            randomConfiguration = new RandomConfiguration(sceneData.seed),
            defaultClickableBehavour = GetComponent<EmptyClickable>()
        };

        if (PhotonNetwork.IsMasterClient)
        {
            systems
                .Add(new WorldInitSystem())
                .Add(new WorldGenSystem())
                .Add(new MasterInitSystem())
                .Add(new PawnMoveSystem())
                .Add(new PawnChooseTaskSystem())
                .Add(new PawnFinishGoSystem())
                .Add(new PawnAttackSystem())
                .Add(new PawnWorkSystem())
                .Add(new PawnDefendSystem())
                .Add(new MouseClickSystem())

                .Add(new SetDesctriptionSystem())
                .Add(new DescriptionSystem())

                .Inject(configuration)
                .Inject(sceneData)
                .Inject(runtimeData)

                .Init();
        }
        else
        {
            systems
                .Add(new WorldInitSystem())
                .Add(new WorldGenSystem())
                .Add(new PawnMoveSystem())
                .Add(new MouseClickSystem())
                .Add(new PawnAttackSystem())
                .Add(new PawnWorkSystem())
                .Add(new PawnDefendSystem())

                .Add(new SetDesctriptionSystem())
                .Add(new DescriptionSystem())
                
                .Inject(configuration)
                .Inject(sceneData)
                .Inject(runtimeData)

                .Init();
        }
    }


    private void Update()
    {
        systems?.Run();
    }

    private void OnDestroy()
    {
        systems?.Destroy();
        systems = null;
        ecsWorld?.Destroy();
        ecsWorld = null;
    }
}