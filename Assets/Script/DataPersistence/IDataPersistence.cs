namespace Assets.Script
{
    public interface IDataPersistence
    {
        void LoadData(string fileName);

        void SaveData(string fileName);

        string GetSaveFileName();

        /// <summary>
        /// Method to register the existence object to
        /// DataPersistenceManager, any data want to be
        /// saved/loaded must implement this method and
        /// call it at appropriate time
        /// </summary>
        void RegisterExistence()
        {
            DataPersistenceManager.AddDataPersistenceObject(this);
        }
    }
}
