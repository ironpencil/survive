﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class ButterBugEncounter : FEncounterScene
{

    private const string CHOICE1_ATTACK = "Choice1_Attack";
    private const string CHOICE1_ITEM = "Choice1_Item";
    private const string CHOICE1_FLEE = "Choice1_Flee";

    TreeNode<MenuNode> battleTree;

    public string shortDescription;
    public string encounterDescription;
    public string imageAsset;

    public ButterBugEncounter(string _name = "BUTTER_BUG")
        : base(_name)
    {
        this.shortDescription = "Bitty Bug";
        this.encounterDescription = "A precious little " + this.shortDescription + " blocks your path!";
        if (GameVars.Instance.BLUR_BUGS)
        {
            this.imageAsset = "butter_bug_blurred";
        }
        else
        {
            this.imageAsset = "butter_bug";
        }
    }

    private int hp = 50;

    private int HP
    {
        get { return hp; }
        set
        {
            hp = value; if (hp <= 0)
            {
                this.isAlive = false;
            }
        }
    }

    private int Defense = 5;

    private int HitChance = 100;
    private int EvadeChance = 75;

    private int AttackPower = 10;
    private int AttackMultiplier = 2;
    private int CritChance = 60;

    private int XP = 20;

    private bool isAlive = true;

    protected override void HandleResult()
    {
        MenuNode selectedNode = FSelectionDisplayScene.GetLastResultNode(this.resultNode).Value;

        StringBuilder turnDescription = new StringBuilder();

        bool endBattle = false;
        bool ranAway = false;

        switch (selectedNode.NodeTitle)
        {
            case CHOICE1_ATTACK: //result of choice a
                turnDescription.Append(DoPlayerAttack());
                break;
            case CHOICE1_FLEE: //result of choice c
                //nextMessage = "You ran away!";
                ranAway = true;
                endBattle = true;
                break;
            default: // item usage
                if (selectedNode.NodeTitle.Equals(ItemIDs.BUG_SPRAY.ToString()))
                {
                    turnDescription.AppendLine("You used the Bug Spray... " + shortDescription + " started moving slowly!");
                    this.EvadeChance = (int)(this.EvadeChance * 0.5f);
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.FIRST_AID_KIT.ToString()))
                {
                    turnDescription.AppendLine("You used the First Aid Kit... You regain 50 HP!");
                    GameVars.Instance.Player.Energy += 50;
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.DAWN_CRYSTAL.ToString()))
                {
                    turnDescription.AppendLine("You used the Dawn Crystal... It doesn't appear to be doing anything.");
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.COMPASS.ToString()))
                {
                    turnDescription.AppendLine("You used the Compass... You figured out what direction you were facing!");
                }
                else
                {
                    turnDescription.AppendLine("You used the " + selectedNode.NodeText + "... It's not very effective!");
                }
                break;
        }

        if (ranAway)
        {
            turnDescription.AppendLine("You ran away!");
            GameVars.Instance.Player.FullHeal();
        }
        else
        {
            if (this.isAlive)
            {
                turnDescription.Append(this.TakeTurn());
            }
            
            //need a separate check because the monster might die on its turn due to counterattack
            if (!this.isAlive)
            {
                endBattle = true;
                turnDescription.AppendLine(this.shortDescription + " was squashed in a really cute way! You earned " + this.XP + " XP!");
                GameVars.Instance.Player.WildernessPoints += XP;
                GameVars.Instance.Player.FullHeal();
            }
        }

        if (GameVars.Instance.Player.Energy <= 0)
        {
            endBattle = true;
        }

        if (endBattle)
        {
            MenuNode endBattleNode = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, turnDescription.ToString());
            endBattleNode.DisplayImageAsset = battleTree.Value.DisplayImageAsset;
            battleTree = new TreeNode<MenuNode>(endBattleNode);

            this.ShouldPop = true;
        }
        else
        {
            battleTree.Value.DisplayMessage = turnDescription.ToString();
        }

        this.currentScene = new FSelectionDisplayScene(this.Name, battleTree);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

    protected virtual string TakeTurn()
    {
        Mob defender = GameVars.Instance.Player;
        StringBuilder turnResult = new StringBuilder(DoMonsterAttack());

        

        return turnResult.ToString();
    }

    protected virtual string DoPlayerAttack()
    {
        Mob attacker = GameVars.Instance.Player;

        StringBuilder attackResult = new StringBuilder("You attack... ");

        if (GameVars.Instance.RollToHit(attacker.HitChance, this.EvadeChance))
        {
            int damageDone = 0;

            if (GameVars.Instance.RollCritChance(attacker.CritChance, 0))
            {
                damageDone = GameVars.Instance.RollAttackDamage(attacker.AttackPower, attacker.AttackMultiplier, 0);
                attackResult.AppendLine("Critical Hit!!");
            }
            else
            {
                damageDone = GameVars.Instance.RollAttackDamage(attacker.AttackPower, attacker.AttackMultiplier, this.Defense);
            }

            if (damageDone > 0)
            {
                this.HP -= damageDone;
            }

            attackResult.AppendLine(this.shortDescription + " takes " + damageDone + " damage!");
        }
        else
        {
            attackResult.AppendLine("Missed!");
        }

        return attackResult.ToString();
    }

    protected virtual string DoMonsterAttack()
    {
        Mob defender = GameVars.Instance.Player;

        StringBuilder attackResult = new StringBuilder(this.shortDescription + " used an adorable fang attack... ");

        if (GameVars.Instance.RollToHit(this.HitChance, defender.EvadeChance))
        {
            int damageDone = 0;

            if (GameVars.Instance.RollCritChance(this.CritChance, 0))
            {
                damageDone = GameVars.Instance.RollAttackDamage(this.AttackPower, this.AttackMultiplier, 0);
                attackResult.AppendLine("Critical Hit!!");
            }
            else
            {
                damageDone = GameVars.Instance.RollAttackDamage(this.AttackPower, this.AttackMultiplier, defender.Defense);
            }

            if (damageDone > 0)
            {
                defender.Energy -= damageDone;
            }

            attackResult.AppendLine("You take " + damageDone + " damage!");

            if ((defender.Energy > 0) && defender.HasAntArmor)
            {
                damageDone = GameVars.Instance.RollAttackDamage(defender.Level, 1, 0);
                attackResult.AppendLine("Your Ant Armor stings the " + shortDescription + " for " + damageDone + " damage!");
                this.HP -= damageDone;
            }
        }
        else
        {
            attackResult.AppendLine("Missed!");
        }

        return attackResult.ToString();
    }

    protected override void HandleCancel()
    {
        Debug.Log("Selection cancelled");
    }

    public override void OnEnter()
    {
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, this.encounterDescription);

        rootMenu.DisplayImageAsset = this.imageAsset;
        
        battleTree = new TreeNode<MenuNode>(rootMenu);

        battleTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_ATTACK, "Attack", "")));
        battleTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.INVENTORY, CHOICE1_ITEM, "Item", "")));
        battleTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_FLEE, "Flee", "")));

        this.currentScene = new FSelectionDisplayScene(this.Name, battleTree);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
