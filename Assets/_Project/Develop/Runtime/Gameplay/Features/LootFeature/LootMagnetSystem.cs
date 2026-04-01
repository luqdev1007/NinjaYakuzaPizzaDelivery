using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore;
using Assets._Project.Develop.Runtime.Gameplay.EntitiesCore.Systems;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.LootFeature
{
    public class LootMagnetSystem : IInitializableSystem, IUpdatableSystem
    {
        private Entity _player;
        private readonly EntitiesLifeContext _lifeContext;
        private readonly float _magnetRange = 6f;

        public LootMagnetSystem(EntitiesLifeContext lifeContext) => _lifeContext = lifeContext;

        public void OnInit(Entity entity) => _player = entity;

        public void OnUpdate(float deltaTime)
        {
            if (_player == null) return;

            foreach (var loot in _lifeContext.Entities)
            {
                // Если нет тега, уже тянется или еще в процессе спавна (разлета) — пропускаем
                if (!loot.HasComponent<LootTag>() ||
                    loot.IsPullingProcess.Value ||
                    loot.InSpawnProcess.Value)
                    continue;

                float dist = Vector2.Distance(loot.Transform.position, _player.Transform.position);
                if (dist < _magnetRange)
                {
                    loot.CurrentTarget.Value = _player;
                    loot.IsPullingProcess.Value = true;

                    var rb = loot.Transform.GetComponent<Rigidbody2D>();
                    if (rb != null) rb.simulated = false;
                }
            }
        }
    }
}