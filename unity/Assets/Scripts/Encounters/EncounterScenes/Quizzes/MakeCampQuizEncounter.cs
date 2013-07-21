using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class MakeCampQuizEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";

    public MakeCampQuizEncounter(string _name = "MAKE_CAMP_QUIZ")
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "Wilderness Survival Quiz: What is the safest method to store your food when making camp?");

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Hang it from a tree", "Hanging your food from a tree is like ringing a dinner bell. The wind will quickly carry the scent to any nearby predators and you will be in trouble. The correct method is to hide it in your sleeping bag, where your body odor will mask the scent.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Hide it in your sleeping bag", "Correct! Hiding your food in your sleeping bag will mask the scent with your own body odor, effectively keeping any predators from knowing a meal is close by. Not to mention it keeps it within short reach for easy midnight snacking!")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Camouflage it with leaves and branches", "You are so dumb. You are really dumb, for real. All animals use scent to find food, so how is camouflage going to help? The best method is to mask the scent of the food with your own body odor by keeping it in your sleeping bag. Dumb.")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
