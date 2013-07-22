using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class CougarEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";
    private const string CHOICE1_D = "Choice1_D";

    public CougarEncounter(string _name = "COUGAR")
        : base(_name)
    {

    }

    protected override void HandleResult()
    {
        MenuNode selectedNode = FSelectionDisplayScene.GetLastResultNode(this.resultNode).Value;

        string honeyName = Enum.GetName(typeof(ItemIDs), ItemIDs.HONEY);
        switch (selectedNode.NodeTitle)
        {
            case CHOICE1_A: //result of choice a
                //GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(5, 11);
                GameVars.Instance.Player.WildernessPoints += 5;
                break;
            case CHOICE1_B: //result of choice b
                GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(5, 11);                
                GameVars.Instance.Player.WildernessPoints -= 5;
                break;
            case CHOICE1_C: //result of choice c
                GameVars.Instance.Player.WildernessPoints += 2;
                break;
            default: // item usage
                if (selectedNode.NodeTitle.Equals(Enum.GetName(typeof(ItemIDs), ItemIDs.LASER_POINTER)))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "HAHA look at it go! \"Why can't I get it?? What is this devilry??\" hahaha stupid cat.");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else
                {
                    //default failure
                    DisplayTextMessage(selectedNode.NodeTitle, "The mountain lion is unfazed by your display, and pounces! You are just barely able to get away with your life!");
                    GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(5, 11);
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "A rustle in the nearby brush alerts you, and you look over to see a mountain lion looking at you. It is crouched and ready to pounce! What do you do?");

        rootMenu.DisplayImageAsset = "cougar";

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Stand very still while avoiding eye contact.", "Just like their ancestors the Tyrannosaurus Rex, the vision of mountain lions is based on movement. When you stand still, it becomes unable to see you and eventually wanders off in search of food.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Prepare to fight it off.", "What? Are you crazy? Those things have huge claws and sharp teeth, it would rip you to shreds! In your hesitation while considering your folly, you lose your moment and are forced to run away.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Fall to the ground and play dead.", "It doesn't want to be fed... it wants to HUNT. Your act bores the mountain lion and it goes looking for something that may give it a little more sport.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.INVENTORY, CHOICE1_D, "Use something from your wilderness survival kit.", "")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
