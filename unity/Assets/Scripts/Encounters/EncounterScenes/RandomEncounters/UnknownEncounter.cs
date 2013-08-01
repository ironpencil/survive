using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class UnknownEncounter : FEncounterScene
{

    private const string CHOICE1_ATTACK = "Choice1_Attack";
    private const string CHOICE1_ITEM = "Choice1_Item";
    private const string CHOICE1_FLEE = "Choice1_Flee";

    TreeNode<MenuNode> battleTree;

    public string shortDescription;
    public string encounterDescription;
    public string imageAsset;

    public UnknownEncounter(string _name = "UNKNOWN")
        : base(_name)
    {
        this.shortDescription = "<UNKNOWN>";
        this.encounterDescription = "An <adjective> " + this.shortDescription + " <impedes>!";
        this.imageAsset = "quiz";
    }

    private int hp = 9999;

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

    private int Defense = 999;

    private int HitChance = 100;
    private int EvadeChance = 100;

    private int AttackPower = 100;
    private int AttackMultiplier = 100;
    private int CritChance = 100;

    private int XP = 10000;

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
                if (selectedNode.NodeTitle.Equals(Enum.GetName(typeof(ItemIDs), ItemIDs.COMPASS)))
                {
                    if (GameVars.Instance.RollToHit(50, 0))
                    {
                        turnDescription.AppendLine("You found your way.");
                        this.isAlive = false;
                        endBattle = true;
                        GameVars.Instance.UNKNOWN_BEATEN = true;
                    }
                    else
                    {
                        turnDescription.AppendLine("You couldn't figure it out.");
                    }
                }
                else if (selectedNode.NodeTitle.Equals(ItemIDs.FIRST_AID_KIT.ToString()))
                {
                    turnDescription.AppendLine("You used the First Aid Kit... You regain 50 HP!");
                    GameVars.Instance.Player.Energy += 50;
                }
                else
                {
                    turnDescription.AppendLine("You used the " + selectedNode.NodeText + "... It's not very effective!");
                }
                break;
        }

        if (ranAway)
        {
            turnDescription.AppendLine("You ran away?");
            GameVars.Instance.Player.FullHeal();
        }
        else
        {
            if (this.isAlive)
            {
                turnDescription.Append(this.TakeTurn());
                endBattle = true;
                int fleeReward = (int) (this.XP / 100);
                turnDescription.AppendLine(" You earned " + fleeReward + " XP.");
                GameVars.Instance.Player.WildernessPoints += fleeReward;
                GameVars.Instance.Player.FullHeal();
            }
            
            //need a separate check because the monster might die on its turn due to counterattack
            if (!this.isAlive)
            {
                endBattle = true;
                turnDescription.AppendLine("Well that was weird. You earned " + this.XP + " XP, I guess.");
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
        //StringBuilder turnResult = new StringBuilder(DoMonsterAttack());

        

        return shortDescription + " vanishes.";
    }

    protected virtual string DoPlayerAttack()
    {
        Mob attacker = GameVars.Instance.Player;

        StringBuilder attackResult = new StringBuilder("You attack... ?");

        if (GameVars.Instance.RollToHit(attacker.HitChance, this.EvadeChance))
        {
            int damageDone = 0;

            if (GameVars.Instance.RollCritChance(attacker.CritChance, 255))
            {
                damageDone = GameVars.Instance.RollAttackDamage(attacker.AttackPower, attacker.AttackMultiplier, 0);
                attackResult.AppendLine("Critical Hit??");
            }
            else
            {
                damageDone = GameVars.Instance.RollAttackDamage(attacker.AttackPower, attacker.AttackMultiplier, this.Defense);
            }

            if (damageDone > 0)
            {
                this.HP -= damageDone;
            }

            attackResult.AppendLine(this.shortDescription + " takes " + damageDone + " damage?");
        }
        else
        {
            attackResult.AppendLine();
            attackResult.AppendLine("Missed?");
        }

        return attackResult.ToString();
    }

    protected virtual string DoMonsterAttack()
    {
        Mob defender = GameVars.Instance.Player;

        StringBuilder attackResult = new StringBuilder(this.shortDescription + " does something... ");

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
        GameVars.Instance.UNKNOWN_SEEN = true;
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, this.encounterDescription);

        rootMenu.DisplayImageAsset = this.imageAsset;
        
        battleTree = new TreeNode<MenuNode>(rootMenu);

        battleTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_ATTACK, "<Aggression>", "")));
        battleTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.INVENTORY, CHOICE1_ITEM, "<Tactics>", "")));
        battleTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_FLEE, "<Cowardice>", "")));

        this.currentScene = new FSelectionDisplayScene(this.Name, battleTree);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
