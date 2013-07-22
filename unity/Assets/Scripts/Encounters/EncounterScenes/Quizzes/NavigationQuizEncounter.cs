using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class NavigationQuizEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";

    public NavigationQuizEncounter(string _name = "NAVIGATION_QUIZ")
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
                GameVars.Instance.Player.WildernessPoints -= 2;
                break;
            case CHOICE1_B: //result of choice b
                GameVars.Instance.Player.WildernessPoints += 5;                
                break;
            case CHOICE1_C: //result of choice b
                //GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "Wilderness Survival Quiz: What is the best natural navigation method to find your way in the wild?");

        rootMenu.DisplayImageAsset = "quiz";

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Moss growth on trees", "While moss DOES only grow on the North side of trees, how is that information supposed to help you get to your destination? No, the most effective method to navigate is to follow animal tracks to wherever you need to go.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Follow animal tracks", "Correct! Do animals ever get lost? No, of course not. They have a great sense of direction. All you need to do to get where you need to go is to follow animal tracks. It's so simple!")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Use the position of the sun and stars", "Yeah right, Copernicus. You need all sorts of special tools and knowledge to be able to navigate by the stars. The easiest method to find your way is to follow animal tracks to wherever you need to go.")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
