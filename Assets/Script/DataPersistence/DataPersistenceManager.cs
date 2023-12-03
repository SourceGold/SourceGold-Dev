using Assets.Script.Backend;
using Assets.Script.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Script
{
    public class DataPersistenceManager
    {
        private List<IDataPersistence> _dataPersistenceObjects { get; set; }

        private string _rootDataPath { get; set; }

        private static DataPersistenceManager _instance;

        public static DataPersistenceManager Instance
        {
            get
            {
                return _instance ?? (_instance = new DataPersistenceManager());
            }
        }

        public DataPersistenceManager()
        {
            _rootDataPath = Application.persistentDataPath;
            _dataPersistenceObjects = new List<IDataPersistence>();
            if (_instance != null)
            {
                throw new InvalidOperationException("Found more than one Data Persistence Manager in the scene");
            }
        }

        // testing only
        public static string _testSave = "DefaultTestingSave";

        public static void AddDataPersistenceObject(IDataPersistence dataPersistence)
        {
            Instance._dataPersistenceObjects.Add(dataPersistence);
        }

        public void NewGame()
        {

        }

        public static void LoadGame(string saveName)
        {
            GameEventLogger.LogEvent("Loading Game", EventLogType.SystemEvent);
            foreach (IDataPersistence dataPersistenceObject in Instance._dataPersistenceObjects)
            {
                var fileName = $"{dataPersistenceObject.GetSaveFileName()}.yaml";
                var fullSaveFileName = Path.Combine(saveName, fileName);
                var fullPath = Path.Combine(Instance._rootDataPath, fullSaveFileName);
                dataPersistenceObject.LoadData(fullPath);
            }
        }

        public static void SaveGame(string saveName)
        {
            GameEventLogger.LogEvent("Saving Game", EventLogType.SystemEvent);
            foreach (IDataPersistence dataPersistenceObject in Instance._dataPersistenceObjects)
            {
                var fileName = $"{dataPersistenceObject.GetSaveFileName()}.yaml";
                var fullSaveFileName = Path.Combine(saveName, fileName);
                var fullPath = Path.Combine(Instance._rootDataPath, fullSaveFileName);
                dataPersistenceObject.SaveData(fullPath);
            }
        }

        public static void SaveDataFile<T>(string fileFullPath, T dataObject)
        {
            var path = Path.GetDirectoryName(fileFullPath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            SerializationUtils.WriteObjectAsYamlFile(fileFullPath, dataObject);
        }

        public static T LoadDataFile<T>(string fileFullPath)
        {
            return SerializationUtils.LoadObjectFromYamlFile<T>(fileFullPath);
        }
    }
}
