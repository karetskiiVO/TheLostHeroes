using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

using Photon.Pun;

public class ECS : MonoBehaviour
{
    public StaticData configuration;
    public SceneData sceneData;
    public RuntimeData runtimeData;
    private EcsWorld ecsWorld;
    private EcsSystems systems;

    private void Start()
    {
        ecsWorld = new EcsWorld();
        systems = new EcsSystems(ecsWorld);
        runtimeData = new RuntimeData
        {
            randomConfiguration = new RandomConfiguration(sceneData.seed),
            defaultClickableBehavour = GetComponent<EmptyClickable>()
        };

        systems
            .Add(new WorldInitSystem())
            .Add(new WorldGenSystem());

        if (PhotonNetwork.IsMasterClient)
            systems
            .Add(new NetworkedUpdateSystem())
            .Add(new MasterInitSystem());

        systems
            .Add(new PawnMoveSystem());

        if (PhotonNetwork.IsMasterClient)
            systems
            .Add(new PawnTaskManagementSystem())
            .Add(new PawnFinishGoSystem());

        systems
            .Add(new RequestSystem())
            .Add(new PawnAttackSystem())
            .Add(new PawnWorkSystem())
            .Add(new PawnDefendSystem())
            .Add(new MouseClickSystem())

            .Add(new DescriptionSystem())
            .Add(new MiningSystem())
            
            .Add(new RoomInitSystem())

            .OneFrame<PlayerClick>()
            .OneFrame<RecruitRequest>()
            .OneFrame<UpgradeRequest>()
            .OneFrame<NewFrame>()

            .Inject(configuration)
            .Inject(sceneData)
            .Inject(runtimeData)

            .Init();
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