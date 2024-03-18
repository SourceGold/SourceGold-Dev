using Assets.Script.Backend;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

public class DescriptionLoader : Singleton<DescriptionLoader>
{
    public bool reloadAfterRestart = true;
    private string filename;

    private IDeserializer deserializer;

    public Dictionary<string, string> itemDescription;
    public Dictionary<string, string> itemName;

    private void Start()
    {
        var descriptionFile = Resources.Load<TextAsset>("TextDescription/ItemDescriptionEng");
        var nameFile  = Resources.Load<TextAsset>("TextDescription/ItemNamesEng");

        deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();

        itemDescription = deserializer.Deserialize<Dictionary<string, string>>(descriptionFile.text);
        itemName = deserializer.Deserialize<Dictionary<string, string>>(nameFile.text);
    }
}

