using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class OptorchidEncounter : FEncounterScene
{

    private const string CHOICE1_ATTACK = "Choice1_Attack";
    private const string CHOICE1_ITEM = "Choice1_Item";
    private const string CHOICE1_FLEE = "Choice1_Flee";

    TreeNode<MenuNode> battleTree;
    TreeNode<MenuNode> stunnedTree;

    public string shortDescription;
    public string encounterDescription;
    public string imageAsset;

    public OptorchidEncounter(string _name = "OPTORCHID")
        : base(_name)
    {
        this.shortDescription = "Optorchid";
        this.encounterDescription = "A bizarre " + this.shortDescription + " blocks your path!";
        this.imageAsset = "optorchid";
    }

    private int hp = 100;

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

    private int Defense = 15;

    private int HitChance = 100;
    private int EvadeChance = 5;

    private int AttackPower = 17;
    private int AttackMultiplier = 2;
    private int CritChance = 5;

    private int XP = 30;

    private bool isAlive = true;

    private bool isBlind = false;

    private bool playerIsStunned = false;
    private int playerStunnedTimer = 0;

    protected override void HandleResult()
    {
        MenuNode selectedNode = FSelectionDisplayScene.GetLastResultNode(this.resultNode).Value;

        StringBuilder turnDescription = new StringBuilder();

        bool endBattle = false;
        bool ranAway = false;

        if (!playerIsStunned)
        {
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
                    if (selectedNode.NodeTitle.Equals(ItemIDs.SALT.ToString()))
                    {
                        if (!isBlind)
                        {
                            turnDescription.AppendLine("You sprinkled some salt around... The " + shortDescription + " was blinded!");
                            this.isBlind = true;
                            this.AttackMultiplier = 3;
                            this.HitChance = 50;
                        }
                        else
                        {
                            turnDescription.AppendLine("The " + shortDescription + " is already blinded!");
                        }
                    }
                    else if (selectedNode.NodeTitle.Equals(ItemIDs.FIRST_AID_KIT.ToString()))
                    {
                        turnDescription.AppendLine("You used the First Aid Kit... You regain 50 HP!");
                        GameVars.Instance.Player.Energy += 50;
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
                turnDescription.AppendLine(this.shortDescription + " was uprooted! You earned " + this.XP + " XP!");
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
            playerIsStunned = false;

            this.ShouldPop = true;
        }
        else
        {
            if (playerStunnedTimer <= 0)
            {
                if (playerIsStunned)
                {
                    playerIsStunned = false;
                    turnDescription.AppendLine("You are able to move again!");
                }
            }            
        }

        if (playerIsStunned)
        {
            stunnedTree.Value.DisplayMessage = turnDescription.ToString();
            this.currentScene = new FSelectionDisplayScene(this.Name, stunnedTree);
        }
        else
        {
            battleTree.Value.DisplayMessage = turnDescription.ToString();
            this.currentScene = new FSelectionDisplayScene(this.Name, battleTree);
        }
        
        FSceneManager.Instance.PushScene(this.currentScene);
    }

    protected virtual string TakeTurn()
    {
        Mob defender = GameVars.Instance.Player;
        StringBuilder turnResult = new StringBuilder();


        if (!isBlind && !playerIsStunned)
        {
            //stun player
            turnResult.AppendLine(this.shortDescription + " attempts to mesmerize you with its hypnotic eye... ");
            if (GameVars.Instance.RollToHit(90, defender.Level))
            {
                playerStunnedTimer = UnityEngine.Random.Range(1, 3);
                playerIsStunned = true;
                turnResult.AppendLine("You can't move!");
            }
            else
            {
                turnResult.AppendLine("You resisted!");
            }
        }
        else
        {
            //attack player
            turnResult.Append(DoMonsterAttack());
            playerStunnedTimer--;
        }
        

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

        StringBuilder attackResult = new StringBuilder();

        if (this.isBlind)
        {
            attackResult.Append(this.shortDescription + " thrashed its leaves wildly... ");
        }
        else
        {
            attackResult.Append(this.shortDescription + " used a cutting leaf attack... ");
        }

        int playerEvadeChance = defender.EvadeChance;
        if (playerIsStunned) { playerEvadeChance = 0; }

        if (GameVars.Instance.RollToHit(this.HitChance, playerEvadeChance))
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
        //cancel is fired when we stun the player because they can't make any choices
        HandleResult();
    }

    public override void OnEnter()
    {
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, this.encounterDescription);

        rootMenu.DisplayImageAsset = this.imageAsset;
        
        battleTree = new TreeNode<MenuNode>(rootMenu);

        battleTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_ATTACK, "Attack", "")));
        battleTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.INVENTORY, CHOICE1_ITEM, "Item", "")));
        battleTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_FLEE, "Flee", "")));

        MenuNode stunnedMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "");

        stunnedMenu.DisplayImageAsset = this.imageAsset;

        stunnedTree = new TreeNode<MenuNode>(stunnedMenu);

        this.currentScene = new FSelectionDisplayScene(this.Name, battleTree);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
