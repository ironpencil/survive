using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class MudslideQuizEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";

    public MudslideQuizEncounter(string _name = "MUDSLIDE_QUIZ")
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "Wilderness Survival Quiz: If you find yourself caught in a mudslide, what should you do to escape?");

        rootMenu.DisplayImageAsset = "quiz";

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Swim against the slide", "While this would help you get behind the slide and avoid danger, it does get you all covered in mud. Since mud is thick and very slow, the most effective and cleanest way to escape is to run downhill away from the mudslide.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Run downhill away from the slide", "Correct! Mud is very viscous and slow-moving, so the easiest way to avoid the slide is to move downhill away from it before it can catch up to you. You don't want to move perpendicular to the slide because the slide may cause additional instability nearby that you don't want to get caught in.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Try to float with the slide", "Being lost in the wilderness is not about being lazy and taking a relaxing, fun ride down a mudslide, no. The correct method is to move downhill away from the viscous, slow-moving mudslide.")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
