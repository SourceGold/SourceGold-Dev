using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Backpack : MonoBehaviour
{

    private void Awake()
    {
        UIDocument _doc = GetComponent<UIDocument>();
        var _playButton = _doc.rootVisualElement.Q<ScrollView>("testing");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
