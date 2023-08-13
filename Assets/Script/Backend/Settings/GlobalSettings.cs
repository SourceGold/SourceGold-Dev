using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using static UnityEditor.SceneView;
using System;
using System.IO;
using UnityEngine.UIElements;
using Unity.VisualScripting.FullSerializer;
using static UnityEditor.Rendering.CameraUI;

namespace Assets.Script.Backend
{
    public class GlobalSettings : MonoBehaviour
    {
        public static GlobalSettings globalSettings;
        public static GlobalSettings instance 
        {
            get {
                if (!globalSettings)
                {
                    Debug.LogError("There needs to be one active GlobalSettings script on a GameObject in your scene.");
                }
               
                return globalSettings;
            }
        }

        public UserDefinedGameSettings userDefinedSettings;
        public bool alwaysLoadDefault;

        private IDeserializer deserializer;
        private ISerializer serializer;

        private string userSettingsFolderPath;
        private string userSettingsPath;
        private void Awake()
        {
            if (globalSettings != null)
            {
                Destroy(gameObject);
                return;
            }
            // end of new code

            globalSettings = this;
            DontDestroyOnLoad(gameObject);

            userSettingsFolderPath = Application.persistentDataPath + "/Configs";
            userSettingsPath = userSettingsFolderPath + "/UserDefinedConfigs.yml";

            if (!System.IO.File.Exists(userSettingsFolderPath))
            {
                Directory.CreateDirectory(userSettingsFolderPath);
            }

            deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();
            serializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();

            // Load User Config
            if (System.IO.File.Exists(userSettingsPath) && !alwaysLoadDefault)
            {
                LoadUserDefinedSettings();
            }
            else
            // If failed, load default config, then save it to user 
            {
                LoadUserDefinedDefaultSettings();
                SaveUserDefinedSettings();
            }
        }

        private void Start()
        {
            
        }

        // Make sure it will have the user settings before calling
        public void LoadUserDefinedSettings()
        {
            if (!System.IO.File.Exists(userSettingsPath))
            {
                throw new FileNotFoundException("User settings cannot be found when calling load user defined settings");
            }
            StreamReader reader = new StreamReader(userSettingsPath);
            var settingsInText = reader.ReadToEnd();
            reader.Close();
            userDefinedSettings = deserializer.Deserialize<UserDefinedGameSettings>(settingsInText);
        }

        public void SaveUserDefinedSettings()
        {
            var yaml = serializer.Serialize(userDefinedSettings);
            File.WriteAllText(userSettingsPath, yaml);
        }

        public void LoadUserDefinedDefaultSettings()
        {
            var settingsInText = Resources.Load<TextAsset>("Configs/defaultConfig");
            userDefinedSettings = deserializer.Deserialize<UserDefinedGameSettings>(settingsInText.text);
        }
    }



    public class UserDefinedGameSettings
    {
        [YamlMember(Alias = "gameplay_settings", ApplyNamingConventions = false)]
        public GamePlaySettings GamePlay { get; set; }
        
        [YamlMember(Alias = "control_settings", ApplyNamingConventions = false)]
        public ControlSettings Control  { get; set; }

        [YamlMember(Alias = "graphics_settings", ApplyNamingConventions = false)]
        public GraphicsSettings Graphics { get; set; }

        [YamlMember(Alias = "audio_settings", ApplyNamingConventions = false)]
        public AudioSettings Audio { get; set; }
    }
            
    

    public class GamePlaySettings
    {
        public difficulties difficulties { get; set; }
        public Dictionary<string, int>  SettingInt { get; set; }
        public Dictionary<string, bool> SettingBool { get; set; }
    }
    public class ControlSettings
    {
        public bool PressToSpeedUp { get; set; }
        public bool RevertCameraMovements { get; set; }
        public float MouseSensitivity { get; set; }
        public Dictionary<string, int> SettingInt { get; set; }
        public Dictionary<string, bool> SettingBool { get; set; }
    }
    public class GraphicsSettings
    {
        public visualQuality VisualQuality { get; set; }
        public Dictionary<string, int> SettingInt { get; set; }
        public Dictionary<string, bool> SettingBool { get; set; }
    }
    public class AudioSettings
    {
        public Dictionary<string, int> SettingInt { get; set; }
        public Dictionary<string, bool> SettingBool { get; set; }
    }

    public enum visualQuality
    {
        poor = 0,
        medium = 1,
        high = 2,
        extreme = 3
    }

    public enum difficulties
    {
        easy = 0,
        medium = 1,
        hard = 2,
    }
}
