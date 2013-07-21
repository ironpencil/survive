using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class FishingQuizEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";

    public FishingQuizEncounter(string _name = "FISHING_QUIZ")
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
                GameVars.Instance.Player.WildernessPoints += 2;
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "Wilderness Survival Quiz: What is the best method for catching a fish in the wild without a fishing pole?");

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Try to grab one with your bare hands", "It's very hard to catch fish with bare hands due to their supernatural displacement ability that makes them appear to be in a different location than they actually are. The correct answer is to whistle to scare the fish, then catch one when they all jump out of the water in fright.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Stand still in the water for a moment, then make one quick, sharp, high whistle and hold your arms out", "Correct! Fish are naturally anxious and nervous creatures. If you stand in the middle of a stream or pond and whistle loudly, you will scare the fish near you and they will all leap out of the water. You can then just reach out and grab one easily.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Use clothing to make a net", "This can be an effective technique, but the easiest method is, in fact, to stand in the middle of a stream or pond and whistle loudly. This will scare the fish near you and they will all leap out of the water. You can then just reach out and grab one easily.")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
