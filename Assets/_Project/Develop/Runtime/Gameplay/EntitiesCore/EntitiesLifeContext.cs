using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets._Project.Develop.Runtime.Gameplay.EntitiesCore
{
    public class EntitiesLifeContext : IDisposable
    {
        public event Action<Entity> Added;
        public event Action<Entity> Released;

        private readonly List<Entity> _entities = new();
        private readonly List<Entity> _releaseRequests = new();

        public IReadOnlyList<Entity> Entities => _entities;

        public void Add(Entity entity)
        {
            _entities.Add(entity);

            entity.Initialize();

            Added?.Invoke(entity);
        }

        public void Update(float deltaTime)
        {
            for (int i = _entities.Count - 1; i >= 0; i--)
            {
                if (_entities[i] == null)
                {
                    _entities.RemoveAt(i);
                    continue;
                }
                try
                {
                    _entities[i].OnUpdate(deltaTime);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Entity [{i}] crashed: {e.StackTrace}");
                    _entities.RemoveAt(i);
                }
            }

            foreach (Entity entity in _releaseRequests)
            {
                _entities.Remove(entity);
                entity?.Dispose();
                Released?.Invoke(entity);
            }
            _releaseRequests.Clear();
        }

        public void Release(Entity entity)
        {
            _releaseRequests.Add(entity);
        }

        public void Dispose()
        {
            foreach (Entity entity in _entities)
                entity.Dispose();

            _entities.Clear();
            _releaseRequests.Clear();
        }
    }
}
