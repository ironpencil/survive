using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class IPTileLayer : IPMapLayer
{

    private List<IPTile> tiles = new List<IPTile>();
    public List<IPTile> Tiles { get { return tiles; } }

    private int[,] gids = null;
    private bool[,] tilesLoaded = null;
    private IPTile[,] tileArray = null;

    public IPTileLayer(IPTileMap tileMapParent) : base(tileMapParent) { }

    public void InitializeTileArray(int widthInTiles, int heightInTiles) {

        tileArray = new IPTile[widthInTiles, heightInTiles];
        gids = new int[widthInTiles, heightInTiles];
        tilesLoaded = new bool[widthInTiles, heightInTiles];

    }

    public void SetGID(int tileX, int tileY, int gid)
    {
        gids[tileX, tileY] = gid;
    }

    public bool IsTileLoaded(int tileX, int tileY)
    {
        bool tileLoaded = false;
        try
        {
            tileLoaded = tilesLoaded[tileX, tileY];
        }
        catch { }
        
        return tileLoaded;
    }



    public void AddTile(IPTile tile)
    {
        tiles.Add(tile);
        int tileX = tile.TileData.TileX;
        int tileY = tile.TileData.TileY;

        tileArray[tileX, tileY] = tile;

        tilesLoaded[tileX, tileY] = true;

        //this.AddChild(tile);
    }

    public void RemoveTile(IPTile tile)
    {
        tiles.Remove(tile);
        int tileX = tile.TileData.TileX;
        int tileY = tile.TileData.TileY;

        IPTile tileToRemove = tileArray[tileX, tileY];

        if (tile == tileToRemove)
        {
            tileArray[tileX, tileY] = null;
        }

        this.RemoveChild(tile);
    }

    public void ShowTile(int tileX, int tileY)
    {

        IPTile tile = LoadTile(tileX, tileY);
        //may have to load the tile first       

        ShowLoadedTile(tile);
    }

    public void HideTile(int tileX, int tileY)
    {
        IPTile tile;
        //check to see if tile is loaded... if not, ignore it
        //because it's not visible anyway
        if (IsTileLoaded(tileX, tileY))
        {
            tile = tileArray[tileX, tileY];
            HideLoadedTile(tile);
        }
    }

    public IPTile LoadTile(int tileX, int tileY)
    {

        if (IsTileLoaded(tileX, tileY))
        {
            return tileArray[tileX, tileY];
        }

        int tileGID = gids[tileX, tileY];

        if (tileGID == 0)
        {
            tilesLoaded[tileX, tileY] = true;
            return null;
        }

        //Load the tile
        IPTileData tileData = new IPTileData();

        tileData.Layer = this;
        tileData.GID = tileGID;
        tileData.TileSet = tileMapParent.FindTileSetContainingGID(tileData.GID);

        tileData.TileX = tileX;
        //tileData.TileY = this.HeightInTiles - tileY - 1; //TileY should count up from the bottom
        tileData.TileY = tileY;

        IPTile tile = new IPTile(tileData);

        tile.x = tileData.TileX * tileData.TileSet.TileWidth;
        tile.y = tileData.TileY * tileData.TileSet.TileHeight;

        //the tile physically resides in the TileLayer container
        //when the tile layer is added to the container, the tile will be as well
        this.AddTile(tile);

        return tile;
    }


    public void ShowLoadedTile(IPTile tile)
    {
        if (tile != null)
        {
            this.AddChild(tile);
            tile.isVisible = true;
        }
    }

    public void HideLoadedTile(IPTile tile)
    {
        if (tile != null)
        {
            this.RemoveChild(tile);
            tile.isVisible = false;
        }
    }

    public IPTile GetTileAt(int tileX, int tileY)
    {
        return LoadTile(tileX, tileY);
    }

    public Vector2 GetTilePosition(int tileX, int tileY)
    {
        float xPos = tileX * this.TileWidth;
        float yPos = tileY * this.TileHeight;

        return new Vector2(xPos, yPos);

        //return GetTileAt(tileX, tileY).GetPosition();
    }

}
