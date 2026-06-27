using System;
using System.Collections.Generic;
using System.Linq;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.BuffsFeature
{
    public class ActiveBuffsList
    {
        public event Action<ActiveBuff> Added;
        public event Action<ActiveBuff> Removed;

        private readonly List<ActiveBuff> _elements = new();

        public IReadOnlyList<ActiveBuff> Elements => _elements;

        public bool TryGetById(string id, out ActiveBuff activeBuff)
        {
            activeBuff = _elements.FirstOrDefault(buff => buff.Id == id);

            return activeBuff != null;
        }

        public void Add(ActiveBuff activeBuff)
        {
            _elements.Add(activeBuff);

            Added?.Invoke(activeBuff);
        }

        public void Remove(ActiveBuff activeBuff)
        {
            _elements.Remove(activeBuff);

            Removed?.Invoke(activeBuff);
        }
    }
}