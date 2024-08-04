using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Transactions;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Dialogue : InteractableObject
{
    public string path;

    private DialogueUI ui;
    private DialogueGraph graph;
    private DialogueGraphBaseNode currentNode;
    private int currentNodeTextID;
    private SceneItemActiveUI uiElement;

    public override void closestActivation()
    {
        if (uiElement != null && currentNode != null)
        {
            uiElement.setActive(-1, -1);
        }
    }

    public override void closestDeactivation()
    {
        if (uiElement != null)
        {
            uiElement.setInactive();
        }
        currentNodeTextID = -1;
        ui.EndOfConversation();
    }

    public override void inRange()
    {
        
    }

    public override void outRange()
    {
        
    }

    public void choiceClick(DialogueGraphBaseNode node)
    {
        currentNode = node;
        currentNodeTextID = 0;
        performStateTransition();
    }

    public override bool playerInteract()
    {
        if (currentNode != null)
        {
            switch (currentNode)
            {
                case DialogueGraphTextNode textNode:
                    currentNodeTextID += 1;
                    if (currentNodeTextID >= textNode.dialogue.Count)
                    {
                        currentNode = textNode.nextNode;
                        currentNodeTextID = 0;
                    }
                    performStateTransition();
                    break;

                case DialogueGraphChoiceNode choiceNode:
                    // Do nothing, waiting for UI response. 
                    // We display again if the first node is choice
                    ui.DisplayChoice(choiceNode.choices, this);
                    break;
                case DialogueGraphEventNode eventNode:
                case DialogueGraphEndNode endNode:
                // Current Node will never be choice node.

                    break;
            }
        }
        return false;
    }

    private void performStateTransition()
    {
        switch (currentNode)
        {
            case DialogueGraphTextNode textNode:
                ui.DisplayText(textNode.dialogue[currentNodeTextID]);
                break;

            case DialogueGraphChoiceNode choiceNode:
                ui.DisplayChoice(choiceNode.choices, this);
                break;

            case DialogueGraphEventNode eventNode:
                // Do something, not implemented yet
                currentNode = eventNode.nextNode;
                currentNodeTextID = 0;
                performStateTransition();
                break;

            case DialogueGraphEndNode endNode:
                ui.EndOfConversation();
                currentNode = endNode.nextNode;
                if (currentNode == null)
                    uiElement.setInactive();
                currentNodeTextID = -1;
                break;
        }
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

        currentNode = graph.entryNode;
        currentNodeTextID = -1;

        ui = FindObjectOfType<DialogueUI>().GetComponent<DialogueUI>();
        uiElement = GetComponentInChildren<SceneItemActiveUI>();
        uiElement.setText("press [F] to talk");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
