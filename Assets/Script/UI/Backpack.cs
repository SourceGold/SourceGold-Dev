using Assets.Script.Backend;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class Backpack : MonoBehaviour
{
    private VisualElement menuArea;
    private VisualElement rootElement;

    #region Main Display Area
    [SerializeField]
    public VisualTreeAsset _itemTemplate;
    [SerializeField]
    public VisualTreeAsset _emptyItemTemplate;
    [SerializeField]
    public VisualTreeAsset _itemBoxTemplate;
    
    public VisualElement _backpackScoll;
    #endregion

    #region Quick Access
    public VisualElement quickAccessPanel;
    #endregion

    #region Right Click
    [SerializeField]
    public VisualTreeAsset _rightClickTemplate;
    [SerializeField]
    public VisualTreeAsset _rightClickButtonTemplate;
    private VisualElement buttonContainer;
    #endregion

    #region Activated Item Region
    [SerializeField]
    public VisualTreeAsset activeElementAdditionalDesciptionTemplate;
    private VisualElement _activatedItemSprite;
    private Label _activatedItemLevel;
    private Label _activatedItemText;
    private Label _activatedItemName;
    private VisualElement _activatedItemAdditionalDescriptionField;

    private VisualElement activatedInventoryItem;
    #endregion

    private Inventory playerInventory;
    private List<int> quickAccessList;
    private void Awake()
    {
        UIDocument _doc = GetComponent<UIDocument>();
        _backpackScoll = _doc.rootVisualElement.Q<ScrollView>("backpack");
        menuArea = _doc.rootVisualElement.Q<VisualElement>("MenuArea");
        rootElement = _doc.rootVisualElement.Q<VisualElement>("RootElement");
        quickAccessPanel = _doc.rootVisualElement.Q<VisualElement>("quickAccessPanel");

        _activatedItemSprite = _doc.rootVisualElement.Q<VisualElement>("activatedItemSprite");
        _activatedItemLevel = _doc.rootVisualElement.Q<Label>("activatedItemLevel");
        _activatedItemName = _doc.rootVisualElement.Q<Label>("activatedItemName");
        _activatedItemText = _doc.rootVisualElement.Q<Label>("activatedItemText");
        _activatedItemAdditionalDescriptionField = _doc.rootVisualElement.Q<VisualElement>("activatedItemAdditionalDescriptionField");
        menuArea.RegisterCallback<MouseUpEvent>(HandleNotRightClick, TrickleDown.TrickleDown);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInventory = Backend.GameLoop.PlayerInventory;
        quickAccessList = Backend.GameLoop.quickAccessItems;
        putItemIntoUI();
        putItemsIntoQuickAccess();
        EventManager.StartListening(GameEventTypes.InventoryChangeEvent, putItemIntoUI);
        EventManager.StartListening(GameEventTypes.InventoryChangeEvent, putItemsIntoQuickAccess);
    }

    #region Main Display Area
    private void putItemIntoUI()
    {
        _backpackScoll.Clear();
        int current_count = 0;
        VisualElement oneRow = _itemBoxTemplate.CloneTree();
        VisualElement rowBox = oneRow.Q<VisualElement>("rowOfInventory");
        _backpackScoll.Add(rowBox);
        for (int i = 0; i < playerInventory._items.Count; i++)
        {
            GameItemDynamic item = playerInventory._items[i];
            VisualElement oneItem = constructOneInventoryItem(item);
            rowBox.Add(oneItem);
            oneItem.RegisterCallback<MouseUpEvent, int>(HandleRightClick, i, TrickleDown.TrickleDown);

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

    private VisualElement constructOneInventoryItem(GameItemDynamic item)
    {
        VisualElement oneItem = _itemTemplate.CloneTree();

        VisualElement sprite = oneItem.Q<VisualElement>("icon");
        VisualElement newItemDot = oneItem.Q<Label>("newItemDot");

        var count = oneItem.Q<Label>(name: "count");
        var level = oneItem.Q<Label>(name: "levelText");

        count.text = item.CurrentCount.ToString();
        level.text = "LV " + item.level.ToString();
        sprite.style.backgroundImage = new StyleBackground(item.staticInfo.itemImage);
        if (item.isNew)
            newItemDot.style.visibility = Visibility.Visible;
        else
            newItemDot.style.visibility = Visibility.Hidden;

        oneItem.RegisterCallback<MouseUpEvent, GameItemDynamic>(HandleLeftClick, item, TrickleDown.TrickleDown);
        return oneItem;
    }
    #endregion

    #region Quick Access
    private void putItemsIntoQuickAccess()
    {

        quickAccessPanel.Clear();
        foreach (int itemID in quickAccessList)
        {
            if (itemID == -1)
            {
                // add as empty
                VisualElement empty = _emptyItemTemplate.CloneTree().Q<VisualElement>("inventoryEmptyItem");
                quickAccessPanel.Add(empty);
            }
            else
            {
                var item = playerInventory._items[itemID];
                var oneElement = constructOneInventoryItem(item);
                oneElement.RegisterCallback<MouseUpEvent, int>(HandleQuickAccessRightClick, itemID, TrickleDown.TrickleDown);

                VisualElement newItemDot = oneElement.Q<Label>("newItemDot");
                newItemDot.style.visibility = Visibility.Hidden;
                quickAccessPanel.Add(oneElement);
            }
        }
    }

    private void addToQuickAccess(int location, int inventoryIndex)
    {
        for (int i = 0; i < quickAccessList.Count; i++)
        {
            if (quickAccessList[i] == inventoryIndex)
                quickAccessList[i] = -1;

        }
        quickAccessList[location] = inventoryIndex;
    }

    private void removeFromQuickAccess(int inventoryIndex)
    {
        for (int i = 0; i < quickAccessList.Count; i++)
        {
            if (quickAccessList[i] == inventoryIndex)
                quickAccessList[i] = -1;

        }
    }
    #endregion

    #region Activated Item Region
    private void constructActiveItemView(GameItemDynamic item)
    {
        _activatedItemName.text = item.staticInfo.englishDescription.displayName;
        _activatedItemText.text = item.staticInfo.englishDescription.displayDescription;
        _activatedItemSprite.style.backgroundImage = new StyleBackground(item.staticInfo.itemImage);

        if (item.staticInfo.max_level > 0)
        {
            _activatedItemLevel.text = "LV " + item.level.ToString() + " / " + item.staticInfo.max_level;
        }
        else
        {
            _activatedItemLevel.text = "LV " + item.level.ToString();
        }

        _activatedItemAdditionalDescriptionField.Clear();

        foreach (var stats in item.additionalFloatStats)
        {
            constructAdditionalDescription($"{stats.Key} : {stats.Value}");
        }

        foreach (var stats in item.additionalIntStats)
        {
            constructAdditionalDescription($"{stats.Key} : {stats.Value}");
        }

        foreach (var stats in item.addtionalStatus)
        {
            constructAdditionalDescription($"{stats}");
        }
    }

    private void constructAdditionalDescription(string description)
    {
        VisualElement oneRow = activeElementAdditionalDesciptionTemplate.CloneTree();
        Label row = oneRow.Q<Label>("Description");
        row.text = description;
        _activatedItemAdditionalDescriptionField.Add(row);
    }
    #endregion

    private void HandleNotRightClick(MouseUpEvent evt)
    {
        var targetElement = evt.target as VisualElement;
        if (buttonContainer != null && !buttonContainer.Contains(targetElement))
        {
            menuArea.Remove(buttonContainer);
            buttonContainer = null;
        }
    }

    private void HandleRightClick(MouseUpEvent evt, int inventoryIndex)
    {
        if (evt.button != (int)MouseButton.RightMouse)
            return;

        var targetElement = evt.target as VisualElement;
        if (targetElement == null)
            return;

        var menu = new UnityEditor.GenericMenu();
        buttonContainer = _rightClickTemplate.CloneTree().Q<VisualElement>("ButtonContainer");
        buttonContainer.style.position = Position.Absolute;

        Button button = _rightClickButtonTemplate.CloneTree().Q<Button>("RightClickButton");
        button.text = "Set to Q";
        button.clicked += () => { addToQuickAccess(0, inventoryIndex); menuArea.Remove(buttonContainer); buttonContainer = null; putItemsIntoQuickAccess(); };
        buttonContainer.Add(button);

        button = _rightClickButtonTemplate.CloneTree().Q<Button>("RightClickButton");
        button.text = "Set to W";
        button.clicked += () => { addToQuickAccess(1, inventoryIndex); menuArea.Remove(buttonContainer); buttonContainer = null; putItemsIntoQuickAccess(); };
        buttonContainer.Add(button);

        button = _rightClickButtonTemplate.CloneTree().Q<Button>("RightClickButton");
        button.text = "Set to E";
        button.clicked += () => { addToQuickAccess(2, inventoryIndex); menuArea.Remove(buttonContainer); buttonContainer = null; putItemsIntoQuickAccess(); };
        buttonContainer.Add(button);

        button = _rightClickButtonTemplate.CloneTree().Q<Button>("RightClickButton");
        button.text = "Set to R";
        button.clicked += () => { addToQuickAccess(3, inventoryIndex); menuArea.Remove(buttonContainer); buttonContainer = null; putItemsIntoQuickAccess(); };
        buttonContainer.Add(button);

        button = _rightClickButtonTemplate.CloneTree().Q<Button>("RightClickButton");
        button.text = "Consume";
        button.clicked += () => {
            ConsumableController.Instance.consumeItem(playerInventory._items[inventoryIndex]);
            menuArea.Remove(buttonContainer); 
            buttonContainer = null; 
        };

        buttonContainer.Add(button);

        menuArea.Add(buttonContainer);

        // Get position of menu on top of target element.
        var menuPosition = evt.mousePosition;
        Vector2 localPos = rootElement.ChangeCoordinatesTo(menuArea, evt.mousePosition);
        buttonContainer.style.top = localPos.y + 5;
        buttonContainer.style.left = localPos.x + 5;
    }

    private void HandleQuickAccessRightClick(MouseUpEvent evt, int inventoryIndex)
    {
        if (evt.button != (int)MouseButton.RightMouse)
            return;

        var targetElement = evt.target as VisualElement;
        if (targetElement == null)
            return;

        var menu = new UnityEditor.GenericMenu();
        buttonContainer = _rightClickTemplate.CloneTree().Q<VisualElement>("ButtonContainer");
        buttonContainer.style.position = Position.Absolute;

        Button button = _rightClickButtonTemplate.CloneTree().Q<Button>("RightClickButton");
        button.text = "Remove";
        button.clicked += () => { removeFromQuickAccess(inventoryIndex); menuArea.Remove(buttonContainer); buttonContainer = null; putItemsIntoQuickAccess(); };
        buttonContainer.Add(button);

        button = _rightClickButtonTemplate.CloneTree().Q<Button>("RightClickButton");
        button.text = "Consume";
        button.clicked += () => {
            ConsumableController.Instance.consumeItem(playerInventory._items[inventoryIndex]);
            menuArea.Remove(buttonContainer);
            buttonContainer = null;
        };

        buttonContainer.Add(button);

        menuArea.Add(buttonContainer);

        // Get position of menu on top of target element.
        var menuPosition = evt.mousePosition;
        Vector2 localPos = rootElement.ChangeCoordinatesTo(menuArea, evt.mousePosition);
        buttonContainer.style.top = localPos.y + 5;
        buttonContainer.style.left = localPos.x + 5;
    }

    private void HandleLeftClick(MouseUpEvent evt, GameItemDynamic item)
    {
        if (evt.button != (int)MouseButton.LeftMouse)
            return;

        var targetElement = evt.target as VisualElement;
        if (targetElement == null)
            return;

        if (activatedInventoryItem != null)
        {
            activatedInventoryItem.RemoveFromClassList("inventoryItemActive");
            activatedInventoryItem.AddToClassList("inventoryItem");
        }

        if (targetElement.name == "inventoryItem")
        {
            activatedInventoryItem = targetElement;
            targetElement.RemoveFromClassList("inventoryItem");
            targetElement.AddToClassList("inventoryItemActive");

            constructActiveItemView(item);
        }
        
        if (item.isNew)
        {
            item.isNew = false;
            VisualElement newItemDot = targetElement.Q<Label>("newItemDot");
            newItemDot.style.visibility = Visibility.Hidden;
        }
    }
}
