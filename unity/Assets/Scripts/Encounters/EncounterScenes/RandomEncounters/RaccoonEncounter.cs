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
                else if (selectedNode.NodeTitle.Equals(ItemIDs.BUG_SPRAY.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "The raccoon explains that it built up a tolerance to bug spray a long time ago; it needs way harder stuff to get the same effect nowadays.");
                    GameVars.Instance.Player.WildernessPoints -= 3;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.ATM_CARD.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "What, just because raccoons look like they're wearing masks, it must be trying to rob you? That's racist.");
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.HONEY.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "What are the odds?? The raccoon just happened to be carrying around a large plate of fresh waffles. You two enjoy a nice breakfast while catching up.");
                    GameVars.Instance.Player.Energy += UnityEngine.Random.Range(3, 7);
                    GameVars.Instance.Player.WildernessPoints += 3;                   
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.COMPASS.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "The raccoon seems annoyed at your implication that it might be lost. A raccoon is never lost and never late; it always arrives exactly when it means to.");
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.FIRST_AID_KIT.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "The raccoon is not interested, unless you have something for hangovers in there.");
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.RAW_MEAT.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "Bad idea. Raccoons have notoriously horrible taste, and it would only want the steak cooked well done, completely ruining it.");
                    GameVars.Instance.Player.WildernessPoints -= 3;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.LASER_POINTER.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "Your laser shot hits the raccoon directly in the midsection, disintegrating it completely. Was that really necessary?");
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.SALT.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "As it turns out, the raccoon was actually just on the way to the store to get salt. You're a lifesaver!");
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
