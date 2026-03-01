using Assets._Project.Develop.Runtime.Utilites.DataManagment;

namespace Assets._Project.Develop.Runtime.Utilites.DataProviders
{
    public interface IDataReader<TData> where TData : ISaveData
    {
        void ReadFrom(TData data);
    }
}
