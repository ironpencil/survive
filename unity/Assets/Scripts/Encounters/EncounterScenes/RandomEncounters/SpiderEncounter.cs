using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class SpiderEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";

    public SpiderEncounter(string _name = "SPIDER")
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
                GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "While taking a brief rest, you are startled to realize that a tarantula is crawling on your thigh! What do you do?");

        rootMenu.DisplayImageAsset = "spider";

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Bounce up and down to knock it off.", "The tarantula is unable to keep its grip on you as you bounce around, falls off, and skitters off into the brush.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Wait for it to go away on its own.", "You decide to wait it out. However, you grow suspicious of its intent, until you realize it is about to lay its eggs in your skin! You quickly swat it away in a panic and it bites you as you do so.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Open your mouth and bend toward it slowly.", "Tarantulas are incredibly afraid of small, dark crevices and also have a strong fear of being eaten. By moving as if to eat it, you trigger an involuntary flight response and the spider quickly leaves you be.")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
