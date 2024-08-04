using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueUI : MonoBehaviour
{
    public VisualTreeAsset _textTemplate;
    public VisualTreeAsset _choiceUI;
    public VisualTreeAsset _choiceTemplate;

    private VisualElement overallRoot;
    private VisualElement choiceRoot;
    private VisualElement dialogRoot;

    private VisualElement _dialogueBox;
    private VisualElement _choiceBox;
    // Start is called before the first frame update
    void Start()
    {
        UIDocument _doc = GetComponent<UIDocument>();
        overallRoot = _doc.rootVisualElement;
        
        dialogRoot = _doc.rootVisualElement.Q<VisualElement>("DialogueRoot");
        _dialogueBox = _doc.rootVisualElement.Q<VisualElement>("textBox");

        choiceRoot = _choiceUI.CloneTree().Q<VisualElement>("ChoiceRoot");
        _choiceBox = choiceRoot.Q<VisualElement>("ChoiceBox");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayText(DialogueText text) {
        overallRoot.Clear();
        overallRoot.Add(dialogRoot);
        string content = "[" + text.speaker + "]: " + text.content;
        Label label = _textTemplate.CloneTree().Q<Label>("one-line-of-text");
        label.text = content;
        _dialogueBox.Add(label);
    }

    public void DisplayChoice(List<DialogueChoice> choices, Dialogue diaglogueController)
    {
        overallRoot.Clear();
        overallRoot.Add(choiceRoot);
        _choiceBox.Clear();
        UnityEngine.Cursor.lockState = CursorLockMode.None;
        UnityEngine.Cursor.visible = true;
        foreach (DialogueChoice choice in choices)
        {
            Button choice_button = _choiceTemplate.CloneTree().Q<Button>("one-line-of-choice");
            choice_button.text = choice.choice;
            choice_button.RegisterCallback<MouseEnterEvent>(x => { VisualElement target = x.target as VisualElement; target.Focus(); });
            choice_button.clicked += () => { 
                diaglogueController.choiceClick(choice.nextNode); 
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
            };
            _choiceBox.Add(choice_button);
        }
    }

    public void EndOfConversation()
    {
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        overallRoot.Clear();
        _dialogueBox.Clear();
    }
}
