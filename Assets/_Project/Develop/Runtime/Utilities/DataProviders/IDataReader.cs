using Assets._Project.Develop.Runtime.Utilities.DataManagment;

namespace Assets._Project.Develop.Runtime.Utilities.DataProviders
{
    public interface IDataReader<TData> where TData : ISaveData
    {
        void ReadFrom(TData data);
    }
}
