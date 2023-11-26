namespace Assets.Script
{
    public abstract class DataPersistence : IDataPersistence
    {
        public DataPersistence()
        {
            ((IDataPersistence)this).RegisterExistence();
        }

        public abstract void LoadData(string fileName);

        public abstract void SaveData(string fileName);

        public abstract string GetSaveFileName();
    }
}
