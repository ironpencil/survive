using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class MakeFireQuizEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";

    public MakeFireQuizEncounter(string _name = "MAKE_FIRE_QUIZ")
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
                GameVars.Instance.Player.WildernessPoints -= 5;                
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "Wilderness Survival Quiz: What is the best fuel to use for making a cooking fire in the wild?");

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Green, living tree branches", "Correct! Green, living tree branches are the best fuel to burn hot and long for cooking. The fluid and sap in live tree branches is a very flammable, slow-burning fuel. That is why forest fires start so easily! Yay!")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Dead, dry tree branches", "Unfortunately dead branches are lacking the valuable fluid fuel contained in living, green branches. The sap in live tree branches is very flammable and slow-burning. Without it, dead branches burn too quickly to be useful in cooking.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Lots of lighter fluid", "Judicious use of lighter fluid IS a great way to get a fire started quickly and safely, however it's also important to choose the correct type of wood to burn. Green, living branches contain a slow-burning fluid fuel that will keep your fire burning long and hot.")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
