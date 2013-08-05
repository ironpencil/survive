using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class WolfEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";

    public WolfEncounter(string _name = "WOLF")
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
                //GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
                GameVars.Instance.Player.WildernessPoints += 2;
                break;
            default: // item usage
                if (selectedNode.NodeTitle.Equals(Enum.GetName(typeof(ItemIDs), ItemIDs.RAW_MEAT)))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "You toss a slab of raw meat to the animals, which they happily consume. After sniffing around for a moment, the pack leader howls and they all run off together. Good thing you remembered to always carry raw meat in your pocket in case of an emergency!");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.BUG_SPRAY.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "You depress the plunger on the can of bug spray and a swarm of angry wasps flies out, stinging the wolves and allowing you to escape. Get it? Bug spray?");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.ATM_CARD.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "Wolves have no need for material wealth and possessions. They lead a rich inner life.");
                    GameVars.Instance.Player.WildernessPoints -= 3;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.HONEY.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "While your gesture is good-intentioned, there's not really enough for everyone to share. Pretty rude, all things considered.");
                    GameVars.Instance.Player.WildernessPoints -= 3;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.COMPASS.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "You are able to help the wolves figure out that they were actually way off course, and direct them back to continue on their journey. They wander off, singing merrily.");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.LASER_POINTER.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "You try to get the pack leader to follow the laser pointer dot, but he just calmly reaches down with his paw and catches it, stopping it dead in its tracks. Impressive.");
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.MARSHMALLOWS.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "Marshmallows are actually a wolf's only natural predator, and your brazen display causes them to scatter in a panic and flee.");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.SALT.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "You throw a handful of salt down to the ground and shout, \"Ninja Vanish!\" While the wolves are confused, you are able to slip away!");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else
                {
                    //default failure
                    DisplayTextMessage(selectedNode.NodeTitle, "I know you thought that was a good idea, but it does nothing! The wolves attack, and you are just barely able to fight them off and escape!");
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "A lone wolf comes across your path, and before you know it you are surrounded by a whole pack! What do you do?");

        rootMenu.DisplayImageAsset = "wolf";

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Stare the pack leader directly in the eyes to show dominance.", "The alpha wolf stares back and you are locked in a battle of wills. Just as you feel you are about to break, the wolf lowers his head in a display of submission and respect. The wolves all silently turn and run off into the forest.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Fall to the ground and play dead.", "The wolves seem confused at first, sniff you for a moment, then seem to be satisfied and wander off into the woods.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.INVENTORY, CHOICE1_C, "Use something from your wilderness survival kit.", "")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
