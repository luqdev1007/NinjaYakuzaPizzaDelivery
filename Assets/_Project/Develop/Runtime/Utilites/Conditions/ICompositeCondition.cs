using Unity.VisualScripting;

namespace Assets._Project.Develop.Runtime.Utilites.Conditions
{
    public interface ICompositeCondition : ICondition
    {
        ICompositeCondition Add(ICondition condition);
        ICompositeCondition Remove(ICondition condition);
    }
}
