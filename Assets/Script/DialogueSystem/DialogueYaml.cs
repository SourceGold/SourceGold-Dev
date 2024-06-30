using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using Unity.VisualScripting.FullSerializer;
using UnityEngine.UIElements;
using System.IO;
using Assets.Script.Backend;
using Unity.VisualScripting;
using System.Diagnostics.Tracing;
using System;

public class DialogueYmlFile
{
    public List<DialogueNodeYml> ListOfNodes;
    public bool checkNodes(List<string> plausibleActions = null)
    {
        if (ListOfNodes == null || ListOfNodes.Count == 0)
        {
            throw new Exception("ListOfNodes cannot be null or empty");
        }

        List<string> available_node = new List<string> { "null" };
        HashSet<string> uniqueNodeNames = new HashSet<string>();

        // First pass: populate available_node and check for unique node names
        foreach (var node in ListOfNodes)
        {
            if (string.IsNullOrWhiteSpace(node.NodeName))
            {
                throw new Exception("Node name cannot be null or whitespace");
            }

            if (!uniqueNodeNames.Add(node.NodeName))
            {
                throw new Exception($"Duplicate node name found: {node.NodeName}");
            }

            available_node.Add(node.NodeName);
        }

        // Second pass: perform checks
        foreach (var node in ListOfNodes)
        {
            if (!Enum.IsDefined(typeof(DialogueNodeYmlType), node.Type))
            {
                throw new Exception($"Node name {node.NodeName} has an invalid node type");
            }

            if (node.Type == DialogueNodeYmlType.normal)
            {
                if (node.NextNodes.Count > 1)
                {
                    throw new Exception($"Node name {node.NodeName} is type normal but has more than 1 next node");
                }
            }
            else if (node.Type == DialogueNodeYmlType.choice)
            {
                if (node.DialogueContent.Count == 0)
                {
                    throw new Exception($"Node name {node.NodeName} is type choice but has empty DialogueContent");
                }
                if (node.NextNodes.Count != node.DialogueContent.Count)
                {
                    throw new Exception($"Node name {node.NodeName} is type choice but choice count does not match next node count");
                }
            }
            else if (node.Type == DialogueNodeYmlType.codeEvent)
            {
                if (node.DialogueContent.Count == 0)
                {
                    throw new Exception($"Node name {node.NodeName} is type codeEvent but has empty DialogueContent");
                }
                if (plausibleActions == null)
                {
                    throw new Exception($"Node name {node.NodeName} defined code action but no action is available");
                }
                foreach (var action in node.DialogueContent)
                {
                    if (!plausibleActions.Contains(action))
                    {
                        throw new Exception($"Node name {node.NodeName} contains invalid action '{action}' which is not in plausible actions list");
                    }
                }
            }

            foreach (var nextNode in node.NextNodes)
            {
                if (!available_node.Contains(nextNode))
                {
                    throw new Exception($"Node name {node.NodeName} has an undefined next node '{nextNode}'");
                }
            }
        }

        return true;
    }

    static public void writeSampleYaml()
    {
        ISerializer serializer;
        serializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();

        List<string> content = new List<string>();
        content.Add("this is cool");
        content.Add("this is not cool");

        List<string> next_node = new List<string>();
        next_node.Add("cool");
        next_node.Add("not cool");

        DialogueNodeYml node1 = new DialogueNodeYml();
        node1.DialogueContent = content;
        node1.NodeName = "haha";
        node1.NextNodes = next_node;
        node1.Type = DialogueNodeYmlType.normal;

        DialogueNodeYml node2 = new DialogueNodeYml();
        node2.DialogueContent = content;
        node2.NodeName = "hahahohei";
        node2.NextNodes = next_node;
        node2.Type = DialogueNodeYmlType.codeEvent;

        DialogueYmlFile toBe = new DialogueYmlFile();
        toBe.ListOfNodes = new List<DialogueNodeYml>() { node1, node2 };
        var yaml = serializer.Serialize(toBe);
        File.WriteAllText("./test_11.yml", yaml);
    }
}

public class DialogueNodeYml
{
    public string NodeName;
    public List<string> DialogueContent = new List<string>();
    public DialogueNodeYmlType Type  = DialogueNodeYmlType.normal;
    public List<string> NextNodes = new List<string>();

}

public enum DialogueNodeYmlType
{
    normal = 0,
    choice = 1,
    codeEvent = 2,
}