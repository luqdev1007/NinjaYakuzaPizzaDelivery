namespace Assets._Project.Develop.Runtime.Utilites.Conditions
{
    public class ManualCondition : ICondition
    {
        private readonly bool _value;
        public ManualCondition(bool value) => _value = value;
        public bool Evaluate() => _value;
    }
}
