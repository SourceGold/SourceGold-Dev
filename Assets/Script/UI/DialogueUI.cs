using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueUI : MonoBehaviour
{
    public VisualTreeAsset _textTemplate;
    private VisualElement _textBox;
    // Start is called before the first frame update
    void Start()
    {
        UIDocument _doc = GetComponent<UIDocument>();
        _textBox = _doc.rootVisualElement.Q<VisualElement>("textBox");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayText(DialogueText text) {
        string content = "[" + text.speaker + "]: " + text.content;
        Label label = _textTemplate.CloneTree().Q<Label>("one-line-of-text");
        label.text = content;
        _textBox.Add(label);
    }
}
