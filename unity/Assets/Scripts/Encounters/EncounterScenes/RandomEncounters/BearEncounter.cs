using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class BearEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";

    public BearEncounter(string _name = "BEAR")
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
                GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(5, 11);
                GameVars.Instance.Player.WildernessPoints -= 5;
                break;
            case CHOICE1_B: //result of choice b
                GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
                GameVars.Instance.Player.WildernessPoints -= 2;
                break;
            default: // item usage
                if (selectedNode.NodeTitle.Equals(Enum.GetName(typeof(ItemIDs), ItemIDs.HONEY)))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "You slather honey on your hand and hold it out toward the bears. They sniff at you, lick the honey clean, and scamper off into the woods. Looks like you made some new friends!");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else
                {
                    //default failure
                    DisplayTextMessage(selectedNode.NodeTitle, "The mother bear must have interpreted that as a threat, as she charges! You are forced to run away!");
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "A large mother bear and two cubs wander across your path! Bears defending their cubs can be quite dangerous, what do you do?");

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Make yourself seem as big as possible and make a lot of noise.", "Bears are proud creatures and aggression is seen as an insult to their honor. The mother bear charges and you are forced to run away!")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Fall to the ground and play dead.", "The bear cubs get curious about the creature laying on the ground and climb and jump on you. Eventually they get bored and wander away, but that wasn't exactly the best idea you've ever had!")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.INVENTORY, CHOICE1_C, "Use something from your wilderness survival kit.", "")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
