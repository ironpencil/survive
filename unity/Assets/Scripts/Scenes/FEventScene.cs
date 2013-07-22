using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class FEventHandlerLayer : FLayer
{
    protected Vector2 tileLocation;
    protected IPTile eventTile;

    protected FEncounterScene encounter;

    private bool ShouldPop = false;

    public string Name { get; private set; }

    public FEventHandlerLayer(FScene parent, string name) : base(parent)
	{
		this.Name = name;
	}

    public FEventHandlerLayer(FScene parent, string eventName, IPTile eventTile, Vector2 tileLocation) : base(parent)
    {
        this.Name = eventName;
        this.tileLocation = tileLocation;
        this.eventTile = eventTile;        
    }
	
	public override void OnUpdate ()
	{
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
            case "FOUND_MUSHROOMS": FoundMushroomEnter();
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
            default:
                break;
        }
	}   

    public override void OnExit()
	{
        //decide which event to exit
        switch (Name)
        {
            case "FOUND_MUSHROOMS": FoundMushroomExit();
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
            default:
                break;
        }
	}

    private void FoundMushroomEnter()
    {
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
                GameVars.Instance.TileHelper.SetTilePropertyValue(tileLocation, "MUSHROOMS_EATEN", true);
            }
        }
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
    private void BearEncounterExit() { }

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
}
