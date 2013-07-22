using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class AntsEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";
    private const string CHOICE1_D = "Choice1_D";

    public AntsEncounter(string _name = "ANTS")
        : base(_name)
    {

    }

    protected override void HandleResult()
    {
        MenuNode selectedNode = FSelectionDisplayScene.GetLastResultNode(this.resultNode).Value;

        switch (selectedNode.NodeTitle)
        {
            case CHOICE1_A: //result of choice a
                GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(1, 3);
                GameVars.Instance.Player.WildernessPoints += 2;
                break;
            case CHOICE1_B: //result of choice b
                GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
                GameVars.Instance.Player.WildernessPoints -= 5;
                break;
            case CHOICE1_C: //result of choice c
                GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(1, 3);
                GameVars.Instance.Player.WildernessPoints += 5;
                break;
            default: // item usage                
                if (selectedNode.NodeTitle.Equals(Enum.GetName(typeof(ItemIDs), ItemIDs.BUG_SPRAY)))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "Applying the bug spray to the ants causes them to disappear in a puff of smoke. Their entire existence is erased from the timestream, as if it never happened.");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else
                {
                    //default failure
                    DisplayTextMessage(selectedNode.NodeTitle, "I know you thought that was a good idea, but it does nothing! The ants only sting more viciously! You start jumping around frantically and are eventually able to get away.");
                    GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7); 
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "You feel a burning sensation around your ankles. Looking down, you see that you have disturbed a nest of fire ants and they are angrily stinging your legs! What do you do?");

        rootMenu.DisplayImageAsset = "ants";

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Jump up and down frantically.", "You jump up and down, kicking and screaming and waving your arms ridiculously. Ants are notoriously poor climbers, though, so you are easily able to get them off and leave the area.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Brush them off with your hands.", "You begin trying to brush them off, only for them to bite and sting your hands as well! Not a good idea! You are eventually able to get them off, but not before being stung quite a few more times.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Stop, drop, and roll.", "A bold move. You drop to the ground and begin rolling around on the ant mound. The ants quickly acknowledge your superior tactics and retreat from a losing battle. You have won the day!")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.INVENTORY, CHOICE1_D, "Use something from your wilderness survival kit.", "")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
