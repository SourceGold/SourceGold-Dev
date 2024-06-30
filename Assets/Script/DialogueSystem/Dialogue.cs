using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Dialogue : InteractableObject
{
    public string path;

    private DialogueUI ui;
    private DialogueGraph graph;
    private DialogueGraphBaseNode entryNode;
    private DialogueGraphBaseNode currentNode;
    private int currentStep;

    public override void closestActivation()
    {
        
    }

    public override void closestDeactivation()
    {
        
    }

    public override void inRange()
    {
        
    }

    public override void outRange()
    {
        
    }

    public override bool playerInteract()
    {
        switch (currentNode)
        {
            case DialogueGraphTextNode textNode:
                ui.DisplayText(textNode.dialogue[currentStep]);
                currentStep = currentStep + 1;
                break;

            case DialogueGraphChoiceNode choiceNode:
                break;

            case DialogueGraphEventNode eventNode:
                break;
        }
        return false;
    }

    // Start is called before the first frame update
    void Start()
    {
        graph = new DialogueGraph(path);
        
        var graph_text = graph.GetGraphAsStrings();
        foreach ( var item in graph_text)
        {
            Debug.Log(item);
        }
        entryNode = graph.entryNode;
        currentNode = entryNode;
        ui = FindObjectOfType<DialogueUI>().GetComponent<DialogueUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
