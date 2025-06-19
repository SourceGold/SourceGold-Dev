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
using UnityEditor.Experimental.GraphView;
using System.Linq;

public class DialogueGraph
{
    private Dictionary<string, DialogueGraphBaseNode> nodes = new Dictionary<string, DialogueGraphBaseNode>();
    private List<string> avaliable_nodes = new List<string>();
    public DialogueGraphBaseNode entryNode = null;
    public DialogueGraph(string path)
    {
        IDeserializer deserializer;
        deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)  // see height_in_inches in sample yml 
                .Build();

        var ymlInText = Resources.Load<TextAsset>(path).text;
        // var ymlInText = File.ReadAllText(path);
        DialogueYmlFile ymls = deserializer.Deserialize<DialogueYmlFile>(ymlInText);
        ymls.checkNodes();

        Convert_yml_to_graph(ymls);
        ResolveNextNodes();
        ValidationCheck();
    }

    private void Convert_yml_to_graph(DialogueYmlFile yml)
    {
        foreach (var ymlNode in yml.ListOfNodes)
        {
            avaliable_nodes.Add(ymlNode.NodeName);
            if (ymlNode.Type == DialogueNodeYmlType.normal)
            {
                nodes.Add(ymlNode.NodeName, new DialogueGraphTextNode(ymlNode));
            }
            else if (ymlNode.Type == DialogueNodeYmlType.choice)
            {
                nodes.Add(ymlNode.NodeName, new DialogueGraphChoiceNode(ymlNode));
            }
            else if (ymlNode.Type == DialogueNodeYmlType.codeEvent)
            {
                nodes.Add(ymlNode.NodeName, new DialogueGraphEventNode(ymlNode));
            }
            else if (ymlNode.Type == DialogueNodeYmlType.end)
            {
                nodes.Add(ymlNode.NodeName, new DialogueGraphEndNode(ymlNode));
            }
        }
        entryNode = nodes[avaliable_nodes[0]];
        if (entryNode.type != DialogueNodeType.normal)
        {
            entryNode = new DialogueGraphTextNode("fake_entry_text", entryNode);
            nodes.Add("fake_entry_text", entryNode);
        }
    }

    private void ResolveNextNodes()
    {
        // Second pass: resolve nextNode references
        foreach (var node in nodes.Values)
        {
            switch (node)
            {
                case DialogueGraphTextNode textNode:
                    if (textNode.nextNodeString == null || textNode.nextNodeString == "null")
                    {
                        throw new Exception($"Node name {textNode.nodeName} is type text should have a next node.");
                    }
                    textNode.nextNode = nodes[textNode.nextNodeString];
                    break;

                case DialogueGraphChoiceNode choiceNode:
                    foreach (var choice in choiceNode.choices)
                    {
                        if (choice.nextNodeString == null || choice.nextNodeString == "null")
                        {
                            throw new Exception($"Node name {choiceNode.nodeName} is type choice and each choice should have a next node."); 
                        }
                        choice.nextNode = nodes[choice.nextNodeString];
                    }
                    break;

                case DialogueGraphEventNode eventNode:
                    if (eventNode.nextNodeString == null || eventNode.nextNodeString == "null")
                    {
                        throw new Exception($"Node name {eventNode.nodeName} is type event and should have a next node.");
                    }
                    eventNode.nextNode = nodes[eventNode.nextNodeString];
                    break;

                case DialogueGraphEndNode endNode:
                    Console.WriteLine(endNode.nextNodeString);
                    if (endNode.nextNodeString != null && endNode.nextNodeString != "null")
                    {
                        endNode.nextNode = nodes[endNode.nextNodeString];
                    }
                    break;
            }
        }
    }

    private void ValidationCheck()
    {
        CheckNodesWithNoIncomingReferences();
        CheckForLoops();
    }


    private void CheckNodesWithNoIncomingReferences()
    {
        var referenceCounts = new Dictionary<string, int>();

        // Initialize all nodes with zero references
        foreach (var nodeName in nodes.Keys)
        {
            referenceCounts[nodeName] = 0;
        }

        // Count references
        foreach (var node in nodes.Values)
        {
            switch (node)
            {
                case DialogueGraphTextNode textNode:
                    if (textNode.nextNode is DialogueGraphBaseNode nextTextNode)
                    {
                        referenceCounts[nextTextNode.nodeName]++;
                    }
                    break;

                case DialogueGraphChoiceNode choiceNode:
                    foreach (var choice in choiceNode.choices)
                    {
                        if (choice.nextNode is DialogueGraphBaseNode nextChoiceNode)
                        {
                            referenceCounts[nextChoiceNode.nodeName]++;
                        }
                    }
                    break;

                case DialogueGraphEventNode eventNode:
                    if (eventNode.nextNode is DialogueGraphBaseNode nextEventNode)
                    {
                        referenceCounts[nextEventNode.nodeName]++;
                    }
                    break;

                case DialogueGraphEndNode endNode:
                    if (endNode.nextNode is DialogueGraphBaseNode nextEndNode)
                    {
                        referenceCounts[nextEndNode.nodeName]++;
                    }
                    break;
            }
        }

        // Log nodes with no incoming references, excluding the first node
        foreach (var kvp in referenceCounts)
        {
            if (kvp.Value == 0 && kvp.Key != nodes.Keys.First())
            {
                Console.WriteLine($"Node with no incoming references: {kvp.Key}");
            }
        }
    }

    private void CheckForLoops()
    {
        var visited = new HashSet<string>();
        var stack = new HashSet<string>();

        foreach (var nodeName in nodes.Keys)
        {
            if (!visited.Contains(nodeName))
            {
                if (DetectCycle(nodeName, visited, stack))
                {
                    Console.WriteLine($"Loop detected in the graph: {string.Join(" -> ", stack)}");
                }
            }
        }
    }

    private bool DetectCycle(string currentNode, HashSet<string> visited, HashSet<string> stack)
    {
        if (stack.Contains(currentNode))
        {
            stack.Add(currentNode); // Complete the cycle for logging
            return true;
        }

        if (visited.Contains(currentNode))
        {
            return false;
        }

        visited.Add(currentNode);
        stack.Add(currentNode);

        DialogueGraphBaseNode node = nodes[currentNode];

        switch (node)
        {
            case DialogueGraphTextNode textNode:
                if (textNode.nextNode is DialogueGraphBaseNode nextTextNode)
                {
                    if (DetectCycle(nextTextNode.nodeName, visited, stack))
                    {
                        return true;
                    }
                }
                break;

            case DialogueGraphChoiceNode choiceNode:
                foreach (var choice in choiceNode.choices)
                {
                    if (choice.nextNode is DialogueGraphBaseNode nextChoiceNode)
                    {
                        if (DetectCycle(nextChoiceNode.nodeName, visited, stack))
                        {
                            return true;
                        }
                    }
                }
                break;

            case DialogueGraphEventNode eventNode:
                if (eventNode.nextNode is DialogueGraphBaseNode nextEventNode)
                {
                    if (DetectCycle(nextEventNode.nodeName, visited, stack))
                    {
                        return true;
                    }
                }
                break;

            case DialogueGraphEndNode endNode:
                if (endNode.nextNode is DialogueGraphBaseNode nextEndNode)
                {
                    if (DetectCycle(nextEndNode.nodeName, visited, stack))
                    {
                        return true;
                    }
                }
                break;
        }

        stack.Remove(currentNode);
        return false;
    }

    public List<string> GetGraphAsStrings()
    {
        var result = new List<string>();
        var visited = new HashSet<string>();

        foreach (var node in nodes.Values)
        {
            if (!visited.Contains(node.nodeName))
            {
                result.Add(GetNodeChainAsStrings(node, visited));
            }
        }

        return result;
    }

    private string GetNodeChainAsStrings(DialogueGraphBaseNode node, HashSet<string> visited)
    {
        var chain = new List<string>();
        var currentNode = node;
        var nodeString = "";
        var local_visited = new HashSet<string>();
        while (currentNode != null && !local_visited.Contains(currentNode.nodeName))
        {
            nodeString += currentNode.nodeName;
            visited.Add(currentNode.nodeName);
            local_visited.Add(currentNode.nodeName);

            DialogueGraphBaseNode nextNode = null;
            List<string> nextNodes = new List<string>();

            switch (currentNode)
            {
                case DialogueGraphTextNode textNode:
                    if (textNode.nextNode is DialogueGraphBaseNode nextTextNode)
                    {
                        nextNode = nextTextNode;
                    }
                    break;

                case DialogueGraphChoiceNode choiceNode:
                    foreach (var choice in choiceNode.choices)
                    {
                        if (choice.nextNode is DialogueGraphBaseNode nextChoiceNode)
                        {
                            nextNodes.Add(nextChoiceNode.nodeName);
                            if (nextNode == null && !local_visited.Contains(nextChoiceNode.nodeName))
                            {
                                nextNode = nextChoiceNode; // Select the first unvisited next node
                            }
                        }
                    }
                    if (nextNodes.Count > 0)
                    {
                        nodeString += " -> [" + string.Join(", ", nextNodes) + "]";
                    }
                    break;

                case DialogueGraphEventNode eventNode:
                    if (eventNode.nextNode is DialogueGraphBaseNode nextEventNode)
                    {
                        nextNode = nextEventNode;
                    }
                    break;

                case DialogueGraphEndNode endNode:
                    if (endNode.nextNode is DialogueGraphBaseNode nextEndNode)
                    {
                        nodeString += " ... " + nextEndNode.nodeName;
                    }
                    else
                    {
                        nodeString += "[END]";
                    }
                    break;
            }

            if (nextNode != null && !local_visited.Contains(nextNode.nodeName))
            {
                nodeString += " -> ";
            }

            currentNode = nextNode;
        }

        return nodeString;
    }
}

public class DialogueGraphEventNode : DialogueGraphBaseNode
{
    public List<string> eventNames = new List<string>();
    public DialogueGraphBaseNode nextNode = null;
    public string nextNodeString;

    public DialogueGraphEventNode(DialogueNodeYml yml)
    {
        nodeName = yml.NodeName;
        type = DialogueNodeType.codeEvent;
        foreach (string codeEvent in yml.DialogueContent)
        {
            eventNames.Add(codeEvent);
        }

        if (yml.NextNodes.Count > 0)
            nextNodeString = yml.NextNodes[0];
        else
            nextNodeString = null;
    }
}

public class DialogueGraphChoiceNode : DialogueGraphBaseNode
{
    public List<DialogueChoice> choices = new List<DialogueChoice>();
    public DialogueGraphChoiceNode(DialogueNodeYml yml)
    {
        nodeName = yml.NodeName;
        type = DialogueNodeType.choice;
        for (int i = 0; i < yml.DialogueContent.Count; i++)
            choices.Add(new DialogueChoice(yml.DialogueContent[i], yml.NextNodes[i]));
    }
}

public class DialogueGraphTextNode : DialogueGraphBaseNode
{
    public List<DialogueText> dialogue = new List<DialogueText>();
    public DialogueGraphBaseNode nextNode = null;
    public string nextNodeString;

    public DialogueGraphTextNode(DialogueNodeYml yml) {
        nodeName = yml.NodeName;
        type = DialogueNodeType.normal;
        foreach (string conversation in yml.DialogueContent)
        {
            dialogue.Add(new DialogueText(conversation));
        }

        if (yml.NextNodes.Count > 0)
            nextNodeString = yml.NextNodes[0];
        else
            nextNodeString = null;
    }

    public DialogueGraphTextNode(string name, DialogueGraphBaseNode nextNode)
    {
        nodeName = name; 
        type = DialogueNodeType.normal;
        this.nextNode = nextNode;
    }
}

public class DialogueGraphEndNode : DialogueGraphBaseNode
{
    public DialogueGraphBaseNode nextNode = null;
    public string nextNodeString;

    public DialogueGraphEndNode(DialogueNodeYml yml)
    {
        nodeName = yml.NodeName;
        type = DialogueNodeType.end;

        if (yml.NextNodes.Count > 0)
            nextNodeString = yml.NextNodes[0];
        else
            nextNodeString = null;
    }
}

public class DialogueGraphBaseNode
{
    public string nodeName;
    public DialogueNodeType type;
}

public class DialogueText
{
    public string speaker;
    public string content;
    public DialogueText(string combinedString)
    {
        int index = combinedString.IndexOf('~');

        if (index != -1)
        {
            // Split the string into two parts
            speaker = combinedString.Substring(0, index);
            content = combinedString.Substring(index + 1);
        }
        else
        {
            speaker = "?";
            content = combinedString;
        }
    }
}

public class DialogueChoice
{
    public string choice;
    public DialogueGraphBaseNode nextNode = null;
    public string nextNodeString;

    public DialogueChoice(string choice, string nextNode)
    {
        this.choice = choice;
        if (nextNode == "null")
            nextNodeString = null;
        else
            nextNodeString = nextNode;
    }
}

public enum DialogueNodeType
{
    normal = 0,
    choice = 1,
    codeEvent = 2,
    end = 3,
}