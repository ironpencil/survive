using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class EncounterEvent
{
    public EncounterSource Source { get; private set; }

    public string Name { get; private set; }
    public string Text { get; private set; }

    public IPTiledObject EventObject { get; private set; }
    public IPTile EventTile { get; private set; }

    private EncounterEvent() { }

    public static EncounterEvent CreateObjectEvent(string eventName, IPTiledObject eventObject) {
        EncounterEvent objectEvent = new EncounterEvent();
        objectEvent.Source = EncounterSource.OBJECT;
        objectEvent.Name = eventName;
        objectEvent.EventObject = eventObject;
        objectEvent.EventTile = null;
        objectEvent.Text = "";

        return objectEvent;
    }

    public static EncounterEvent CreateTileEvent(string eventName, IPTile eventTile)
    {
        EncounterEvent tileEvent = new EncounterEvent();
        tileEvent.Source = EncounterSource.TILE;
        tileEvent.Name = eventName;
        tileEvent.EventTile = eventTile;
        tileEvent.EventObject = null;
        tileEvent.Text = "";

        return tileEvent;
    }

    public static EncounterEvent CreateRandomEvent(string eventName)
    {
        EncounterEvent randomEvent = new EncounterEvent();
        randomEvent.Source = EncounterSource.RANDOM;
        randomEvent.Name = eventName;
        randomEvent.EventTile = null;
        randomEvent.EventObject = null;
        randomEvent.Text = "";

        return randomEvent;
    }

    public static EncounterEvent CreateTextEvent(string eventName, string eventText)
    {
        EncounterEvent randomEvent = new EncounterEvent();
        randomEvent.Source = EncounterSource.TEXT;
        randomEvent.Name = eventName;
        randomEvent.EventTile = null;
        randomEvent.EventObject = null;
        randomEvent.Text = eventText;

        return randomEvent;
    }
}
