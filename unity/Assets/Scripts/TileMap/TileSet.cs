using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TileSet
{


    public int FirstGID { get; set; }

    public string Image { get; set; }

    public string Name { get; set; }

    public Dictionary<string, object> TileProperties { get; set; }

    private string assetBase = "";
    public string GetAssetBase()
    {
        if (assetBase.Length > 0)
        {
            return assetBase;
        }

        int extensionIndex = Image.LastIndexOf('.');

        assetBase = Image.Substring(0, extensionIndex);

        return assetBase;
    }

    public int TileWidth { get; set; }

    public int TileHeight { get; set; }
}
