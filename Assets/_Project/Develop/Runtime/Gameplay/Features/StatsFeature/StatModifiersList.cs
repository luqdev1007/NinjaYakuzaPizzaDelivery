using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Gameplay.Features.StatsFeature
{
    public class StatModifiersList
    {
        public event Action Changed;

        private readonly List<IStatModifier> _elements = new();

        public IReadOnlyList<IStatModifier> Elements => _elements;

        public void Add(IStatModifier modifier)
        {
            _elements.Add(modifier);

            Changed?.Invoke();
        }

        public void Remove(IStatModifier modifier)
        {
            _elements.Remove(modifier);

            Changed?.Invoke();
        }
    }
}