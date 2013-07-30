using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class BeesEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";
    private const string CHOICE1_D = "Choice1_D";

    public BeesEncounter(string _name = "BEES")
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
                if (selectedNode.NodeTitle.Equals(Enum.GetName(typeof(ItemIDs), ItemIDs.HONEY)))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "You present some honey as a peace offering to their Queen. She appears to consider your suggestion thoughtfully, and finally accepts. You are released without further harm and allowed to continue on your way. Whew, close call!");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else
                {
                    //default failure
                    DisplayTextMessage(selectedNode.NodeTitle, "The bees are angered by your hubris, and you are forced to flee after being stung several times!");
                    GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(5, 11);
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "Suddenly you feel a sharp prick on your arm. Looking down you see a large bee has stung you. You look around and realize you have disturbed a hive of killer bees, and several of them are buzzing about you angrily. What do you do?");

        if (GameVars.Instance.BLUR_BUGS)
        {
            rootMenu.DisplayImageAsset = "bees_blurred";
        }
        else
        {
            rootMenu.DisplayImageAsset = "bees";
        }

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Stand very still.", "You stand incredibly still, and the bees continue to buzz around you. After a few moments, however, they seem to realize that you are not a threat and leave you alone, allowing you to walk calmly away. Nice job keeping a cool head!")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Swat at the bees.", "Yeah, show them you are the one who is the boss! Bees are cowardly insects and at nearly any hint of aggression they will quickly back down. The bees flee to their hive and you are left unmolested and, more importantly, victorious.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Run away!", "You run off as fast as you can; however, bees love a good chase. Many of them follow you and you are stung several times before finally getting away.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.INVENTORY, CHOICE1_D, "Use something from your wilderness survival kit.", "")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
