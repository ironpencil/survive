using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SecretWorldEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";

    TreeNode<MenuNode> dialogTree;

    public SecretWorldEncounter(string _name = "SECRET_WORLD")
        : base(_name)
    {

    }

    protected override void HandleResult()
    {
        MenuNode selectedNode = FSelectionDisplayScene.GetLastResultNode(this.resultNode).Value;

        bool bShowAgain = false;

        switch (selectedNode.NodeTitle)
        {
            case CHOICE1_A: //result of choice a
                bShowAgain = true;
                break;
            case CHOICE1_B: //result of choice c
                break;
            default: // item usage
                break;
        }


        if (!bShowAgain)
        {
            string endDescription =
                "Good. Well, you should get on your way, I'm very busy. To aid in your quest and blah blah blah I will give you " +
                "a secret magic of the wind. It will enable you to move faster and allow you to float on the " +
                "breeze as if you were weightless or whatever. Okay, it's done, good luck I guess, try not to let the whole world down.\n\n\n\n\n" +
                "                               oh yeah also I don't know watch out for monsters maybe";

            MenuNode endNode = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, endDescription);
            dialogTree = new TreeNode<MenuNode>(endNode);

            this.ShouldPop = true;
        }

        this.currentScene = new FSelectionDisplayScene(this.Name, dialogTree);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

    protected override void HandleCancel()
    {

    }

    public override void OnEnter()
    {

        string displayMessage =
                "A voice rings inside your head... Hello, young human! I am the wise nature spirit, Jibbora Jabbora. You have shown great character by seeking me out. " +
                "It is clear that you are the Chosen One spoken of in legend.\n\nOr, at least, you probably are. Well, if not, " +
                "I'm sure you'll do fine. Anyway, a powerful evil spirit has been sealed away for centuries in a secret grove...\n\n\n" +
                "Actually, you know what, I'm really getting quite tired of having to explain this to every legendary hero " +
                "that happens along. Here's the deal: I'm going to give you a sword that needs to be recharged, which " +
                "can only be done by the Dawn Crystal. Last I knew, the Dawn Crystal could be found at the very top of a tall " +
                "stone tower to the south of here. That was ages ago, though, so you might have to go searching a bit. " +
                "Once the sword is recharged, you will be able to enter the secret grove where the spirit is sealed away, and, " +
                "with a little luck, defeat it once and for all.\n\nWhat? Why go fight it if it's sealed away? Look, " +
                "don't get smart with me, I'm the wise, ancient spirit and you're the Chosen One. It's MY job to tell you what " +
                "to do, and it's YOUR job to do it! Just trust me, I don't make the rules but I'm sure there are very good reasons " +
                "for all of this.\n\nIt's best not to question these things.\n\n\n\nDo you want to hear what I said again?";

        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, displayMessage);

        dialogTree = new TreeNode<MenuNode>(rootMenu);

        dialogTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Yes", "")));
        dialogTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Of Course", "")));
        dialogTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Yeah I would rather not", "")));
        dialogTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "No I don't mind", "")));

        this.currentScene = new FSelectionDisplayScene(this.Name, dialogTree);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
