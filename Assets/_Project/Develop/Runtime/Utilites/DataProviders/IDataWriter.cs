using Assets._Project.Develop.Runtime.Utilites.DataManagment;

namespace Assets._Project.Develop.Runtime.Utilites.DataProviders
{
    public interface IDataWriter<TData> where TData : ISaveData
    {
        void WriteTo(TData data);
    }
}
