using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TileLayer : MapLayer
{
    private List<LayerTile> tiles = new List<LayerTile>();
    public List<LayerTile> Tiles { get { return tiles; } }

    private LayerTile[,] tileArray = null;

    public void InitializeTileArray(int widthInTiles, int heightInTiles) {

        tileArray = new LayerTile[widthInTiles, heightInTiles];        

    }

    public void AddTile(LayerTile tile)
    {
        tiles.Add(tile);
        int tileX = tile.TileData.TileX;
        int tileY = tile.TileData.TileY;

        tileArray[tileX, tileY] = tile;

        this.AddChild(tile);
    }

    public void RemoveTile(LayerTile tile)
    {
        tiles.Remove(tile);
        int tileX = tile.TileData.TileX;
        int tileY = tile.TileData.TileY;

        LayerTile tileToRemove = tileArray[tileX, tileY];

        if (tile == tileToRemove)
        {
            tileArray[tileX, tileY] = null;
        }

        this.RemoveChild(tile);
    }

    public LayerTile GetTileAt(int tileX, int tileY)
    {
        return tileArray[tileX, tileY];
    }

    public Vector2 GetTilePosition(int tileX, int tileY)
    {
        return GetTileAt(tileX, tileY).GetPosition();
    }

}
