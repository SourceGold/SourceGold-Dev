using Assets.Script.Backend;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.PlayerSettings;

public class Backpack : MonoBehaviour
{
    [SerializeField]
    public VisualTreeAsset _itemTemplate;
    [SerializeField]
    public VisualTreeAsset _emptyItemTemplate;
    [SerializeField]
    public VisualTreeAsset _itemBoxTemplate;
    [SerializeField]
    public VisualTreeAsset _rightClickTemplate;

    public VisualElement _backpackScoll;
    public VisualElement button;

    


    private Inventory playerInventory;
    private List<int> quickAccessList;

    private VisualElement menuArea;
    private VisualElement rootElement;

    private VisualElement buttonContainer;
    private VisualElement activatedInventoryItem;

    #region activatedItemRegion
    [SerializeField]
    public VisualTreeAsset activeElementAdditionalDesciptionTemplate;

    private VisualElement _activatedItemSprite;
    private Label _activatedItemLevel;
    private Label _activatedItemText;
    private Label _activatedItemName;
    private VisualElement _activatedItemAdditionalDescriptionField;
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


    private void HandleRightClick(MouseUpEvent evt)
    {
        if (evt.button != (int)MouseButton.RightMouse)
            return;

        var targetElement = evt.target as VisualElement;
        if (targetElement == null)
            return;

        var menu = new UnityEditor.GenericMenu();
        VisualElement clonedRightClick = _rightClickTemplate.CloneTree();
        buttonContainer = clonedRightClick.Q<VisualElement>("ButtonContainer");
        buttonContainer.style.position = Position.Absolute;
        
        menuArea.Add(buttonContainer);

        // Get position of menu on top of target element.
        var menuPosition = evt.mousePosition;
        Vector2 localPos = rootElement.ChangeCoordinatesTo(menuArea, evt.mousePosition);
        buttonContainer.style.top = localPos.y + 5;
        buttonContainer.style.left = localPos.x + 5;
    }

    private void HandleLeftClick(MouseUpEvent evt, InventoryItem item)
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
        print(targetElement.name);

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

    private void Awake()
    {
        UIDocument _doc = GetComponent<UIDocument>();
        _backpackScoll = _doc.rootVisualElement.Q<ScrollView>("backpack");
        button = _doc.rootVisualElement.Q<Button>("consumableButton");
        menuArea = _doc.rootVisualElement.Q<VisualElement>("MenuArea");
        rootElement = _doc.rootVisualElement.Q<VisualElement>("RootElement");

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
        EventManager.StartListening(GameEventTypes.InventoryChangeEvent, putItemIntoUI);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void putItemsIntoQuickAccess()
    {
        foreach (int itemID in quickAccessList) 
        {
            if (itemID == -1)
            {
                // add as empty
            } else
            {
                var item = playerInventory._items[quickAccessList[itemID]];

            }
        }
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
            VisualElement oneItem = constructOneInventoryItem(item);
            rowBox.Add(oneItem);
            oneItem.RegisterCallback<MouseUpEvent>(HandleRightClick, TrickleDown.TrickleDown);

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

    private VisualElement constructOneInventoryItem(InventoryItem item)
    {
        VisualElement   oneItem = _itemTemplate.CloneTree();

        VisualElement   sprite = oneItem.Q<VisualElement>("icon");
        VisualElement   newItemDot = oneItem.Q<Label>("newItemDot");

        var             count = oneItem.Q<Label>(name: "count");
        var             level = oneItem.Q<Label>(name: "levelText");

        count.text = item.CurrentCount.ToString();
        level.text = "LV " + item.level.ToString();
        sprite.style.backgroundImage = new StyleBackground(item.staticInfo.itemImage);
        if (item.isNew)
            newItemDot.style.visibility = Visibility.Visible;
        else
            newItemDot.style.visibility = Visibility.Hidden;

        oneItem.RegisterCallback<MouseUpEvent, InventoryItem>(HandleLeftClick, item, TrickleDown.TrickleDown);
        return oneItem;
    }

    private void constructActiveItemView(InventoryItem item)
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

        for (int i = 0; i < _activatedItemAdditionalDescriptionField.childCount; i ++)
        {
            _activatedItemAdditionalDescriptionField.RemoveAt(0);
        }
        
    }
}
