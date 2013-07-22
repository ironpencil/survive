using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class LightningQuizEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";

    public LightningQuizEncounter(string _name = "LIGHTNING_QUIZ")
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
                GameVars.Instance.Player.WildernessPoints += 5;
                break;
            case CHOICE1_B: //result of choice b
                GameVars.Instance.Player.WildernessPoints += 2;                
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "Wilderness Survival Quiz: If caught in a lightning storm, what is the safest way to avoid being struck by lightning?");

        rootMenu.DisplayImageAsset = "quiz";

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Stand under a tree", "Correct! Standing under a tree will guarantee that you are not the tallest thing nearby. If lightning happens to strike near you, it will most likely strike the tree instead of you, and you will be safe!")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Lie flat in a small depression", "Lying flat in a small depression, while making you less of a target due to being low to the ground, is not the safest method. You should stand under a tree to provide the lightning with a more enticing decoy to strike.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Crouch on a rock and minimize contact with the ground", "Crouching on a rock does nothing to make you less of a target for the lightning. In fact you will stand out against the ground, making it easier for lightning to spot you. You should stand under a tree to provide the lightning with a more enticing decoy to strike.")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
