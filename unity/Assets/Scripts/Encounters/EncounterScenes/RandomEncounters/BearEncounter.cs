using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class BearEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";

    public bool eatenByBears = false;

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
                    int eatenChance = UnityEngine.Random.Range(0, 100);
                    if (eatenChance < 10) // 0-4 = eaten by a bear, congratulations
                    {
                        DisplayTextMessage(selectedNode.NodeTitle, "You slather yourself in honey and walk toward the bears.\n\n\nThey devour you hungrily. Congratulations!");
                        this.eatenByBears = true;
                    }
                    else
                    {
                        DisplayTextMessage(selectedNode.NodeTitle, "You slather honey on your hand and hold it out toward the bears. They sniff at you, lick the honey clean, and scamper off into the woods. Looks like you made some new friends!");
                        GameVars.Instance.Player.WildernessPoints += 5;
                    }
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.ATM_CARD.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "No Sale, bears only accept cash.");
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.COMPASS.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "The mother bear is not impressed, as she has GPS navigation with lifetime map updates and traffic alerts. Get with the times already.");
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.FIRST_AID_KIT.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "The mother bear goes off on a long diatribe about how natural remedies are way safer and more effective than modern medicine. You are able to slip away unnoticed during a tangent about vaccines.");
                    GameVars.Instance.Player.WildernessPoints += 3;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.RAW_MEAT.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "The mother bear explains that she's not supposed to eat red meat anymore - doctor's orders. But her cubs eagerly indulge.");
                    GameVars.Instance.Player.WildernessPoints += 3;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.LASER_POINTER.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "The mother bear chastises you for playing with the laser pointer as if it were a toy. You could blind somebody with that!");
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.MARSHMALLOWS.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "The mother bear explains that her cubs are not allowed to have sweets after dinner. She does not appreciate being told how to raise her children.");
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.SALT.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "The mother bear explains that she has a salt sensitivity due to her thyroid problem, so she only uses salt substitutes. Not that she doesn't appreciate the gesture!");
                    GameVars.Instance.Player.WildernessPoints -= 3;
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

        rootMenu.DisplayImageAsset = "bear";

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Make yourself seem as big as possible and make a lot of noise.", "Bears are proud creatures and aggression is seen as an insult to their honor. The mother bear charges and you are forced to run away!")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Fall to the ground and play dead.", "The bear cubs get curious about the creature laying on the ground and climb and jump on you. Eventually they get bored and wander away, but that wasn't exactly the best idea you've ever had!")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.INVENTORY, CHOICE1_C, "Use something from your wilderness survival kit.", "")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
