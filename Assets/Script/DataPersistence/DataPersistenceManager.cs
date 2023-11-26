using Assets.Script.Backend;
using Assets.Script.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Script
{
    public class DataPersistenceManager : MonoBehaviour
    {
        private List<IDataPersistence> _dataPersistenceObjects { get; set; }

        private string _rootDataPath { get; set; }

        public static DataPersistenceManager Instance { get; private set; }

        private void Awake()
        {
            _rootDataPath = Application.persistentDataPath;
            _dataPersistenceObjects = new List<IDataPersistence>();
            DontDestroyOnLoad(this);
            if (Instance == null)
            {
                Instance = this;
                return;
            }
            throw new InvalidOperationException("Found more than one Data Persistence Manager in the scene");
        }

        // testing only
        private string _testSave = "DefaultTestingSave";

        public void AddDataPersistenceObject(IDataPersistence dataPersistence)
        {
            _dataPersistenceObjects.Add(dataPersistence);
        }

        public void NewGame()
        {

        }

        public void LoadGame(string saveName)
        {
            GameEventLogger.LogEvent("Loading Game", EventLogType.SystemEvent);
            foreach (IDataPersistence dataPersistenceObject in _dataPersistenceObjects)
            {
                var fileName = $"{dataPersistenceObject.GetSaveFileName()}.yaml";
                var fullSaveFileName = Path.Combine(saveName, fileName);
                var fullPath = Path.Combine(_rootDataPath, fullSaveFileName);
                dataPersistenceObject.LoadData(fullPath);
            }
        }

        public void SaveGame(string saveName)
        {
            GameEventLogger.LogEvent("Saving Game", EventLogType.SystemEvent);
            foreach (IDataPersistence dataPersistenceObject in _dataPersistenceObjects)
            {
                var fileName = $"{dataPersistenceObject.GetSaveFileName()}.yaml";
                var fullSaveFileName = Path.Combine(saveName, fileName);
                var fullPath = Path.Combine(_rootDataPath, fullSaveFileName);
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
