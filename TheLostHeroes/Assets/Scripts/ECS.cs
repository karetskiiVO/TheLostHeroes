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
        RuntimeData runtimeData = new RuntimeData();

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