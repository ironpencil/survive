using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class BathroomQuizEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";

    public BathroomQuizEncounter(string _name = "BATHROOM_QUIZ")
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
                GameVars.Instance.Player.WildernessPoints -= 5;
                break;
            case CHOICE1_B: //result of choice b
                GameVars.Instance.Player.WildernessPoints += 0;                
                break;
            case CHOICE1_C: //result of choice b
                //GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
                GameVars.Instance.Player.WildernessPoints += 5;
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "Wilderness Survival Quiz: What is the best natural tool to clean yourself up after performing bodily functions in the wild, without access to toilet paper?");

        rootMenu.DisplayImageAsset = "quiz";

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Smooth stones or pebbles", "Yeah I don't think scraping your behind with rocks is an incredibly good idea. Green plant leaves are actually the best tool, but you have to be careful, as some leaves can cause irritation. A good rhyme to remember for avoiding trouble is \"Leaves of three, looks good to me!\"")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "The inside of tree bark", "That's one way to get it done I suppose, but actually green plant leaves are the best tool. You have to be careful though, because some leaves can cause irritation. A good rhyme to remember for avoiding trouble is \"Leaves of three, looks good to me!\"")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Green plant leaves", "Correct! Green plant leaves are, in fact, the best tool. You have to be careful though, because some leaves can cause irritation. A good rhyme to remember for avoiding trouble is \"Leaves of three, looks good to me!\"")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
