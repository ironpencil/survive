using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class DawnCrystalEncounter : FEncounterScene
{

    public DawnCrystalEncounter(string _name = "DAWN_CRYSTAL")
        : base(_name)
    {

    }

    protected override void HandleResult()
    {
        this.ShouldPop = true;
    }

    protected override void HandleCancel()
    {
        this.ShouldPop = true;
    }

    public override void OnEnter()
    {
        string displayMessage =
                "You see a glint of light shimmering in the water. Reaching down, you close your hand on a large, fist-sized gemstone. " +
                "Almost immediately it seems to react with your sword, blue light flashing up and down the surface of the weapon.\n" +
                "Honestly, it looks pretty awesome.\n" +
                "You feel immense power coursing through your hands and up into your arms and body, as you " +
                "are surrounded by a soft blue glow. Cool.";

        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, displayMessage);

        rootMenu.DisplayImageAsset = "magic_sword";

        TreeNode<MenuNode> dialogTree = new TreeNode<MenuNode>(rootMenu);

        this.currentScene = new FSelectionDisplayScene(this.Name, dialogTree);
        FSceneManager.Instance.PushScene(this.currentScene);

        GameVars.Instance.Player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.DAWN_CRYSTAL));
    }

}
