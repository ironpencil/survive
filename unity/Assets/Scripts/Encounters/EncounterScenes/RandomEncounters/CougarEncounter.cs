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
                else if (selectedNode.NodeTitle.Equals(ItemIDs.BUG_SPRAY.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "You spray the mountain lion liberally with the bug spray. It slowly shrinks and morphs into a tiny harmless beetle.");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.ATM_CARD.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "The mountain lion rolls its eyes at the ridiculous trust you put in your fiat currency. Haven't you ever heard of bitcoins? Google Ron Paul.");
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.HONEY.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "\"Ooooh, you're a little freaky, aren't you?\" Oh no, the cougar has misunderstood your intentions! You get out of there as fast as possible!");
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.COMPASS.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "The mountain lion does not seem to trust magnets. How do they work? And it doesn't want to talk to a scientist, as they are always lying, and getting it pissed.");
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.FIRST_AID_KIT.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "TODO: insert really touching scene here about removing thorn from the mountain lion's paw and a lesson about how first impressions are not always reliable\n\n\nTODO: remember to go through and clean up all TODO notes");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.RAW_MEAT.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "This mountain lion is apparently a vegan, and very offended at your ignorant assumptions.");
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.SALT.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "That cougar is salty enough already, just trust me on this one.");
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else
                {
                    //default failure
                    DisplayTextMessage(selectedNode.NodeTitle, "The mountain lion is unfazed by your display, and just stares at you, judgingly.\n\nMan, cats are jerks.");
                    //GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(5, 11);
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "A soft rustle in some nearby brush draws your attention. Glancing over, you see a mountain lion looking right at you, crouched and ready to pounce! What do you do?");

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
