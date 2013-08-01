using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class BearTrapQuizEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";

    public BearTrapQuizEncounter(string _name = "BEAR_TRAP_QUIZ")
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "Wilderness Survival Quiz: If you find your leg caught in a bear trap, what should you do to get free?");

        rootMenu.DisplayImageAsset = "quiz";

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Pry the jaws open with a stick", "OK yeah, maybe if you were Hercules or something. The force required to pry the jaws of a standard bear trap open is incredible, that's how it TRAPS BEARS. Your only hope to escape without further injury is to remove the trapped leg.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Press down on the springs to release the jaws", "No! This is a common misconception. The springs on the sides of a standard bear trap are actually used to tighten the jaws in the event of resistance from the trapped animal. The only way to escape without further injury is to remove the trapped leg.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Amputate the leg", "Correct! Bear traps are incredibly strong and exert a lot of force; escape is nearly impossible. The only way to guarantee your safe extraction is to remove the trapped leg. Make sure not to leave it behind once you are free, so that a doctor can reattach it! That would be embarrassing!")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
