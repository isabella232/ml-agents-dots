using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Random = UnityEngine.Random;
using Unity.AI.MLAgents;

public class BalanceBallManager : MonoBehaviour
{
    public MLAgentsWorldSpecs MyWorldSpecs;

    private EntityManager manager;
    public GameObject prefabPlatform;
    public GameObject prefabBall;
    private Entity _prefabEntityPlatform;
    private Entity _prefabEntityBall;
    int currentIndex;

    void Awake()
    {
        var world = MyWorldSpecs.GetWorld();
        var ballSystem = World.DefaultGameObjectInjectionWorld.GetOrCreateSystem<BallSystem>();
        ballSystem.Enabled = true;
        ballSystem.BallWorld = world;

        manager = World.DefaultGameObjectInjectionWorld.EntityManager;

        BlobAssetStore blob = new BlobAssetStore();
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blob);
        _prefabEntityPlatform = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabPlatform, settings);
        _prefabEntityBall = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabBall, settings);

        Spawn(1000);
        blob.Dispose();
    }

    void Spawn(int amount)
    {
        NativeArray<Entity> entitiesP = new NativeArray<Entity>(amount, Allocator.Temp);
        NativeArray<Entity> entitiesB = new NativeArray<Entity>(amount, Allocator.Temp);
        manager.Instantiate(_prefabEntityPlatform, entitiesP);
        manager.Instantiate(_prefabEntityBall, entitiesB);
        for (int i = 0; i < amount; i++)
        {
            float3 position = new float3((currentIndex % 10) - 5, (currentIndex / 10 % 10) - 5, currentIndex / 100) * 5f;
            float valX = Random.Range(-0.1f, 0.1f);
            float valZ = Random.Range(-0.1f, 0.1f);
            manager.SetComponentData(entitiesP[i],
                new Translation
                {
                    Value = position
                });
            manager.SetComponentData(entitiesB[i],
                new Translation
                {
                    Value = position + new float3(0, 0.2f, 0)
                });

            manager.SetComponentData(entitiesP[i],
                new Rotation
                {
                    Value = quaternion.EulerXYZ(valX, 0, valZ)
                });
            manager.AddComponent<AgentData>(entitiesP[i]);
            manager.SetComponentData(entitiesP[i], new AgentData { 
                BallResetPosition = position + new float3(0, 0.2f, 0),
                BallRef = entitiesB[i],
                StepCount = 0
             });
            manager.AddComponent<Actuator>(entitiesP[i]);
            currentIndex++;
        }

        entitiesP.Dispose();
        entitiesB.Dispose();
    }
}
