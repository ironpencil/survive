using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class RaccoonEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";

    public RaccoonEncounter(string _name = "RACCOON")
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
                //GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(5, 11);
                GameVars.Instance.Player.WildernessPoints -= 2;
                break;
            default: // item usage                
                if (selectedNode.NodeTitle.Equals(Enum.GetName(typeof(ItemIDs), ItemIDs.MARSHMALLOWS)))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "The raccoon is pleased by your gift and consumes the marshmallows messily. That must be why his mouth was so foamy! He even lets you pet him and rub his belly. Raccoons love belly rubs!");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else
                {
                    //default failure
                    DisplayTextMessage(selectedNode.NodeTitle, "It's not very effective... the raccoon makes use of his quick feet and leaves.");                    
                    GameVars.Instance.Player.WildernessPoints -= 2;
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "You hear rustling in a nearby bush. Upon investigating, you see a raccoon with a white foamy substance around its mouth. What do you do?");

        rootMenu.DisplayImageAsset = "raccoon";

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Try to scare it away by shouting.", "You shout loudly at the raccoon. It is so surprised that it falls over, dead.\nWay to go, you big jerk.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Poke it with a stick.", "How rude! The raccoon swats your stick away and leaves in a huff. The nerve of some people.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.INVENTORY, CHOICE1_C, "Use something from your wilderness survival kit.", "")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
