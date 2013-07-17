using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TileMapHelper
{
    public IPTileMap TileMap { get; set; }

    public TileMapHelper(IPTileMap tileMap)
    {
        this.TileMap = tileMap;
    }

#region TileProperties
    private Dictionary<Vector2, Dictionary<string, object>> allTileProperties = new Dictionary<Vector2, Dictionary<string, object>>();

    public object GetTilePropertyValue(Vector2 tile, string paramName)
    {
        Dictionary<string, object> tileProperties;

        if (allTileProperties.TryGetValue(tile, out tileProperties))
        {
            object paramValue = null;

            tileProperties.TryGetValue(paramName, out paramValue);

            return paramValue;
        }

        return null;
    }

    public string GetTilePropertyValueString(Vector2 tile, string paramName)
    {
        string returnValue = "";
        object paramValue = this.GetTilePropertyValue(tile, paramName);

        try
        {
            returnValue = paramValue.ToString();
        }
        catch { }

        return returnValue;
    }

    public bool GetTilePropertyValueBool(Vector2 tile, string paramName)
    {
        bool returnValue = false;
        object paramValue = this.GetTilePropertyValue(tile, paramName);

        try
        {
            returnValue = bool.Parse(paramValue.ToString());
        }
        catch { }

        return returnValue;
    }

    public void SetTilePropertyValue(Vector2 tile, string paramName, object paramValue)
    {
        if (allTileProperties.ContainsKey(tile))
        {
            Dictionary<string, object> tileProperties = allTileProperties[tile];

            if (tileProperties.ContainsKey(paramName))
            {
                tileProperties[paramName] = paramValue;
            }
            else
            {
                tileProperties.Add(paramName, paramValue);
            }
        }
        else
        {
            Dictionary<string, object> newProperty = new Dictionary<string, object>();
            newProperty.Add(paramName, paramValue);
            allTileProperties.Add(tile, newProperty);
        }
    }
#endregion


    #region TileHelpers

    public Vector2 GetCenterTile()
    {
        Vector2 centerTile = new Vector2();


        centerTile.x = (int)(TileMap.WidthInTiles / 2);
        centerTile.y = (int)(TileMap.HeightInTiles / 2);

        return centerTile;
    }

    #endregion
}
