using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LayerTileData
{
    public int GID { get; set; }

    public TileLayer Layer { get; set; }

    public TileSet TileSet { get; set; }

    public int TileX { get; set; }

    public int TileY { get; set; }

    private string assetName = "";

    public string GetAssetName()
    {
        if (assetName.Length > 0)
        {
            return assetName;
        }

        int assetID = GID - TileSet.FirstGID;

        //assetID++; //asset IDs start at 1

        assetName = TileSet.GetAssetBase() + "_" + assetID; // + ".png";

        return assetName;
    }

    public string GetPropertyValue(string propertyName)
    {
        string propertyValue = "";

        string propertyKey = TileSet.GeneratePropertyKey(GID, propertyName);

        propertyValue = TileSet.GetPropertyValue(propertyKey);

        return propertyValue;
    }
}
