using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leopotam.Ecs;

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

        systems
            // Системы с основной логикой должны
            // быть зарегистрированы здесь, порядок важен:
            // .Add (new TestSystem1 ())
            // .Add (new TestSystem2 ())

            // OneFrame-компоненты должны быть зарегистрированы
            // в общем списке систем, порядок важен:
            // .OneFrame<TestComponent1> ()
            // .OneFrame<TestComponent2> ()

            // Инъекция должна быть произведена здесь,
            // порядок не важен:
            // .Inject (new CameraService ())
            // .Inject (new NavMeshSupport ())

            .Add(new WorldInitSystem())
            .Add(new WorldGenSystem())
            .Add(new MasterInitSystem())
            .Add(new PawnMoveSystem())

            .Inject(configuration)
            .Inject(sceneData)
            .Inject(runtimeData)

            .Init();
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