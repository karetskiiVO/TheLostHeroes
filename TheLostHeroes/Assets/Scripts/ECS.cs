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
        };

        if (PhotonNetwork.IsMasterClient)
        {
            systems
                .Add(new WorldInitSystem())
                .Add(new WorldGenSystem())
                .Add(new MasterInitSystem())
                .Add(new PawnMoveSystem())
                .Add(new PawnAISystem())

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

                .Inject(configuration)
                .Inject(sceneData)
                .Inject(runtimeData)

                .Init();
        }
    }


    private void Update() 
    {
        HandleInput();
        systems?.Run();
    }

    private void HandleInput ()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            var clickPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            var hit = Physics2D.Raycast(clickPos, Vector2.zero);
            if (hit) {
                // TODO: понять а как перенести полученные результаты в ECS

                var hitedGameObject = hit.collider.gameObject;
            }
        }
    }

    private void OnDestroy()
    {
        systems?.Destroy();
        systems = null;
        ecsWorld?.Destroy();
        ecsWorld = null;
    }
}