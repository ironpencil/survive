using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class IPTileLayer : IPMapLayer
{

    //private List<IPTile> tiles = new List<IPTile>();
    //public List<IPTile> Tiles { get { return tiles; } }

    private int[,] gids = null;
    private bool[,] tilesLoaded = null;
    private IPTile[,] tileArray = null;

    private IPTileData[,] tileDataArray = null;

    public IPTileLayer(IPTileMap tileMapParent) : base(tileMapParent) { }

    public void InitializeTileArray(int widthInTiles, int heightInTiles) {

        tileArray = new IPTile[widthInTiles, heightInTiles];
        gids = new int[widthInTiles, heightInTiles];
        tilesLoaded = new bool[widthInTiles, heightInTiles];
        tileDataArray = new IPTileData[widthInTiles, heightInTiles];

        for (int i = 0; i < widthInTiles; i++)
        {
            for (int j = 0; j < heightInTiles; j++)
            {
                tilesLoaded[i, j] = false;
            }
        }

    }

    public void SetGID(int tileX, int tileY, int gid)
    {
        gids[tileX, tileY] = gid;
    }

    public bool IsTileLoaded(int tileX, int tileY)
    {
        bool tileLoaded = false;
        //try
        //{
            tileLoaded = tilesLoaded[tileX, tileY];
        //}
        //catch { }
        
        return tileLoaded;
    }



    public void AddTile(IPTile tile)
    {
        //tiles.Add(tile);
        int tileX = tile.TileData.TileX;
        int tileY = tile.TileData.TileY;

        tileArray[tileX, tileY] = tile;

        tilesLoaded[tileX, tileY] = true;

        //this.AddChild(tile);
    }

    public void RemoveTile(IPTile tile)
    {
        //tiles.Remove(tile);
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

        int tileGID = gids[tileX, tileY];

        if (tileGID == 0)
        {
            tilesLoaded[tileX, tileY] = true;
            return null;
        }

        //if (IsTileLoaded(tileX, tileY))
        //{

        IPTile tile = tileArray[tileX, tileY];

        if (tile != null)
        {
            return tile;
        }
        //}

        IPDebug.ForceLog("Tile not loaded, have to load it: " + tileX + "," + tileY);
        ////IPDebug.ForceAllowed = true;

        //float startTime = Time.realtimeSinceStartup * 1000;        

        //Load the tile
        IPTileData tileData = LoadTileData(tileX, tileY, tileGID, false);

        //endTime = Time.realtimeSinceStartup * 1000;
        //timeDiff = endTime - startTime;

        ////IPDebug.ForceLog("2:" + timeDiff);

        //startTime = Time.realtimeSinceStartup * 1000;

        tile = new IPTile(tileData);

        // endTime = Time.realtimeSinceStartup * 1000;
        // timeDiff = endTime - startTime;

        //// IPDebug.ForceLog("3:" + timeDiff);
        // startTime = Time.realtimeSinceStartup * 1000;

        tile.x = tileX * tileData.TileSet.TileWidth;
        tile.y = tileY * tileData.TileSet.TileHeight;


        //the tile physically resides in the TileLayer container
        //when the tile layer is added to the container, the tile will be as well
        this.AddTile(tile);

        //endTime = Time.realtimeSinceStartup * 1000;
        //timeDiff = endTime - startTime;

        //IPDebug.ForceLog("4:" + timeDiff);        



        //IPDebug.ForceLog("Loading tile[x,y] : " + tileX + "," + tileY);

        //IPDebug.ForceAllowed = false;

        return tile;
    }

    public IPTileData LoadTileData(int tileX, int tileY, bool updateOnly)
    {
        int tileGID = gids[tileX, tileY];

        if (tileGID == 0)
        {
            //tilesLoaded[tileX, tileY] = true;
            return null;
        }

        return LoadTileData(tileX, tileY, tileGID, updateOnly);
    }

    //setting update only won't attempt to load existing data, it will only generate and store a new tile based on the data provided
    public IPTileData LoadTileData(int tileX, int tileY, int tileGID, bool updateOnly)
    {
        //gid is being provided, so we assume it is valid/not 0

        IPTileData tileData;

        if (!updateOnly)
        {

            tileData = tileDataArray[tileX, tileY];

            if (tileData != null)
            {
                return tileData;
            }
        }

        IPDebug.ForceLog("Loading tile data for tile [x,y]: " + tileX + "," + tileY);

        tileData = new IPTileData();

        //float endTime = Time.realtimeSinceStartup * 1000;
        //float timeDiff = endTime - startTime;

        ////IPDebug.ForceLog("1:" + timeDiff);

        //startTime = Time.realtimeSinceStartup * 1000;

        tileData.Layer = this;
        tileData.GID = tileGID;
        tileData.TileSet = tileMapParent.FindTileSetContainingGID(tileData.GID);


        tileData.TileX = tileX;
        //tileData.TileY = this.HeightInTiles - tileY - 1; //TileY should count up from the bottom
        tileData.TileY = tileY;

        //puts the asset name together
        tileData.GenerateAssetName();

        tileDataArray[tileX, tileY] = tileData;

        return tileData;
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
