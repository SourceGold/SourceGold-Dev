using Assets.Script.Backend;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class collectableItem : InteractableObject
{
    public float fullVisibleRange = -1f;
    public float initialVisibleRange = -1f;

    [TextAreaAttribute]
    public string itemsConfig;
    
    private SceneItemActiveUI uiElement;
    private bool activated;
    private List<GameItemDynamic> gameItems = new List<GameItemDynamic>();
    private Inventory playerInventory;
    // Start is called before the first frame update
    void Start()
    {
        uiElement = GetComponentInChildren<SceneItemActiveUI>();

        // build the items
        convertStringToGameItem(itemsConfig);
        playerInventory = Backend.GameLoop.PlayerInventory;
    }

    private void convertStringToGameItem(string config)
    {
        config = config.Replace("\r\n", "\n");
        config = config.Replace("\r", "\n");
        string[] seperated = config.Split('\n');
        string display_name = "";
        int displayed_count = 0;
        foreach (string s in seperated)
        {
            string itemName;
            Dictionary<string, string> additionalConfig = new Dictionary<string, string>();

            string[] nameConfigSplit = s.Split(':');
            itemName = nameConfigSplit[0];
            string[] additionalConfigsSplit = nameConfigSplit[1].Split(",");
            foreach (string additionalConfigText in additionalConfigsSplit)
            {
                string[] configNameToValue = additionalConfigText.Split('=');
                additionalConfig[configNameToValue[0]] = configNameToValue[1];
            }
            gameItems.Add(new GameItemDynamic(itemName, additionalConfig));
            if (displayed_count < 2)
                display_name += "[" + itemName + "] ";
            if (displayed_count == 2)
                display_name += "...\n";

            displayed_count += 1;
            
        }
        uiElement.setText(display_name + " Press [f] to pick up");
    }
    public override void closestActivation()
    {
        if (uiElement != null) {
            uiElement.setActive(initialVisibleRange, fullVisibleRange); 
        }
    }

    public override void closestDeactivation()
    {
        if (uiElement != null) { 
            uiElement.setInactive(); 
        }
    }
    public override bool playerInteract()
    {
        foreach (GameItemDynamic item in gameItems)
        {
            playerInventory.AddItem(item);
        }
        Destroy(gameObject);
        return true;
    }
    
    public override void inRange() { }
    public override void outRange() { }
}
