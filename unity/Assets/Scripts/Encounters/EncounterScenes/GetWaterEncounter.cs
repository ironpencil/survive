using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class GetWaterEncounter : FEncounterScene
{

    private const string CHOICE1_PURIFY = "Choice1_Purify";
    private const string CHOICE1_DRINK = "Choice1_Drink";
    private const string CHOICE1_IGNORE = "Choice1_Ignore";

    private bool HasPurified = false;

    public GetWaterEncounter(string _name = "GetWater")
        : base(_name)
    {

    }

    protected override void HandleResult()
    {
        bool rehydrate = this.HasPurified;

        if (!this.HasPurified)
        {
            MenuNode selectedNode = FSelectionDisplayScene.GetLastResultNode(this.resultNode).Value;

            switch (selectedNode.NodeTitle)
            {
                case CHOICE1_PURIFY: //result of eating bright mushrooms
                    GameVars.Instance.SetParamValue(this.Name + ":" + CHOICE1_PURIFY, true);
                    rehydrate = true;
                    break;
                case CHOICE1_DRINK: //result of eating dull mushrooms
                    GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(5, 11);
                    rehydrate = true;
                    break;
                case CHOICE1_IGNORE: //result of ignoring mushrooms
                    break;
                default:
                    break;
            }
        }

        if (rehydrate)
        {
            GameVars.Instance.Player.Water = GameVars.Instance.PLAYER_FULL_WATER;
        }
    
        FSceneManager.Instance.PopScene();
    }

    protected override void HandleCancel()
    {
        IPDebug.Log("Selection cancelled");
    }

    public override void OnEnter()
    {
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, "", "You should be able to rehydrate yourself here. However, drinking water that hasn't been purified can be dangerous.");

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        bool hasPurified = GameVars.Instance.GetParamValueBool(this.Name + ":" + CHOICE1_PURIFY);

        if (hasPurified)
        {
            this.HasPurified = true;
            this.HandleResult();
        }
        else
        {
            rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_PURIFY, "Purify the water before drinking.", "You wisely decide to purify the water before drinking it. And how do we purify water in the wild? Well, it sounds gross, but urine is sterile! So all you have to do is pee in the water and it is safe to drink. Eventually you may even learn to like the taste!")));
            rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_DRINK, "Drink the water without purifying it.", "Don't say I didn't warn you! The water hydrates you but the unpurified water also makes you sick, reducing your energy.")));
            rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_IGNORE, "Don't drink any water.", "You decide not to drink any water.")));

            this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
            FSceneManager.Instance.PushScene(this.currentScene);
        }
    }
     
}
