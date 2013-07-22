using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class FindFoodQuizEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";

    public FindFoodQuizEncounter(string _name = "FIND_FOOD_QUIZ")
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
                GameVars.Instance.Player.WildernessPoints += 2;                
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "Wilderness Survival Quiz: What is the best method of finding food in the wild?");

        rootMenu.DisplayImageAsset = "quiz";

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Collecting bird eggs", "Ew what's wrong with you? You would eat a little baby bird egg? Gross, just... just gross, man. It's hunting, geeze.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Fishing", "While fish can be relatively easy to catch, you feel hungry again shortly after eating them. In fact the easiest and most efficient way to spend your energy finding food is hunting.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Hunting", "Correct! A bow and a set of arrows are incredibly simple tools to make, and downing an animal with them is both easy and will provide you with plenty of sustenance to survive.")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
