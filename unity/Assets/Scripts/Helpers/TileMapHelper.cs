using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TileMovementManager
{
    public IPTileMap TileMap { get; set; }

    public TileMovementManager(IPTileMap tileMap)
    {
        this.TileMap = tileMap;
    }

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
