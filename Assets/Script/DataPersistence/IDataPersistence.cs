using System;

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

        /// <summary>
        /// Method to remove registration 
        /// of existing object from DataPersistenceManager when 
        /// the scene is destroyed or reloaded, must call in object's
        /// OnDestory method to avoid double registration when reload
        /// </summary>
        void Dispose()
        {
            DataPersistenceManager.RemoveDataPersistenceObject(this);
        }
    }
}
