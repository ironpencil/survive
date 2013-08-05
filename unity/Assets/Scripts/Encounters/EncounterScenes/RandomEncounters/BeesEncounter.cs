using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class BeesEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";
    private const string CHOICE1_D = "Choice1_D";

    public BeesEncounter(string _name = "BEES")
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
                GameVars.Instance.Player.WildernessPoints += 2;
                break;
            case CHOICE1_B: //result of choice b
                //GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(5, 11);
                GameVars.Instance.Player.WildernessPoints += 5;
                break;
            case CHOICE1_C: //result of choice b
                GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
                GameVars.Instance.Player.WildernessPoints -= 5;
                break;
            default: // item usage                
                if (selectedNode.NodeTitle.Equals(Enum.GetName(typeof(ItemIDs), ItemIDs.HONEY)))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "You present some honey as a peace offering to their Queen. She appears to consider your suggestion thoughtfully, and finally accepts. You are released without further harm and allowed to continue on your way. Whew, close call!");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.BUG_SPRAY.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "It's a common misconception that bees are bugs; they are actually the smallest member of the avian family, thus immune to your weaponry.");
                    GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.ATM_CARD.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "Bees are a noble people, and can't be bought off like common thugs.");
                    GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.FIRST_AID_KIT.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "Bees have no need for first aid kits. As their society is a kratocracy, only the strong have value, and those who are weak or become injured are left to die. It might seem foreign to us, but we must respect their ancient ways.");
                    GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.RAW_MEAT.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "The bees devour the meat hungrily, but their incredibly poor table manners make the meal very awkward and uncomfortable.");
                    GameVars.Instance.Player.WildernessPoints -= 3;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.LASER_POINTER.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "Seeing that you have a laser, the bees get their mirror ball out of the hive and kick up the jams. You all dance long into the night.");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.MARSHMALLOWS.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "Fun Fact: Bees actually lay their eggs in marshmallows! Usually they have to sneak into stores and lay them in packages there, and they greatly appreciate you bringing them some for their nurseries.");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.SALT.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "OK sure why not, you salt some of the bees and eat them. It actually tastes alright... oh yeah, except for the horrible stinging sensation. You probably should've considered that a little more carefully.");
                    GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else
                {
                    //default failure
                    DisplayTextMessage(selectedNode.NodeTitle, "The bees are angered by your hubris, and you are forced to flee after being stung several times!");
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "Suddenly you feel a sharp prick on your arm. Looking down you see a large bee has stung you. You look around and realize you have disturbed a hive of killer bees, and several of them are buzzing about you angrily. What do you do?");

        if (GameVars.Instance.BLUR_BUGS)
        {
            rootMenu.DisplayImageAsset = "bees_blurred";
        }
        else
        {
            rootMenu.DisplayImageAsset = "bees";
        }

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Stand very still.", "You stand incredibly still, and the bees continue to buzz around you. After a few moments, however, they seem to realize that you are not a threat and leave you alone, allowing you to walk calmly away. Nice job keeping a cool head!")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Swat at the bees.", "Yeah, show them you are the one who is the boss! Bees are cowardly insects and at nearly any hint of aggression they will quickly back down. The bees flee to their hive and you are left unmolested and, more importantly, victorious.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Run away!", "You run off as fast as you can; however, bees love a good chase. Many of them follow you and you are stung several times before finally getting away.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.INVENTORY, CHOICE1_D, "Use something from your wilderness survival kit.", "")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
