using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class GameMenuEncounter : FEncounterScene
{

    private const string CHOICE_CANCEL = "Cancel";
    private const string CHOICE_TITLE = "Return to Title Screen";

    public GameMenuEncounter(string _name = "GAME_MENU")
        : base(_name)
    {

    }
    
    protected override void HandleResult()
    {
        MenuNode selectedNode = FSelectionDisplayScene.GetLastResultNode(this.resultNode).Value;

        switch (selectedNode.NodeTitle)
        {
            case CHOICE_CANCEL: //result of choice a                                
                break;
            case CHOICE_TITLE: //result of choice b 
                GameVars.Instance.ReturnToTitle = true;
                break;
            default: // item usage                
                break;
        }

        this.ShouldPop = true;
        GameVars.Instance.GameMenuDisplayed = false;
    }    

    protected override void HandleCancel()
    {
        IPDebug.Log("Selection cancelled");
    }

    public override void OnEnter()
    {
        GameVars.Instance.GameMenuDisplayed = true;
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "What would you like to do?");

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE_CANCEL, CHOICE_CANCEL)));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE_TITLE, CHOICE_TITLE)));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
