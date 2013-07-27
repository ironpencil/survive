using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class IPTileData
{
    public int GID { get; set; }

    public IPTileLayer Layer { get; set; }

    public IPTileSet TileSet { get; set; }

    public int TileX { get; set; }

    public int TileY { get; set; }

    private string assetName = null;

    public string GetAssetName()
    {
        if (assetName != null)
        {
            return assetName;
        }

        GenerateAssetName();

        return assetName;
    }

    public void GenerateAssetName()
    {
        int assetID = GID - TileSet.FirstGID;

        assetName = TileSet.GetAssetBase() + "_" + assetID; // + ".png";
    }

    public string GetPropertyValue(string propertyName)
    {
        string propertyValue = "";

        string propertyKey = TileSet.GeneratePropertyKey(GID, propertyName);

        propertyValue = TileSet.GetPropertyValue(propertyKey);

        return propertyValue;
    }

    public bool PropertyExists(string propertyName)
    {
        string propertyKey = TileSet.GeneratePropertyKey(GID, propertyName);

        return TileSet.PropertyExists(propertyKey);
    }
}
