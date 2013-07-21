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
}
