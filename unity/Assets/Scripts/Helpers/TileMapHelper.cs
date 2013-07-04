using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TileMapHelper
{
    public FTilemap TileMap { get; set; }

    public TileMapHelper(FTilemap tileMap)
    {
        this.TileMap = tileMap;
    }

    #region TileHelpers

    public Vector2 GetTilePosition(Vector2 tile)
    {
        Vector2 desiredTilePosition = Vector2.zero;

        desiredTilePosition.x += (tile.x * TileMap.tileWidth);
        desiredTilePosition.y += (tile.y * TileMap.tileHeight) - TileMap.height;
        //desiredTilePosition.y += (tile.y * TILE_HEIGHT);

        //desiredTilePosition.x += Futile.screen.halfWidth + (background.width / 2);
        //desiredTilePosition.y -= Futile.screen.halfHeight + (background.height / 2);

        //desiredTilePosition -= GetTilePosition(tile);

        return desiredTilePosition;
    }

    public Vector2 GetSizeInTiles(Vector2 sizeInPixels)
    {
        int x = TileMap.widthInTiles;
        int y = TileMap.heightInTiles;

        return new Vector2(x, y);
    }

    public Vector2 GetCenterTile()
    {
        Vector2 centerTile = new Vector2();


        centerTile.x = (int)(TileMap.widthInTiles / 2);
        centerTile.y = (int)(TileMap.heightInTiles / 2);

        return centerTile;
    }

    #endregion
}
