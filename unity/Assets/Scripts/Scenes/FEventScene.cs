using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class FEventHandlerScene : FScene
{
    protected Vector2 tileLocation;
    protected IPTile eventTile;

    protected FEncounterScene encounter;

    private bool ShouldPop = false;

    public FEventHandlerScene(string _name = "Default")
        : base(_name)
	{
		mName = _name;
	}

    public FEventHandlerScene(string eventName, IPTile eventTile, Vector2 tileLocation)
    {
        this.mName = eventName;
        this.tileLocation = tileLocation;
        this.eventTile = eventTile;
    }
	
	public override void OnUpdate ()
	{
        if (this.Paused)
        {
            return;
        }

        if (encounter != null && encounter.IsFinished)
        {
            ShouldPop = true;
        }

        if (ShouldPop)
        {
            FSceneManager.Instance.PopScene();
        }
	}

    public override void OnEnter()
	{
        //decide which event to enter
        switch (mName)
        {
            case "FOUND_MUSHROOMS": FoundMushroomEnter();
                break;
            default:
                break;
        }
	}

    public override void OnExit()
	{
        //decide which event to exit
        switch (mName)
        {
            case "FOUND_MUSHROOMS": FoundMushroomExit();
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
}
