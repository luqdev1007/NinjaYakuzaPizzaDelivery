namespace Assets._Project.Develop.Runtime.Utilities.DataManagment.KeyStorage
{
    public interface IDataKeysStorage
    {
        string GetKeyFor<Tdata>() where Tdata : ISaveData;
    }
}
