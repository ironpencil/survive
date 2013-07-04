using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LayerTile : FSprite
{

    public LayerTileData TileData;

    public LayerTile(LayerTileData tileData) : base(tileData.GetAssetName())
    {
        this.TileData = tileData;
        //this.anchorX = 0;
        //this.anchorY = 1;
    }

}
