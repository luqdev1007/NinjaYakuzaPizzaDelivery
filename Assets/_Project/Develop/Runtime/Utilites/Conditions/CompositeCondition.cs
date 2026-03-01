using System;
using System.Collections.Generic;

namespace Assets._Project.Develop.Runtime.Utilites.Conditions
{
    public class CompositeCondition : ICompositeCondition
    {
        private List<ICondition> _conditions = new();
        private Func<bool, bool, bool> _standartLogicOperation;

        public CompositeCondition(Func<bool, bool, bool> standartLogicOperation)
        {
            _standartLogicOperation = standartLogicOperation;
        }

        public CompositeCondition() : this(LogicOperations.And)
        {
            
        }

        public ICompositeCondition Add(ICondition condition)
        {
            _conditions.Add(condition);

            return this;
        }

        public bool Evaluate()
        {
            if (_conditions.Count == 0)
                return false;

            bool result = _conditions[0].Evaluate();

            for (int i = 1; i < _conditions.Count; i++)
            {
                ICondition condition = _conditions[i];

                result = _standartLogicOperation.Invoke(result, condition.Evaluate());
            }

            return result;
        }

        public ICompositeCondition Remove(ICondition condition)
        {
           _conditions.Remove(condition);

            return this;
        }
    }
}
