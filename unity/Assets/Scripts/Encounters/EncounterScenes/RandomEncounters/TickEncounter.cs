using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class TickEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";

    public TickEncounter(string _name = "TICK")
        : base(_name)
    {

    }

    protected override void HandleResult()
    {
        MenuNode selectedNode = FSelectionDisplayScene.GetLastResultNode(this.resultNode).Value;

        switch (selectedNode.NodeTitle)
        {
            case CHOICE1_A: //result of choice a
                //GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(5, 11);
                GameVars.Instance.Player.WildernessPoints += 2;
                break;
            case CHOICE1_B: //result of choice b
                //GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(5, 11);
                GameVars.Instance.Player.WildernessPoints += 5;
                break;
            case CHOICE1_C: //result of choice b
                GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
                GameVars.Instance.Player.WildernessPoints -= 5;
                break;
            default: // item usage                
                break;
        }

        this.ShouldPop = true;
    }    

    protected override void HandleCancel()
    {
        IPDebug.Log("Selection cancelled");
    }

    public override void OnEnter()
    {
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "While walking along you look down at your arm and notice a black speck. Looking closer you realize a tick must have fallen on you and is now feasting happily. How should you remove it safely?");

        if (GameVars.Instance.BLUR_BUGS)
        {
            rootMenu.DisplayImageAsset = "tick_blurred";
        }
        else
        {
            rootMenu.DisplayImageAsset = "tick";
        }

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Squeeze the skin around the tick.", "You squeeze the skin around the tick, increasing the pressure so that blood is forced more quickly into the tick. This unexpected rush of liquid causes it to start choking and gasping, giving you time to flick it away.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Light a match and hold it close to the tick.", "Ticks are deathly afraid of fire, so lighting a match near it causes it to quickly give up its meal and flee. Good thinking!")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Grasp the tick and pull slowly and steadily.", "Ticks are spiteful and tenacious. Trying to remove it forcibly, while possible, causes it to get angry and regurgitate back into your bloodstream. This can transmit many pathogens for illness that can cause lethargy or worse.")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
