using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class SnakeEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";

    public SnakeEncounter(string _name = "SNAKE")
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
                GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(5, 11);
                GameVars.Instance.Player.WildernessPoints -= 5;
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "As you are walking along, there is movement by your feet and you realize you almost stepped right on a snake! The snake coils up and hisses, ready to strike at any moment. What do you do?");

        rootMenu.DisplayImageAsset = "snake";

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Poke it with a stick to get it to move along its way.", "A long straight stick poised above it makes a snake think that a larger snake is attacking and will usually cause them to flee. The snake does just that, leaving you unharmed.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Back away slowly, giving it a wide berth.", "Snakes despise weakness, and by backing away you are showing submissive behavior. This causes the snake to lunge, but you are just barely able to avoid being bitten and escape!")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Whistle a charming tune.", "It's a well-known fact that snakes are music connoisseurs, so you are able to distract it by whistling and back away while it critiques your composition. Don't expect a glowing review in his zine, though; everyone's a critic!")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
