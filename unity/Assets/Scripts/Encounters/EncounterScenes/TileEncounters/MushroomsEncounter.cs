using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class MushroomsEncounter : FEncounterScene
{

    private const string CHOICE1_BRIGHT = "Choice1_Bright";
    private const string CHOICE1_DULL = "Choice1_Dull";
    private const string CHOICE1_IGNORE = "Choice1_Ignore";
    private const string CHOICE1_ALL = "Choice1_All";

    private const float ENERGY_THRESHOLD = 50;

    public bool MushroomsEaten = true;

    public MushroomsEncounter(string _name = "Default")
        : base(_name)
    {

    }

    private int GetBrightEnergy()
    {
        return UnityEngine.Random.Range(5, 16);
    }
    private int GetDullEnergy()
    {
        return UnityEngine.Random.Range(7, 13);
    }

    protected override void HandleResult()
    {
        MenuNode selectedNode = FSelectionDisplayScene.GetLastResultNode(this.resultNode).Value;

        int addEnergy = 0;

        switch (selectedNode.NodeTitle)
        {
            case CHOICE1_BRIGHT: //result of eating bright mushrooms
                GameVars.Instance.SetParamValue(this.Name + ":" + CHOICE1_BRIGHT, true);
                addEnergy = GetBrightEnergy();
                break;
            case CHOICE1_DULL: //result of eating dull mushrooms
                GameVars.Instance.SetParamValue(this.Name + ":" + CHOICE1_DULL, true);
                addEnergy = GetDullEnergy();
                break;
            case CHOICE1_IGNORE: //result of ignoring mushrooms
                GameVars.Instance.SetParamValue(this.Name + ":" + CHOICE1_IGNORE, true);
                this.MushroomsEaten = false;
                break;
            case CHOICE1_ALL: //result of eating all mushrooms
                GameVars.Instance.SetParamValue(this.Name + ":" + CHOICE1_ALL, true);
                addEnergy = GetBrightEnergy() + GetDullEnergy();
                break;
            default:
                break;
        }

        if (this.MushroomsEaten)
        {
            GameVars.Instance.Player.Energy += addEnergy;
        }

        IPDebug.Log(selectedNode.NodeTitle);
        this.ShouldPop = true;
    }

    protected override void HandleCancel()
    {
        IPDebug.Log("Selection cancelled");
    }

    public override void OnEnter()
    {
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "You find a cluster of mushrooms here on the forest floor. There are many different sizes, shapes, and colors.");

        rootMenu.DisplayImageAsset = "mushroom";
        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        bool hasEatenBright = GameVars.Instance.GetParamValueBool(this.Name + ":" + CHOICE1_BRIGHT);
        bool hasEatenDull = GameVars.Instance.GetParamValueBool(this.Name + ":" + CHOICE1_DULL);

        if (hasEatenBright && hasEatenDull)
        {
            rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_ALL, "Eat all the mushrooms!", "Bright? Dull? They're both favorite!")));
        }
        else
        {
            rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_BRIGHT, "Eat some of the brightly-colored mushrooms.", "The brightly-colored mushrooms are very beautiful and easily catch your eye. As it turns out, they taste just like candy! Yum!")));
            rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_DULL, "Eat some of the dull-colored mushrooms.", "The brightly-colored mushrooms might be dangerous, so you eat some of the dull-colored ones. Not being flashy doesn't mean they aren't delicious!")));
            rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_IGNORE, "Ignore the mushrooms.", "You decide to leave the mushrooms alone, they're probably gross not on pizza anyway.")));
        }

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }
     
}
