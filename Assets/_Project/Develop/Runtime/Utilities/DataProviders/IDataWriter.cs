using Assets._Project.Develop.Runtime.Utilities.DataManagment;

namespace Assets._Project.Develop.Runtime.Utilities.DataProviders
{
    public interface IDataWriter<TData> where TData : ISaveData
    {
        void WriteTo(TData data);
    }
}
