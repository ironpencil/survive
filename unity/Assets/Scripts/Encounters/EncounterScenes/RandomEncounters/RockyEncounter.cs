using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class EsmudohrBattleEncounter : FEncounterScene
{

    private const string CHOICE_ATTACK = "Attack";
    private const string CHOICE_ATTACK_HOST = "AttackHost";
    private const string CHOICE_ATTACK_TENTACLES = "AttackTentacles";
    private const string CHOICE_ITEM = "Item";

    private const string CHOICE_DENIAL = "Disbelieve";
    private const string CHOICE_ANGER = "Rage";
    private const string CHOICE_BARGAINING = "Plead";
    private const string CHOICE_DEPRESSION = "Despair";
    private const string CHOICE_ACCEPTANCE = "Endure";

    int phase3Index = 0;

    List<string> phase3Choices = new List<string>() { CHOICE_DENIAL, CHOICE_ANGER, CHOICE_BARGAINING, CHOICE_DEPRESSION, CHOICE_ACCEPTANCE };

    TreeNode<MenuNode> battleTree;
    TreeNode<MenuNode> stunnedTree;

    public string stage0Description = "Rocky Raccoon";
    public string stage1Description = "Esmudohr Quiescent";
    public string stage2Description = "Esmudohr Resplendent";
    public string stage3Description = "Esmudohr Omnipresent";

    public string stage0Image = "rocky";
    public string stage1Image = "evil_rocky";
    public string stage2Image = "esmudohr";
    public string stage3Image = "eternal";

    public string shortDescription;
    public string encounterDescription;
    public string imageAsset;

    private string winText = "Esmudohr Omnipresent is destroyed!";

    public bool esmudohrDefeated = false;

    private enum Phase
	{
        PHASE0,
        PHASE1,
        PHASE2,
        PHASE3
	}

    private Phase currentPhase = Phase.PHASE1;

    private int turnCounter = 0;

    public EsmudohrBattleEncounter(string _name = "ESMUDOHR")
        : base(_name)
    {
        this.shortDescription = stage0Description;
        this.encounterDescription = "";
        this.imageAsset = stage1Image;

        hp = stage1HP;
    }

    private int stage1HP = 1000;
    private int stage2HP = 10000;
    private int stage3HP = 1000000;
    private int TentacleHP = 60;

    private int hp;

    private int HP
    {
        get { return hp; }
        set
        {
            hp = value;
            this.isAlive = (hp > 0);
        }
    }

    private int MaxTentaclesAtOnce = 10;
    private int MaxTentaclesSpawned = 50;
    private int TotalTentaclesSpawned = 5;
    private int TentacleCount = 5;
    private int TentacleDefense = 50;
    private int TentacleHitChance = 50;
    private int TentacleAttackPower = 5;
    private int TentacleAttackMultiplier = 2;
    private int TentacleCritChance = 0;

    private int Defense;

    private int HitChance;
    private int EvadeChance;

    private int AttackPower;
    private int AttackMultiplier;
    private int CritChance;

    private int XP = 5000;

    private bool isAlive = true;

    private bool playerIsStunned = false;
    private int playerStunnedTimer = 0;

    protected override void HandleResult()
    {
        switch (this.currentPhase)
        {
            case Phase.PHASE0: MoveToPhase(Phase.PHASE1);
                this.currentScene = new FSelectionDisplayScene(this.Name, battleTree);
                FSceneManager.Instance.PushScene(this.currentScene);
                break;
            case Phase.PHASE1: HandlePhase1Result();
                break;
            case Phase.PHASE2: HandlePhase2Result();
                break;
            case Phase.PHASE3: HandlePhase3Result();
                break;
            default:
                break;
        }
    }

    private void MoveToPhase(Phase phase)
    {
        this.currentPhase = phase;

        MenuNode rootMenu;

        switch (phase)
        {
            case Phase.PHASE0:
                this.imageAsset = stage0Image;
                this.shortDescription = stage0Description;
                this.encounterDescription = "As you step into the rune, dark red mist begins to coalesce around you. All of a sudden, Rocky Raccoon, your friendly North Texarado State Park mascot, rushes in and pulls you away from it.\n\n\n" +
                    "\"Oh no, what have you done? You shouldn't be here, little one, this is a very dangerous place!\"\n\n\nThe mist continues to gather, and in a flash it flies towards Rocky, who appears to absorb it. " +
                    "He is quiet for a moment, then he slowly turns towards you.\n\nBut he is changed.";

                this.turnCounter = 0;

                this.HP = 1000;
                this.Defense = 1000;

                this.HitChance = 100;
                this.EvadeChance = 0;

                this.AttackPower = 20;
                this.AttackMultiplier = 4;
                this.CritChance = 5;

                rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, this.encounterDescription);
                rootMenu.DisplayImageAsset = this.imageAsset;

                battleTree = new TreeNode<MenuNode>(rootMenu);

                break;
            case Phase.PHASE1:
                this.imageAsset = stage1Image;
                this.shortDescription = stage1Description;
                this.encounterDescription = "\"Thank you, little one, for releasing me from that dreary prison. I am Esmudohr the All-Consuming. To show my gratitude for freeing me, I will devour you first - before moving on to the rest of your world.\"\n\n" +
                    stage1Description + " stands before you.";

                this.turnCounter = 0;

                this.HP = 1000;
                this.Defense = 1000;

                this.HitChance = 100;
                this.EvadeChance = 0;

                this.AttackPower = 20;
                this.AttackMultiplier = 4;
                this.CritChance = 5;

                rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, this.encounterDescription);
                rootMenu.DisplayImageAsset = this.imageAsset;

                battleTree = new TreeNode<MenuNode>(rootMenu);
                battleTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE_ATTACK, "Attack", "")));
                battleTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.INVENTORY, CHOICE_ITEM, "Item", "")));

                MenuNode stunnedMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, "");
                stunnedMenu.DisplayImageAsset = this.imageAsset;
                stunnedTree = new TreeNode<MenuNode>(stunnedMenu);

                break;
            case Phase.PHASE2:
                this.imageAsset = stage2Image;
                this.shortDescription = stage2Description;
                this.encounterDescription = stage2Description + " writhes before you.";

                this.turnCounter = 0;

                this.HP = stage2HP;
                this.Defense = 60;

                this.HitChance = 0;
                this.EvadeChance = 0;

                this.AttackPower = 0;
                this.AttackMultiplier = 0;
                this.CritChance = 0;

                rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, this.encounterDescription);
                rootMenu.DisplayImageAsset = this.imageAsset;

                battleTree = new TreeNode<MenuNode>(rootMenu);
                battleTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE_ATTACK_HOST, "Attack " + this.shortDescription, "")));
                battleTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE_ATTACK_TENTACLES, "Attack Tentacles", "")));
                battleTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.INVENTORY, CHOICE_ITEM, "Item", "")));

                stunnedTree.Value.DisplayImageAsset = this.imageAsset;
                break;
            case Phase.PHASE3:
                this.imageAsset = stage3Image;
                this.shortDescription = stage3Description;
                this.encounterDescription = stage3Description + " bursts forth from " + stage2Description + " and engulfs you.";

                playerIsStunned = true;
                playerStunnedTimer = 3;

                this.turnCounter = 0;

                this.HP = stage3HP;
                this.Defense = 1000;

                this.HitChance = 100;
                this.EvadeChance = 100;

                this.AttackPower = 1000;
                this.AttackMultiplier = 10;
                this.CritChance = 0;

                rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, this.encounterDescription);
                rootMenu.DisplayImageAsset = this.imageAsset;

                battleTree = new TreeNode<MenuNode>(rootMenu);

                battleTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE_DENIAL, CHOICE_DENIAL, "")));

                stunnedTree.Value.DisplayImageAsset = this.imageAsset;
                break;
            default:
                break;
        }
    }

    #region Phase1
    private void HandlePhase1Result() {

        MenuNode selectedNode = FSelectionDisplayScene.GetLastResultNode(this.resultNode).Value;

        StringBuilder turnDescription = new StringBuilder();

        bool endBattle = false;

        if (this.isAlive)
        {
            turnDescription.Append(this.TakeTurnPhase1());
        }

        if (!playerIsStunned && GameVars.Instance.Player.Energy > 0)
        {
            switch (selectedNode.NodeTitle)
            {
                case CHOICE_ATTACK: //result of choice a
                    turnDescription.Append(DoPlayerAttackPhase1());
                    break;
                default: // item usage
                    if (selectedNode.NodeTitle.Equals(ItemIDs.DAWN_CRYSTAL.ToString()))
                    {
                        turnDescription.AppendLine("You used the Dawn Crystal... ");
                        turnDescription.AppendLine(this.shortDescription + "'s true nature is revealed!");
                        //move to phase 2
                        this.isAlive = false;
                    }
                    else if (selectedNode.NodeTitle.Equals(ItemIDs.FIRST_AID_KIT.ToString()))
                    {
                        turnDescription.AppendLine("You used the First Aid Kit... You regain 100 HP!");
                        GameVars.Instance.Player.Energy += 100;
                    }
                    else
                    {
                        turnDescription.AppendLine("You used the " + selectedNode.NodeText + "... nothing happens.");
                    }
                    break;
            }
        }
                        
            //need a separate check because the monster might die on its turn due to counterattack
            if (!this.isAlive)
            {
                //move to next phase
                this.MoveToPhase(Phase.PHASE2);
                turnDescription.AppendLine(this.encounterDescription);

                //turnDescription.AppendLine(this.shortDescription + " was uprooted! You earned " + this.XP + " XP!");
                //GameVars.Instance.Player.WildernessPoints += XP;
                //GameVars.Instance.Player.FullHeal();
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

        turnCounter++;
        FSceneManager.Instance.PushScene(this.currentScene);
    }    

    protected virtual string TakeTurnPhase1()
    {
        Mob defender = GameVars.Instance.Player;
        StringBuilder turnResult = new StringBuilder();

        int actionChoice;

        if (turnCounter == 0)
        {
            //don't attack on the first turn
            actionChoice = UnityEngine.Random.Range(1, 4);
        }
        else
        {
            actionChoice = UnityEngine.Random.Range(0, 4);
        }

        switch (actionChoice)
        {
            case 0: turnResult.Append(DoMonsterAttackPhase1());
                break;
            case 1:
            case 2: turnResult.AppendLine(this.shortDescription + " smiles pleasantly.");
                break;
            case 3:turnResult.AppendLine(this.shortDescription + " flares with brilliant, hot, red light.");
                break;
            default:
                break;
        }


        //if (!playerIsStunned)
        //{
        //    //stun player
        //    turnResult.AppendLine(this.shortDescription + " attempts to mesmerize you with its hypnotic eye... ");
        //    if (GameVars.Instance.RollToHit(90, defender.Level))
        //    {
        //        //playerStunnedTimer = UnityEngine.Random.Range(1, 3);
        //        //playerIsStunned = true;
        //        turnResult.AppendLine("You can't move!");
        //    }
        //    else
        //    {
        //        turnResult.AppendLine("You resisted!");
        //    }
        //}
        //else
        //{
        //    //attack player
        //    turnResult.Append(DoMonsterAttack());
        //    playerStunnedTimer--;
        //}


        return turnResult.ToString();
    }

    protected virtual string DoPlayerAttackPhase1()
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

            //takes no damage in phase 1
            damageDone = 0;

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

    protected virtual string DoMonsterAttackPhase1()
    {
        Mob defender = GameVars.Instance.Player;

        StringBuilder attackResult = new StringBuilder();

        attackResult.Append(this.shortDescription + " assails your mind... ");

        int playerEvadeChance = defender.EvadeChance;
        if (playerIsStunned) { playerEvadeChance = 0; }

        if (GameVars.Instance.RollToHit(this.HitChance, (playerEvadeChance / 4)))
        {
            int damageDone = 0;

            damageDone = GameVars.Instance.RollAttackDamage(this.AttackPower, this.AttackMultiplier, 0);

            if (damageDone > 0)
            {
                defender.Energy -= damageDone;
            }

            attackResult.AppendLine("You take " + damageDone + " damage!");
        }
        else
        {
            attackResult.AppendLine("You resisted!");
        }

        return attackResult.ToString();
    }

    #endregion


    #region Phase2
    private void HandlePhase2Result()
    {

        MenuNode selectedNode = FSelectionDisplayScene.GetLastResultNode(this.resultNode).Value;

        StringBuilder turnDescription = new StringBuilder();

        bool endBattle = false;

        if (this.isAlive)
        {
            turnDescription.Append(this.TakeTurnPhase2());
        }

        if (!playerIsStunned && GameVars.Instance.Player.Energy > 0)
        {
            switch (selectedNode.NodeTitle)
            {
                case CHOICE_ATTACK_HOST: //result of choice a
                    turnDescription.Append(DoPlayerAttackHost());
                    break;
                case CHOICE_ATTACK_TENTACLES:
                    turnDescription.Append(DoPlayerAttackTentacles());
                    break;
                default: // item usage
                    if (selectedNode.NodeTitle.Equals(ItemIDs.DAWN_CRYSTAL.ToString()))
                    {
                        turnDescription.AppendLine("You used the Dawn Crystal... its power is gone, now.");
                    }
                    else if (selectedNode.NodeTitle.Equals(ItemIDs.FIRST_AID_KIT.ToString()))
                    {
                        turnDescription.AppendLine("You used the First Aid Kit... You regain 100 HP!");
                        GameVars.Instance.Player.Energy += 100;
                    }
                    else
                    {
                        turnDescription.AppendLine("You used the " + selectedNode.NodeText + "... nothing happens.");
                    }
                    break;
            }
        }

        if (TentacleCount == 0 && TotalTentaclesSpawned == MaxTentaclesSpawned)
        {
            //ran out of tentacles, move to next phase
            this.isAlive = false;
        }

        if (this.isAlive)
        {
            turnDescription.Append(this.TentacleTurnPhase2());
        }

        //need a separate check because the monster might die on its turn due to counterattack
        if (!this.isAlive)
        {
            //move to next phase
            this.MoveToPhase(Phase.PHASE3);
            turnDescription.AppendLine(this.encounterDescription);

            //turnDescription.AppendLine(this.shortDescription + " was uprooted! You earned " + this.XP + " XP!");
            //GameVars.Instance.Player.WildernessPoints += XP;
            //GameVars.Instance.Player.FullHeal();
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

        turnCounter++;
        FSceneManager.Instance.PushScene(this.currentScene);
    }

    protected virtual string TakeTurnPhase2()
    {
        Mob defender = GameVars.Instance.Player;
        StringBuilder turnResult = new StringBuilder();

        int tentaclesSpawned = UnityEngine.Random.Range(4, 9);

        if (this.TotalTentaclesSpawned + tentaclesSpawned > this.MaxTentaclesSpawned)
        {
            tentaclesSpawned = this.MaxTentaclesSpawned - this.TotalTentaclesSpawned;
        }

        if (tentaclesSpawned + this.TentacleCount > this.MaxTentaclesAtOnce)
        {
            tentaclesSpawned = this.MaxTentaclesAtOnce - this.TentacleCount;
        }

        this.TotalTentaclesSpawned += tentaclesSpawned;
        this.TentacleCount += tentaclesSpawned;

        turnResult.AppendLine(this.shortDescription + "'s unearthly wails echo loudly inside your head.");

        switch (tentaclesSpawned)
        {
            case 0:
                break;
            case 1: turnResult.AppendLine("A tentacle erupts from " + this.shortDescription + "!");
                break;
            default: turnResult.AppendLine(tentaclesSpawned + " tentacles erupt from " + this.shortDescription + "!");
                break;
        }
        
        //if (!playerIsStunned)
        //{
        //    //stun player
        //    turnResult.AppendLine(this.shortDescription + " attempts to mesmerize you with its hypnotic eye... ");
        //    if (GameVars.Instance.RollToHit(90, defender.Level))
        //    {
        //        //playerStunnedTimer = UnityEngine.Random.Range(1, 3);
        //        //playerIsStunned = true;
        //        turnResult.AppendLine("You can't move!");
        //    }
        //    else
        //    {
        //        turnResult.AppendLine("You resisted!");
        //    }
        //}
        //else
        //{
        //    //attack player
        //    turnResult.Append(DoMonsterAttack());
        //    playerStunnedTimer--;
        //}


        return turnResult.ToString();
    }

    int tentacleHitCount = 0;
    protected virtual string TentacleTurnPhase2()
    {
        Mob defender = GameVars.Instance.Player;
        StringBuilder turnResult = new StringBuilder();

        tentacleHitCount = 0;

        for (int i = 0; i < TentacleCount; i++)
        {
            turnResult.Append(DoTentacleAttackPhase2());
            if (tentacleHitCount >= 5)
            {
                break;
            }
        }

        return turnResult.ToString();
    }

    protected virtual string DoPlayerAttackHost()
    {
        Mob attacker = GameVars.Instance.Player;

        StringBuilder attackResult = new StringBuilder("You attack " + this.shortDescription + "... ");

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

    protected virtual string DoPlayerAttackTentacles()
    {
        Mob attacker = GameVars.Instance.Player;

        StringBuilder attackResult = new StringBuilder("");

        if (TentacleCount <= 0)
        {
            attackResult.AppendLine("There are no tentacles to attack!");
            return attackResult.ToString();
        }

        if (TentacleCount == 1)
        {
            attackResult.Append("You attack the tentacle... ");
        }
        else
        {
            attackResult.Append("You attack the tentacles... ");
        }
        int damageDone = 0;

        bool criticalHit = false;
        if (GameVars.Instance.RollCritChance(attacker.CritChance, 0))
        {
            damageDone = GameVars.Instance.RollAttackDamage(attacker.AttackPower, attacker.AttackMultiplier, 0);
            criticalHit = true; //attackResult.AppendLine("Critical Hit!!");
        }
        else
        {
            damageDone = GameVars.Instance.RollAttackDamage(attacker.AttackPower, attacker.AttackMultiplier, TentacleDefense);
        }

        int tentaclesDestroyed = 0;

        if (damageDone > 0)
        {
            tentaclesDestroyed = damageDone / TentacleHP;
        }

        if (tentaclesDestroyed > TentacleCount)
        {
            tentaclesDestroyed = TentacleCount;
        }

        if (tentaclesDestroyed > 0)
        {
            string hitResponse = this.shortDescription + " thrashes about in pain!";
            if (criticalHit)
            {
                attackResult.AppendLine("Critical Hit!!");
            }
            if (tentaclesDestroyed == 1)
            {
                attackResult.AppendLine("You severed 1 tentacle! " + hitResponse);
            }
            else
            {
                attackResult.AppendLine("You severed " + tentaclesDestroyed + " tentacles! " + hitResponse);
            }
        }
        else
        {
            attackResult.AppendLine("You were unable to sever any tentacles!");
        }

        TentacleCount -= tentaclesDestroyed;

        if (TentacleCount > 0) {

            if (TentacleCount == 1) {
                attackResult.AppendLine("1 tentacle remains.");
            } else {
                attackResult.AppendLine(TentacleCount + " tentacles remain.");
            }
        }
        return attackResult.ToString();
    }

    protected virtual string DoTentacleAttackPhase2()
    {
        Mob defender = GameVars.Instance.Player;

        StringBuilder attackResult = new StringBuilder("");       

        int playerEvadeChance = defender.EvadeChance;
        if (playerIsStunned) { playerEvadeChance = 0; }

        if (GameVars.Instance.RollToHit(TentacleHitChance, playerEvadeChance/2))
        {
            int damageDone = 0;
            bool criticalHit = false;

            //if (GameVars.Instance.RollCritChance(TentacleCritChance, 0))
            //{
            //    criticalHit = true;
                damageDone = GameVars.Instance.RollAttackDamage(TentacleAttackPower, TentacleAttackMultiplier, 0);
            //}
            //else
            //{
            //    damageDone = GameVars.Instance.RollAttackDamage(TentacleAttackPower, TentacleAttackMultiplier, defender.Defense);
            //}

            if (damageDone > 0)
            {
                defender.Energy -= damageDone;
                attackResult.Append("A tentacle rends you... ");
                if (criticalHit) {
                    attackResult.AppendLine("Critical hit!!");   
                }
                
                attackResult.AppendLine("You take " + damageDone + " damage!");
                tentacleHitCount++;
            }            
        }

        return attackResult.ToString();
    }

    #endregion

    #region Phase3

    private bool playerFirstTurnPhase3 = true;

    private void HandlePhase3Result()
    {

        MenuNode selectedNode = FSelectionDisplayScene.GetLastResultNode(this.resultNode).Value;

        StringBuilder turnDescription = new StringBuilder();

        bool endBattle = false;

        turnDescription.Append(this.TakeTurnPhase3());

        if (playerStunnedTimer <= 0)
        {
            if (playerIsStunned)
            {
                playerIsStunned = false;
            }
        }

        if (!playerIsStunned)
        {
            if (playerFirstTurnPhase3)
            {
                phase3Index = 0;
                playerFirstTurnPhase3 = false;
            }
            else
            {
                phase3Index++;
            }
        }

        //if (phase3Index > 4)
        //{
        //    endBattle = true;
        //}

        if (!this.isAlive)
        {
            endBattle = true;
        }

        if (endBattle)
        {
            turnDescription.AppendLine(this.winText);
            turnDescription.AppendLine("You gain " + XP + " XP.");            
            GameVars.Instance.Player.WildernessPoints += XP;
            GameVars.Instance.Player.FullHeal();

            MenuNode endBattleNode = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, turnDescription.ToString());
            endBattleNode.DisplayImageAsset = battleTree.Value.DisplayImageAsset;
            battleTree = new TreeNode<MenuNode>(endBattleNode);
            playerIsStunned = false;
            this.esmudohrDefeated = true;
            this.ShouldPop = true;
        }
        else
        {
            if (playerIsStunned)
            {
                stunnedTree.Value.DisplayMessage = turnDescription.ToString();
                //this.currentScene = new FSelectionDisplayScene(this.Name, stunnedTree);
            }
            else
            {
                MenuNode battleNode = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, turnDescription.ToString());
                battleNode.DisplayImageAsset = battleTree.Value.DisplayImageAsset;
                battleTree = new TreeNode<MenuNode>(battleNode);
                battleTree.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, phase3Choices[phase3Index], phase3Choices[phase3Index], "")));
                //this.currentScene = new FSelectionDisplayScene(this.Name, battleTree);
            }
        }

        if (playerIsStunned)
        {
            this.currentScene = new FSelectionDisplayScene(this.Name, stunnedTree);
        }
        else
        {
            this.currentScene = new FSelectionDisplayScene(this.Name, battleTree);
        }

        turnCounter++;
        FSceneManager.Instance.PushScene(this.currentScene);
    }

    protected virtual string TakeTurnPhase3()
    {
        StringBuilder turnResult = new StringBuilder();

        Mob defender = GameVars.Instance.Player;

        if (!playerIsStunned)
        {
            if (phase3Index >= 4)
            {
                playerStunnedTimer = 100;
            }
            else
            {
                turnResult.AppendLine("It does nothing.");
                //stun player for a random number of turns
                playerStunnedTimer = UnityEngine.Random.Range(1, 4);
            }
            playerIsStunned = true;
        }
        else
        {
            playerStunnedTimer--;
        }

        turnResult.AppendLine(this.shortDescription + " consumes...");

        if (phase3Index >= 4)
        {
            this.AttackPower *= 2;
            turnResult.Append(DoMonsterAttackSelfPhase3());            
        }
        else
        {
            turnResult.Append(DoMonsterAttackPhase3());
        }

        return turnResult.ToString();
    }

    protected virtual string DoMonsterAttackPhase3()
    {
        Mob defender = GameVars.Instance.Player;

        StringBuilder attackResult = new StringBuilder();

            int damageDone = 0;

            damageDone = GameVars.Instance.RollAttackDamage(this.AttackPower, this.AttackMultiplier, 0);

            if (damageDone > 0)
            {
                defender.Energy -= damageDone;
            }

            attackResult.AppendLine("You take " + damageDone + " damage!");

        return attackResult.ToString();
    }

    protected virtual string DoMonsterAttackSelfPhase3()
    {
        StringBuilder attackResult = new StringBuilder();

        int damageDone = 0;

        damageDone = GameVars.Instance.RollAttackDamage(this.AttackPower, this.AttackMultiplier, 0);

        if (damageDone > 0)
        {
            this.HP -= damageDone;
        }

        attackResult.AppendLine(this.shortDescription + " takes " + damageDone + " damage!");

        return attackResult.ToString();
    }


    #endregion



    protected override void HandleCancel()
    {
        //cancel is fired when we stun the player because they can't make any choices
        HandleResult();
    }

    public override void OnEnter()
    {
        MoveToPhase(Phase.PHASE0);

        this.currentScene = new FSelectionDisplayScene(this.Name, battleTree);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

}
