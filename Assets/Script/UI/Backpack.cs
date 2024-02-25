using Assets.Script.Backend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Backpack : MonoBehaviour
{
    [SerializeField]
    public VisualTreeAsset _itemTemplate;
    [SerializeField]
    public VisualTreeAsset _itemBoxTemplate;

    public VisualElement _backpackScoll;

    private Inventory playerInventory;

    private void Awake()
    {
        UIDocument _doc = GetComponent<UIDocument>();
        _backpackScoll = _doc.rootVisualElement.Q<ScrollView>("backpack");
    }
    // Start is called before the first frame update
    void Start()
    {
        playerInventory = Backend.GameLoop.PlayerInventory;
        putItemIntoUI();
        EventManager.StartListening(GameEventTypes.InventoryChangeEvent, putItemIntoUI);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void putItemIntoUI()
    {
        _backpackScoll.Clear();
        int current_count = 0;
        VisualElement oneRow = _itemBoxTemplate.CloneTree();
        VisualElement rowBox = oneRow.Q<VisualElement>("rowOfInventory");
        _backpackScoll.Add(rowBox);
        foreach (var item in playerInventory._items)
        {
            VisualElement oneItem = _itemTemplate.CloneTree();
            VisualElement sprite = oneItem.Q<VisualElement>("icon");
            var count = oneItem.Q<Label>(name: "count");
            var level = oneItem.Q<Label>(name: "levelText");
            count.text = item.CurrentCount.ToString();
            level.text = "LV " + item.level.ToString();
            sprite.style.backgroundImage = new StyleBackground(item.staticInfo.itemImage);
            rowBox.Add(oneItem);
            current_count++;
            if (current_count == 5)
            {
                current_count = 0;
                oneRow = _itemBoxTemplate.CloneTree();
                rowBox = oneRow.Q<VisualElement>("rowOfInventory");
                _backpackScoll.Add(rowBox);
            }
        }

    }
}
