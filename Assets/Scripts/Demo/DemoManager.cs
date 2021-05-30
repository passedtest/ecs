using ECS;

/// TODO:
/// 1. MT Foreach parralel;
/// 2. Query iteration.
/// 3. Compoenent access by ref.
/// 4. Convenience initialization, like system via attribute, master systems.
/// 5. World state history buffer and replication system.
/// 6. Networking.
/// 7. Aysnc world save/loading;

using System;
using UnityEngine;
using Random = UnityEngine.Random;

[Serializable]
public struct EntitySetup
{
    public GameObject View;
    public AgentTeam Team;
    public float Speed;
    public float DestinationChangePeriod;
}

public class DemoManager : MonoBehaviour
{
    ECSWorld m_World;
    ECS.Core.ISystem m_MasterSystem;

    [SerializeField]
    EntitySetup[] m_BoidSettings;

    [SerializeField]
    int[] m_WorldCountPresets;

    void Start()
    {
        m_MasterSystem = new MasterSystem(
            new WorldTimeSystem(),
            new MovementSystem(), 
            new VIewInstanceSystem(m_BoidSettings),
            new VIewMovementSystem());

        BuildWorld(400);
    }

    void BuildWorld(int entityCount)
    {
        DestroyCurrentWorld();

        m_World = new ECSWorld();
        for (var i = 0; i < entityCount; i++)
            BuildBoidEnitity(m_World);
    }

    void DestroyCurrentWorld()
    {
        if (m_World != null)
            m_World.Destroy();

        m_World = null;
    }

    void BuildBoidEnitity(ECSWorld world)
    {
        var boidSettings = m_BoidSettings[Random.Range(0, m_BoidSettings.Length)];

        var builder = EntityLazyBuilder.New(world);

        builder += new BehaviourComponent()
        {
            Team = boidSettings.Team,
            Speed = boidSettings.Speed,
            DestinationChangePeriod = boidSettings.DestinationChangePeriod,
        };

        builder += new DestinationComponent()
        {
            Destination = Random.insideUnitSphere * 30f,
            NextDestinationUpdateTime = 0,
        };

        builder += new MoveComponent()
        {
            Position = Random.insideUnitSphere * 30f
        };

        builder.BuildNow();
    }

    void Update()
    {
        if (m_World != null)
            m_MasterSystem.ExecuteUpdate(m_World, Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (m_World != null)
            m_MasterSystem.ExecuteFixedUpdate(m_World, Time.fixedDeltaTime);
    }

    void OnDrawGizmos()
    {
        if (m_World != null)
            m_MasterSystem.ExecuteGizmoUpdate(m_World, Time.deltaTime);
    }

    bool m_Saving;
    bool m_Loading;
    ECS.Core.ComponentMapSnapshot m_Snapshot;

    void OnGUI()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        foreach (var entityCount in m_WorldCountPresets)
            if (GUILayout.Button($"{entityCount}"))
                BuildWorld(entityCount);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        if(m_Saving || m_Loading)
        {
            GUILayout.Label($"Async process in progress...");
        }
        else
        {
            if (GUILayout.Button("Load"))
            {
                m_Loading = true;
                SaveLoadText.LoadAsync(json =>
                {
                    m_Loading = false;
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        DestroyCurrentWorld();
                        m_World = new ECSWorld(JsonUtility.FromJson<WorldSnapshot>(json));
                    }
                    else
                        Debug.LogError("Nothing to load");
                });
            }

            if (m_World != null)
            {
                if (GUILayout.Button($"Destroy"))
                    DestroyCurrentWorld();

                if (GUILayout.Button($"Save"))
                {
                    var json = JsonUtility.ToJson(m_World.ToSnapshot());

                    m_Saving = true;
                    SaveLoadText.SaveAsync(json, () => m_Saving = false);

                    GUIUtility.systemCopyBuffer = json;
                }
            }
        }
        GUILayout.EndHorizontal();

        if (m_World != null)
        {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button($"Save Snapshot"))
                m_Snapshot = m_World.ToComponentMapSnapshot();

            if (m_Snapshot != null)
                if (GUILayout.Button($"Load Snapshot"))
                    m_World.UpdateFrom(m_Snapshot);

            GUILayout.EndHorizontal();

            if (m_Snapshot != null)
                GUILayout.Label($"Snapshot Count: {m_Snapshot.Count}");

            //GUILayout.Label($"Entities: {m_World.Entities.Count}");

            if (Single<WorldTimeComponent>.TryGet(m_World.UID, out var timeComponent))
                GUILayout.Label($"Time: {timeComponent.Time}");

            if (Single<WorldFixedTimeComponent>.TryGet(m_World.UID, out var fixedTimeComponent))
                GUILayout.Label($"FTime: {fixedTimeComponent.FixedTime}");

            GUILayout.Label($"Entities: {m_World.EntitiesCount}");

            DrawComponenetCount<WorldTimeComponent>(m_World);
            DrawComponenetCount<BehaviourComponent>(m_World);
            DrawComponenetCount<MoveComponent>(m_World);
            DrawComponenetCount<ViewComponent>(m_World);

            //foreach (var c in ECS.Core.SharedComponentMap.ComponentsForWorld(m_World.UID))
            //    GUILayout.Label($"{c.Key} : {c.Value.GetType().FullName}");

        }
        GUILayout.EndVertical();
    }

    void DrawComponenetCount<TComponent>(ECSWorld world) where TComponent : struct, IComponent
    {
        var components = world.GetComponentsWithEntityReadOnly<TComponent>();
        if (components != null)
            GUILayout.Label($"{typeof(TComponent).Name}: {components.Count}");
    }
}

