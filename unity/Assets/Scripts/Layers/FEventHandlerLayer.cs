using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class FEventHandlerLayer : FLayer
{
    protected Vector2 tileLocation;

    protected IPTile eventTile;
    protected IPTiledObject tiledObject;

    protected FEncounterScene encounter;

    private bool ShouldPop = false;

    public string Name { get; private set; }

    public FEventHandlerLayer(FScene parent, string eventName) : base(parent)
	{
        this.Name = eventName;
        this.tileLocation = Vector2.zero;
        this.eventTile = null;
        this.tiledObject = null;
	}

    public FEventHandlerLayer(FScene parent, string eventName, IPTile eventTile, Vector2 tileLocation) : base(parent)
    {
        this.Name = eventName;
        this.tileLocation = tileLocation;
        this.eventTile = eventTile;
        this.tiledObject = null;
    }

    public FEventHandlerLayer(FScene parent, string eventName, IPTiledObject tiledObject)
        : base(parent)
    {
        this.Name = eventName;
        this.tileLocation = Vector2.zero;
        this.eventTile = null;
        this.tiledObject = tiledObject;
    }
	
	public override void OnUpdate ()
	{
        //if (this.Parent.Paused) { return; }

        if (encounter != null && encounter.IsFinished)
        {
            ShouldPop = true;
        }

        if (ShouldPop)
        {
            this.RemoveFromContainer();
        }
	}

    public override void OnEnter()
	{
        //decide which event to enter
        switch (Name)
        {
            case "GAME_MENU": GameMenuEnter();
                break;
            case "FOUND_MUSHROOMS": FoundMushroomEnter();
                break;
            case "FOUND_MUSIC": FoundMusicEnter();
                break;
            case "PORNO": FoundPornEnter();
                break;
            case "GET_WATER": GetWaterEnter();
                break;
            case "BEAR": BearEncounterEnter();
                break;
            case "WOLF": WolfEncounterEnter();
                break;
            case "SNAKE": SnakeEncounterEnter();
                break;
            case "COUGAR": CougarEncounterEnter();
                break;
            case "FIND_FOOD_QUIZ": FindFoodQuizEncounterEnter();
                break;
            case "BATHROOM_QUIZ": BathroomQuizEncounterEnter();
                break;
            case "FISHING_QUIZ": FishingQuizEncounterEnter();
                break;
            case "QUICKSAND_QUIZ": QuicksandQuizEncounterEnter();
                break;
            case "MAKE_FIRE_QUIZ": MakeFireQuizEncounterEnter();
                break;
            case "MAKE_CAMP_QUIZ": MakeCampQuizEncounterEnter();
                break;
            case "MUDSLIDE_QUIZ": MudslideQuizEncounterEnter();
                break;
            case "BEAR_TRAP_QUIZ": BearTrapQuizEncounterEnter();
                break;
            case "NAVIGATION_QUIZ": NavigationQuizEncounterEnter();
                break;
            case "LIGHTNING_QUIZ": LightningQuizEncounterEnter();
                break;
            case "TICK": TickEncounterEnter();
                break;
            case "BEES": BeesEncounterEnter();
                break;
            case "RACCOON": RaccoonEncounterEnter();
                break;
            case "ANTS": AntsEncounterEnter();
                break;
            case "SPIDER": SpiderEncounterEnter();
                break;
            case "WILD_PLANT": WildPlantEncounterEnter();
                break;
            case "FERAL_ANUM": FeralAnumEncounterEnter();
                break;
            case "BUTTER_BUG": ButterBugEncounterEnter();
                break;
            case "OPTORCHID": OptorchidEncounterEnter();
                break;
            case "UNKNOWN": UnknownEncounterEnter();
                break;
            case "SECRET_WORLD": SecretWorldEncounterEnter();
                break;
            case "DAWN_CRYSTAL": DawnCrystalEncounterEnter();
                break;
            case "FINAL_BATTLE": EsmudohrEncounterEnter();
                break;
            default:
                break;
        }
	}   

    public override void OnExit()
	{
        //decide which event to exit
        switch (Name)
        {
            case "GAME_MENU": GameMenuExit();
                break;
            case "FOUND_MUSHROOMS": FoundMushroomExit();
                break;
            case "FOUND_MUSIC": FoundMusicExit();
                break;
            case "PORNO": FoundPornExit();
                break;
            case "GET_WATER": GetWaterExit();
                break;
            case "BEAR": BearEncounterExit();
                break;
            case "WOLF": WolfEncounterExit();
                break;
            case "SNAKE": SnakeEncounterExit();
                break;
            case "COUGAR": CougarEncounterExit();
                break;
            case "FIND_FOOD_QUIZ": FindFoodQuizEncounterExit();
                break;
            case "BATHROOM_QUIZ": BathroomQuizEncounterExit();
                break;
            case "FISHING_QUIZ": FishingQuizEncounterExit();
                break;
            case "QUICKSAND_QUIZ": QuicksandQuizEncounterExit();
                break;
            case "MAKE_FIRE_QUIZ": MakeFireQuizEncounterExit();
                break;
            case "MAKE_CAMP_QUIZ": MakeCampQuizEncounterExit();
                break;
            case "MUDSLIDE_QUIZ": MudslideQuizEncounterExit();
                break;
            case "BEAR_TRAP_QUIZ": BearTrapQuizEncounterExit();
                break;
            case "NAVIGATION_QUIZ": NavigationQuizEncounterExit();
                break;
            case "LIGHTNING_QUIZ": LightningQuizEncounterExit();
                break;
            case "TICK": TickEncounterExit();
                break;
            case "BEES": BeesEncounterExit();
                break;
            case "RACCOON": RaccoonEncounterExit();
                break;
            case "ANTS": AntsEncounterExit();
                break;
            case "SPIDER": SpiderEncounterExit();
                break;
            case "WILD_PLANT": WildPlantEncounterExit();
                break;
            case "FERAL_ANUM": FeralAnumEncounterExit();
                break;
            case "BUTTER_BUG": ButterBugEncounterExit();
                break;
            case "OPTORCHID": OptorchidEncounterExit();
                break;
            case "UNKNOWN": UnknownEncounterExit();
                break;
            case "SECRET_WORLD": SecretWorldEncounterExit();
                break;
            case "DAWN_CRYSTAL": DawnCrystalEncounterExit();
                break;
            case "FINAL_BATTLE": EsmudohrEncounterExit();
                break;
            default:
                break;
        }
	}

    private void GameMenuEnter()
    {
        if (!GameVars.Instance.GameMenuDisplayed)
        {
            encounter = new GameMenuEncounter();
            FSceneManager.Instance.PushScene(encounter);
        }
        else
        {
            ShouldPop = true;
        }
    }
    private void GameMenuExit() { }

    private void FoundMushroomEnter()
    {
        //if we're in the secret world, we don't care about mushrooms
        if (GameVars.Instance.GetParamValueBool(GameVarParams.SECRET_WORLD.ToString()))
        {
            ShouldPop = true;
            return;
        }

        bool mushroomsEaten = GameVars.Instance.TileHelper.GetTilePropertyValueBool(tileLocation, "MUSHROOMS_EATEN");

        if (!mushroomsEaten)
        {
            encounter = new MushroomsEncounter("FOUND_MUSHROOMS");

            FSceneManager.Instance.PushScene(encounter);
        }
        else
        {
            ShouldPop = true;
        }
    }
    private void FoundMushroomExit()
    {
        if (encounter != null)
        {
            if (((MushroomsEncounter)encounter).MushroomsEaten)
            {
                eventTile.element = Futile.atlasManager.GetElementWithName("game_tiles_32");
                GameVars.Instance.TileHelper.SetTilePropertyValue(tileLocation, "MUSHROOMS_EATEN", true);
            }
        }
    }

    private void FoundMusicEnter()
    {
        bool musicFound = GameVars.Instance.GetParamValueBool("MUSIC_FOUND");

        if (!musicFound)
        {
            FTextDisplayScene musicScene = new FTextDisplayScene("Found Music", "There are papers scattered here. It appears to be some kind of sheet music. You pick them up and take them with you.");
            FSceneManager.Instance.PushScene(musicScene);
        }

        ShouldPop = true;
    }

    private void FoundMusicExit()
    {
        eventTile.element = Futile.atlasManager.GetElementWithName("game_tiles_32");
        GameVars.Instance.SetParamValue("MUSIC_FOUND", true);

        GameVars.Instance.MUSIC_FOUND = true;
    }

    private void FoundPornEnter()
    {
        bool pornoFound = GameVars.Instance.GetParamValueBool("PORN_FOUND");

        if (!pornoFound)
        {
            FTextDisplayScene pornScene = new FTextDisplayScene("Found Porn", "There seems to be a magazine stuffed in the bushes here... Whoa, naked ladies!");
            FSceneManager.Instance.PushScene(pornScene);
        }

        ShouldPop = true;
    }

    private void FoundPornExit()
    {
        eventTile.element = Futile.atlasManager.GetElementWithName("game_tiles_18");
        GameVars.Instance.SetParamValue("PORN_FOUND", true);

        GameVars.Instance.PORN_FOUND = true;
    }

    private void GetWaterEnter()
    {
        if (GameVars.Instance.Player.Water < GameVars.Instance.PLAYER_FULL_WATER)
        {
            encounter = new GetWaterEncounter("GET_WATER");
            FSceneManager.Instance.PushScene(encounter);
        }
        else
        {
            ShouldPop = true;
        }
    }
    private void GetWaterExit()
    {

    }

    private void BearEncounterEnter()
    {
        encounter = new BearEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void BearEncounterExit()
    {
        if (((BearEncounter)encounter).eatenByBears)
        {
            GameVars.Instance.SetParamValue(GameVarParams.POINTS.ToString(), "BEAR END");
            GameVars.Instance.EATEN_BY_BEARS = true;
            GameVars.Instance.SetParamValue(GameVarParams.WIN_MESSAGE.ToString(), "You were eaten by bears, yay!\n" + 
                "That doesn't happen every day, you must be... BEARY lucky!");
            ((FWorldScene)this.Parent).DoGameWon();
        }
    }

    private void WolfEncounterEnter()
    {
        encounter = new WolfEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void WolfEncounterExit() { }

    private void SnakeEncounterEnter()
    {
        encounter = new SnakeEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void SnakeEncounterExit() { }

    private void CougarEncounterEnter()
    {
        encounter = new CougarEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void CougarEncounterExit() { }

    private void FindFoodQuizEncounterEnter()
    {
        encounter = new FindFoodQuizEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void FindFoodQuizEncounterExit() { }

    private void BathroomQuizEncounterEnter()
    {
        encounter = new BathroomQuizEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void BathroomQuizEncounterExit() { }

    private void FishingQuizEncounterEnter()
    {
        encounter = new FishingQuizEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void FishingQuizEncounterExit() { }

    private void QuicksandQuizEncounterEnter()
    {
        encounter = new QuicksandQuizEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void QuicksandQuizEncounterExit() { }

    private void MakeFireQuizEncounterEnter()
    {
        encounter = new MakeFireQuizEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void MakeFireQuizEncounterExit() { }

    private void MakeCampQuizEncounterEnter()
    {
        encounter = new MakeCampQuizEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void MakeCampQuizEncounterExit() { }

    private void MudslideQuizEncounterEnter()
    {
        encounter = new MudslideQuizEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void MudslideQuizEncounterExit() { }

    private void BearTrapQuizEncounterEnter()
    {
        encounter = new BearTrapQuizEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void BearTrapQuizEncounterExit() { }

    private void NavigationQuizEncounterEnter()
    {
        encounter = new NavigationQuizEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void NavigationQuizEncounterExit() { }

    private void LightningQuizEncounterEnter()
    {
        encounter = new LightningQuizEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void LightningQuizEncounterExit() { }

    private void TickEncounterEnter()
    {
        encounter = new TickEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void TickEncounterExit() { }

    private void BeesEncounterEnter()
    {
        encounter = new BeesEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void BeesEncounterExit() { }

    private void RaccoonEncounterEnter()
    {
        encounter = new RaccoonEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void RaccoonEncounterExit() { }

    private void AntsEncounterEnter()
    {
        encounter = new AntsEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void AntsEncounterExit() { }

    private void SpiderEncounterEnter()
    {
        encounter = new SpiderEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void SpiderEncounterExit() { }

    private void WildPlantEncounterEnter()
    {
        encounter = new WildPlantEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void WildPlantEncounterExit() { }

    private void FeralAnumEncounterEnter()
    {
        encounter = new FeralAnumEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void FeralAnumEncounterExit() { }

    private void ButterBugEncounterEnter()
    {
        encounter = new ButterBugEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void ButterBugEncounterExit() { }

    private void OptorchidEncounterEnter()
    {
        encounter = new OptorchidEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void OptorchidEncounterExit() { }

    private void UnknownEncounterEnter()
    {
        encounter = new UnknownEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void UnknownEncounterExit() { }

    private void SecretWorldEncounterEnter()
    {
        encounter = new SecretWorldEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void SecretWorldEncounterExit()
    {
        GameVars.Instance.Player.Level += 5;
        //GameVars.Instance.Player.speed *= 2;
        GameVars.Instance.Player.SpeedMultiplier = 2;
        GameVars.Instance.Player.AttackPower += 20;
        GameVars.Instance.Player.Defense += 5;
        GameVars.Instance.Player.IsFloating = true;

        GameVars.Instance.Player.FullHeal();
    }

    private void DawnCrystalEncounterEnter()
    {
        encounter = new DawnCrystalEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void DawnCrystalEncounterExit()
    {
        GameVars.Instance.Player.Level += 10;
        GameVars.Instance.Player.AttackPower += 50;
        GameVars.Instance.Player.Defense += 50;

        GameVars.Instance.Player.FullHeal();
    }

    private void EsmudohrEncounterEnter()
    {
        encounter = new EsmudohrBattleEncounter();
        FSceneManager.Instance.PushScene(encounter);
    }
    private void EsmudohrEncounterExit()
    {
        if (((EsmudohrBattleEncounter)encounter).esmudohrDefeated)
        {
            GameVars.Instance.SetParamValue(GameVarParams.WIN_MESSAGE.ToString(), "You defeated Esmudohr!");
            GameVars.Instance.SetParamValue(GameVarParams.ROCKY_BEATEN.ToString(), true);
            GameVars.Instance.ROCKY_BEATEN = true;
        }
    }
}
