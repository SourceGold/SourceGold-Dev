using Assets.Script.Backend;
using Assets.Script.Utilities;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

/// <summary>
/// All scripting related with the consumable will be in this file
/// This controller is stateless. All information will be stored in 
/// GameItemDynamic
/// </summary>
public class ConsumableController : Singleton<ConsumableController>
{
    ConsumableConfig config;
    Inventory inventory;
    // Start is called before the first frame update
    void OnEnable()
    {
        // load the config file
        // var settingsInText = Resources.Load<TextAsset>("Configs/Consumable");
        IDeserializer deserializer;
        deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();
        var consumableYaml = Resources.Load<TextAsset>("Configs/Consumable");
        config = deserializer.Deserialize<ConsumableConfig>(consumableYaml.text);
        inventory = Backend.GameLoop.GetInventory();
    }

    public void activateItem(GameItemDynamic item)
    {
        string name = item.staticInfo.itemName;
        if (config.healingPotionRecoverHealth.ContainsKey(name))
        {
            item.additionalFloatStats["Recovery Health"] = config.healingPotionRecoverHealth[name];
            item.additionalIntStats["Recovery Time"] = 10;
            item.addtionalStatus.Add("Test");
        }
    }

    public void activateItem(GameItemDynamic item, Dictionary<string, string> additionalConfig)
    {
        string name = item.staticInfo.itemName;
        if (config.healingPotionRecoverHealth.ContainsKey(name))
        {
            if (additionalConfig.TryGetValue("recovery_health", out var recoveryHealth))
            {
                item.additionalFloatStats["Recovery Health"] = float.Parse(recoveryHealth);
           
            }
            else { 
                item.additionalFloatStats["Recovery Health"] = config.healingPotionRecoverHealth[name];
            }
            item.additionalIntStats["Recovery Time"] = 10;
            item.addtionalStatus.Add("Test");
        }
    }

    public void consumeItem(GameItemDynamic item)
    {
        if (item.additionalFloatStats.ContainsKey("Recovery Health"))
        {
            Backend.GameLoop.ProcessHealing((int)item.additionalFloatStats["Recovery Health"]) ;
        }
        inventory.RemoveItem(item, 1);
    }

    private void writeDictionaryAfterUpdate() 
    {
        config = new ConsumableConfig();
        config.healingPotionRecoverHealth = new Dictionary<string, float>();
        config.healingPotionRecoverHealth["Potion"] = 25.0f;
        SerializationUtils.WriteObjectAsYamlFile("Assets/Resources/Configs/Consumable.txt", config);
    }
}

public class ConsumableConfig
{
    public Dictionary<string, float> healingPotionRecoverHealth { get; set; }
}