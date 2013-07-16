using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public sealed class GameData
{
    private static readonly GameData instance = new GameData();
    public static GameData Instance
    {
        get
        {
            return instance;
        }
    }

    TreeNode<MenuNode> mainMenu = null;
    public TreeNode<MenuNode> MainMenu { get { return mainMenu; } }

    TreeNode<MenuNode> invItemMenu = null;

    Dictionary<ItemIDs, Item> allItems = new Dictionary<ItemIDs, Item>();

    private GameData() { }

    private string gameFile = "JSON/gameData";
    private Dictionary<string, object> gameData;

    private static string TryGetString(string key, Dictionary<string, object> json)
    {
        object value;

        if (json.TryGetValue(key, out value))
        {
            return value.ToString();
        }

        return "";
    }

    private static List<object> TryGetList(string key, Dictionary<string, object> json)
    {
        object value;

        if (json.TryGetValue(key, out value))
        {
            return (List<object>) value;
        }

        return new List<object>();
    }

    public void LoadData()
    {

        TextAsset gameAsset = (TextAsset)Resources.Load(gameFile, typeof(TextAsset));

        if (!gameAsset)
        {
            //game asset not found, no error trapping though...
        }

        gameData = gameAsset.text.dictionaryFromJson();

        Resources.UnloadAsset(gameAsset);

        Dictionary<string, object> itemsData = (Dictionary<string, object>)gameData["items"];

        foreach (string key in itemsData.Keys)
        {
            string itemID = key;

            Dictionary<string, object> itemData = (Dictionary<string, object>)itemsData[key];

            string description = TryGetString("description", itemData);
            string name = TryGetString("name", itemData);
            string type = TryGetString("type", itemData);
            Targets target = GetTargets(TryGetString("target", itemData));

            //int charges = 1;
            //int chargesPerUse = 0;

            int charges = 1;
            int chargesPerUse = 0;

            int.TryParse(TryGetString("charges", itemData), out charges);
            int.TryParse(TryGetString("chargesPerUse", itemData), out chargesPerUse);

            List<IGameEvent> itemEvents = new List<IGameEvent>();

            List<object> eventList = TryGetList("events", itemData);

            foreach (Dictionary<string, object> itemEvent in eventList)
            {
                itemEvents.Add(GenerateGameEvent(itemEvent));
            }


            Item item = new Item(itemID, name, description, target);
            item.Charges = charges;
            item.ChargesPerUse = chargesPerUse;
            //item.ItemType = type; // not used yet
            item.Events.AddRange(itemEvents);

            allItems.Add(GetItemId(itemID), item);
        }

        Dictionary<string, object> menuData = (Dictionary<string, object>)gameData["menus"];

        Dictionary<string, object> mainMenuData = (Dictionary<string, object>)menuData["main"];

        mainMenu = GenerateMenuTree(mainMenuData);

    }

    private Targets GetTargets(string target)
    {
        Targets targeting;

        switch (target)
        {
            case "self": targeting = Targets.SELF; break;
            case "any": targeting = Targets.ANY; break;
            case "all": targeting = Targets.ALL; break;
            case "enemies": targeting = Targets.ENEMIES; break;
            default: targeting = Targets.NONE;
                break;
        }

        return targeting;
    }

    private TreeNode<MenuNode> GenerateMenuTree(Dictionary<string, object> parent)
    {
        string title = TryGetString("title", parent);
        string text = TryGetString("text", parent);
        string description = TryGetString("description", parent);
        string displayMessage = TryGetString("displayMessage", parent);
        MenuNodeType type = GetMenuNodeType(TryGetString("type", parent));

        MenuNode menuNode = new MenuNode(type, title, text, displayMessage, description);

        TreeNode<MenuNode> treeNode = new TreeNode<MenuNode>(menuNode);

        if (parent.ContainsKey("children"))
        {
            Dictionary<string, object> childrenData = (Dictionary<string, object>)parent["children"];

            foreach (string key in childrenData.Keys)
            {
                Dictionary<string, object> childMenu = (Dictionary<string, object>)childrenData[key];
                treeNode.AddChild(GenerateMenuTree(childMenu));
            }
        }

        return treeNode;
    }

    public static MenuNodeType GetMenuNodeType(string type)
    {
        MenuNodeType nodeType;

        switch (type)
        {
            case "text": nodeType = MenuNodeType.TEXT; break;
            case "inventory": nodeType = MenuNodeType.INVENTORY; break;
            default:
                throw new ArgumentException("Menu node type of '" + type + "' is not valid.");
                break;
        }

        return nodeType;
    }

    public static MobStats GetMobStat(string stat)
    {
        MobStats mobStat;

        switch (stat)
        {
            case "hp": mobStat = MobStats.HP; break;
            case "energy": mobStat = MobStats.ENERGY; break;
            default: mobStat = MobStats.NONE; break;
                break;
        }

        return mobStat;
    }


    public static ItemIDs GetItemId(string itemId)
    {
        ItemIDs item = ItemIDs.NONE;

        foreach (ItemIDs itemType in Enum.GetValues(typeof(ItemIDs)))
        {
            if (Enum.GetName(typeof(ItemIDs), itemType).Equals(itemId))
            {
                item = itemType;
            }
        }

        return item;
    }


    public static IGameEvent GenerateGameEvent(Dictionary<string, object> itemEvent)
    {
        string eventType = TryGetString("type", itemEvent);

        IGameEvent gameEvent;

        switch (eventType)
        {
            case "modify_mob_stat":
                MobStats eventStat = GetMobStat(TryGetString("stat", itemEvent));
                float eventValue = 0;
                float.TryParse(TryGetString("value", itemEvent), out eventValue);
                gameEvent = new ModifyMobEvent(eventStat, eventValue);
                break;
            default:
                throw new ArgumentException("Event type of '" + eventType + "' is not valid.");
                break;
        }

        return gameEvent;
    }

    public Item GetNewItem(ItemIDs itemID)
    {
        Item blueprint = null;

        allItems.TryGetValue(itemID, out blueprint);

        return blueprint.Clone();
    }


    public TreeNode<MenuNode>[] GenerateInventoryNodes()
    {
        List<Item> items = GameVars.Instance.Player.Inventory;

        TreeNode<MenuNode>[] itemNodes = new TreeNode<MenuNode>[items.Count()];

        for (int i = 0; i < items.Count(); i++)
        {
            Item item = items[i];
            MenuNode node = new MenuNode(MenuNodeType.TEXT, item.ID, item.Name);
            TreeNode<MenuNode> treeNode = new TreeNode<MenuNode>(node);
            itemNodes[i] = treeNode;
        }

        return itemNodes;
    }
}
