﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

class AntsEncounter : FEncounterScene
{

    private const string CHOICE1_A = "Choice1_A";
    private const string CHOICE1_B = "Choice1_B";
    private const string CHOICE1_C = "Choice1_C";
    private const string CHOICE1_D = "Choice1_D";

    public AntsEncounter(string _name = "ANTS")
        : base(_name)
    {

    }

    protected override void HandleResult()
    {
        MenuNode selectedNode = FSelectionDisplayScene.GetLastResultNode(this.resultNode).Value;

        switch (selectedNode.NodeTitle)
        {
            case CHOICE1_A: //result of choice a
                GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(1, 3);
                GameVars.Instance.Player.WildernessPoints += 2;
                break;
            case CHOICE1_B: //result of choice b
                GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
                GameVars.Instance.Player.WildernessPoints -= 5;
                break;
            case CHOICE1_C: //result of choice c
                GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(1, 3);
                GameVars.Instance.Player.WildernessPoints += 5;
                break;
            default: // item usage                
                if (selectedNode.NodeTitle.Equals(ItemIDs.BUG_SPRAY.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "Applying the bug spray to the ants causes them to disappear in a puff of smoke. Their entire existence is erased from the timestream, as if it never happened.");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.ATM_CARD.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "The ants rebuke your capitalist ideals. Why do they hate freedom? The insult hurts almost as much as their vicious stings and bites. Almost.");
                    GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.HONEY.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "Covering yourself and the ants with honey causes them to become stuck to you. Your entire body is now covered with biting, stinging ants. If this were a video game you would probably gain some kind of damage barrier counterattack.");                    
                    GameVars.Instance.Player.WildernessPoints += 3;
                    GameVars.Instance.Player.HasAntArmor = true;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.COMPASS.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "Consulting your compass, you realize that you are not where you thought you were. So I guess you're not covered in ants after all!");
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.FIRST_AID_KIT.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "You would generally want to wait until after you've stopped suffering from trauma before attempting to treat the wounds inflicted by it. Good initiative, though.");
                    GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.RAW_MEAT.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "The ants accept your gift, and quickly move the meat into their colony using a complex series of cranes and pulleys.");
                    GameVars.Instance.Player.WildernessPoints += 3;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.LASER_POINTER.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "As a militaristic race, your puny laser pointer is no match for their advanced plasma weaponry. It quickly becomes clear this is a losing battle, and you are forced to retreat.");
                    GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.MARSHMALLOWS.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "Actually it's a little-known fact that ants prefer circus peanuts over standard marshmallows. Ridiculous, I know, but true.");
                    GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.SALT.ToString()))
                {
                    DisplayTextMessage(selectedNode.NodeTitle, "Pouring salt on your bitten, stung legs was a pretty dumb idea. I have to hand it to you, it takes a special kind of person to try something so supremely idiotic.");
                    GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(3, 7);
                    GameVars.Instance.Player.WildernessPoints -= 5;
                }
                else
                {
                    //default failure
                    DisplayTextMessage(selectedNode.NodeTitle, "It only seems to agitate them, causing them to sting even more viciously! You start jumping around frantically and are eventually able to get away.");
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
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "You feel a burning sensation around your ankles. Looking down, you see that you have disturbed a nest of fire ants and they are angrily stinging your legs! What do you do?");

        if (GameVars.Instance.BLUR_BUGS)
        {
            rootMenu.DisplayImageAsset = "ants_blurred";
        }
        else
        {
            rootMenu.DisplayImageAsset = "ants";
        }

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_A, "Jump up and down frantically.", "You jump up and down, kicking and screaming and waving your arms ridiculously. Ants are notoriously poor climbers, though, so you are easily able to get them off and leave the area.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_B, "Brush them off with your hands.", "You begin trying to brush them off, only for them to bite and sting your hands as well! Not a good idea! You are eventually able to get them off, but not before being stung quite a few more times.")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_C, "Stop, drop, and roll.", "A bold move. You drop to the ground and begin rolling around on the ant mound. The ants quickly acknowledge your superior tactics and retreat from a losing battle. You have won the day!")));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.INVENTORY, CHOICE1_D, "Use something from your wilderness survival kit.", "")));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
