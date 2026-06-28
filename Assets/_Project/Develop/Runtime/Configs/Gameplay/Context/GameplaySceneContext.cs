using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Context
{
    public class GameplaySceneContext : MonoBehaviour
    {
        [field: Header("Cameras")]
        [field: SerializeField] public CinemachineCamera IntroCamera { get; private set; }
        [field: SerializeField] public CinemachineCamera ScoutingCamera { get; private set; }
        [field: SerializeField] public CinemachineCamera HeroCamera { get; private set; }

        [field: Header("Points")]
        [field: SerializeField] public Transform StartPoint { get; private set; }
        [field: SerializeField] public Transform FinishPoint { get; private set; }

        [Header("Read Only Data (Auto-filled)")]
        [SerializeField] private List<EnemySpawnData> _enemies = new();

        public IReadOnlyList<EnemySpawnData> Enemies => _enemies;

        public void SyncWithScene()
        {
            _enemies = GetComponentsInChildren<EnemySpawnMarker>(true)
                .Select(m => new EnemySpawnData(m.Config, m.transform.position, m.transform.rotation))
                .ToList();

            Debug.Log($"[SceneContext] Собрано врагов: {_enemies.Count}");
        }
    }
}