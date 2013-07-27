using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class IPTileMap : FContainer
{

    //private MapTile[,] tiles;

    private int tileLoaderXIndex = 0;
    private int tileLoaderYIndex = 0;
    private bool allTilesLoaded = false;

    private List<IPTileLayer> tileLayers;
    public List<IPTileLayer> TileLayers { get { return tileLayers; } set { tileLayers = value; } }

    private List<IPObjectLayer> objectLayers;
    public List<IPObjectLayer> ObjectLayers { get { return objectLayers; } set { objectLayers = value; } }

    public string Name { get; set; }

    private Dictionary<int, IPTileSet> tileSets;

    //these two collections are only used to hold tile data during setup
    private List<int> tileSetFirstGIDs;
    private Dictionary<int, IPTileSet> tileSetFoundGIDs;

    public int Width { get { return TileWidth * WidthInTiles; } }
    public int Height { get { return TileHeight * HeightInTiles; } }

    public int TileWidth { get; set; }
    public int TileHeight { get; set; }

    public int WidthInTiles { get; set; }
    public int HeightInTiles { get; set; }
  
    private string mapFile;
    Dictionary<string, object> mapData;

    public bool LoadAllTilesAtInitialization { get; set; }

    public IPTileMap(string name, string mapFile, bool loadAllTilesAtInitialization)
    {
        Name = name;
        this.mapFile = mapFile;
        this.LoadAllTilesAtInitialization = loadAllTilesAtInitialization;
        allTilesLoaded = loadAllTilesAtInitialization;
    }

    public void SetAllVisible()
    {
        foreach (IPTileLayer tileLayer in tileLayers)
        {
            for (int tileY = 0; tileY < HeightInTiles; tileY++)
            {
                for (int tileX = 0; tileX < WidthInTiles; tileX++)
                {
                    tileLayer.ShowTile(tileX, tileY);
                }
            }
        }
    }


    public void LoadMoreTiles(int tilesToLoad)
    {

        if (allTilesLoaded) { return; }

        int newLoadingRow = 0;
        int newLoadingCol = 0;

        foreach (IPTileLayer tileLayer in tileLayers)
        {
            newLoadingRow = tileLoaderYIndex;
            newLoadingCol = tileLoaderXIndex;
            int tilesLoaded = 0;

            for (int col = newLoadingCol; col < WidthInTiles; col++)            
            {
                for (int row = newLoadingRow; row < HeightInTiles; row++)
                {
                    tileLayer.LoadTile(col, row);
                    IPDebug.ForceLog("Loading more tiles[x,y] : " + col + "," + row);
                    tilesLoaded++;

                    newLoadingRow = row;
                    if (tilesLoaded > tilesToLoad) { break; }
                }

                if (newLoadingRow == (HeightInTiles - 1))
                {
                    //end of column, reset row index back to 0
                    newLoadingRow = 0;
                }

                newLoadingCol = col;
                if (tilesLoaded > tilesToLoad) { break; }
            }
        }

        tileLoaderYIndex = newLoadingRow;
        tileLoaderXIndex = newLoadingCol;

        if (tileLoaderXIndex == (WidthInTiles - 1) && tileLoaderYIndex == (HeightInTiles - 1))
        {
            allTilesLoaded = true;
            bool forceAllowed = IPDebug.ForceAllowed;
            IPDebug.ForceAllowed = true;
            IPDebug.ForceLog("All tiles loaded. Took " + Time.time);
            IPDebug.ForceAllowed = forceAllowed;
        }
    }

    public void LoadTilesInRange(Vector2 centerTile, Vector2 tileRange)
    {

        int minX = (int)(centerTile.x - tileRange.x);
        int maxX = (int)(centerTile.x + tileRange.x);

        int minY = (int)(centerTile.y - tileRange.y);
        int maxY = (int)(centerTile.y + tileRange.y);

        if (minX < 0) { minX = 0; }
        if (maxX > WidthInTiles) { maxX = WidthInTiles; }

        if (minY < 0) { minY = 0; }
        if (maxY > HeightInTiles) { maxY = HeightInTiles; }

        //if (minX < 0) { minX = 0; }
        //if (maxX >= WidthInTiles) { maxX = WidthInTiles - 1; }

        //if (minY < 0) { minY = 0; }
        //if (maxY >= HeightInTiles) { maxY = HeightInTiles - 1; }        

        foreach (IPTileLayer tileLayer in tileLayers)
        {

            //don't have to iterate through all rows because we're just loading nearby tiles
            for (int row = minY; row < maxY; row++)
            {
                for (int col = minX; col < maxX; col++)
                {
                    tileLayer.LoadTile(col, row);
                    IPDebug.ForceLog("Loading Tile (Range) : " + col + "," + row);
                }
            }


            //foreach (IPTile tile in tileLayer.Tiles)
            //{
            //    int xDistance = 0;
            //    int yDistance = 0;

            //    xDistance = Math.Abs(tile.TileData.TileX - (int) centerTile.x);
            //    yDistance = Math.Abs(tile.TileData.TileY - (int) centerTile.y);

            //    if (xDistance < tileRange.x && yDistance < tileRange.y)
            //    {
            //        tileLayer.ShowLoadedTile(tile);
            //    }
            //    else
            //    {
            //        tileLayer.HideLoadedTile(tile);
            //    }
            //}

        }

    }

    public void LoadAllTiles()
    {
        float startTime = Time.realtimeSinceStartup;
        foreach (IPTileLayer tileLayer in tileLayers)
        {

            for (int row = 0; row < HeightInTiles; row++)
            {
                for (int col = 0; col < WidthInTiles; col++)
                {
                        tileLayer.LoadTile(col, row);
                }
            }
        }
        float endTime = Time.realtimeSinceStartup;

        Debug.Log("All tile sprites loaded, took " + (endTime - startTime) + " seconds.");
    }

    public void LoadAllTileData()
    {
        float startTime = Time.realtimeSinceStartup;
        foreach (IPTileLayer tileLayer in tileLayers)
        {

            for (int row = 0; row < HeightInTiles; row++)
            {
                for (int col = 0; col < WidthInTiles; col++)
                {
                    tileLayer.LoadTileData(col, row, true);
                }
            }
        }
        float endTime = Time.realtimeSinceStartup;

        Debug.Log("All tile data loaded, took " + (endTime - startTime) + " seconds.");
    }


    public void SetVisibleRange(Vector2 centerTile, Vector2 tileRange)
    {

        int minVisX = (int) (centerTile.x - tileRange.x);
        int maxVisX = (int) (centerTile.x + tileRange.x);

        int minVisY = (int) (centerTile.y - tileRange.y);
        int maxVisY = (int) (centerTile.y + tileRange.y);

        int minSearchX = minVisX - 2;
        int maxSearchX = maxVisX + 2;

        int minSearchY = minVisY - 2;
        int maxSearchY = maxVisY + 2;

        //update the minimum/maximum range based on map indexes
        if (minSearchX < 0) { minSearchX = 0; }
        if (maxSearchX > WidthInTiles) { maxSearchX = WidthInTiles; }

        if (minSearchY < 0) { minSearchY = 0; }
        if (maxSearchY > HeightInTiles) { maxSearchY = HeightInTiles; }
        
        foreach (IPTileLayer tileLayer in tileLayers)
        {

            //have to iterate through ALL tiles because we may have to hide some out-of-range tiles
            for (int row = minSearchY; row < maxSearchY; row++)
            {
                for (int col = minSearchX; col < maxSearchX; col++)
                {

                    if (col < maxVisX && col > minVisX &&
                        row < maxVisY && row > minVisY)
                    {

                        tileLayer.ShowTile(col, row);
                    }
                    else
                    {
                        tileLayer.HideTile(col, row);
                    }
                }
            }
            

            //foreach (IPTile tile in tileLayer.Tiles)
            //{
            //    int xDistance = 0;
            //    int yDistance = 0;

            //    xDistance = Math.Abs(tile.TileData.TileX - (int) centerTile.x);
            //    yDistance = Math.Abs(tile.TileData.TileY - (int) centerTile.y);

            //    if (xDistance < tileRange.x && yDistance < tileRange.y)
            //    {
            //        tileLayer.ShowLoadedTile(tile);
            //    }
            //    else
            //    {
            //        tileLayer.HideLoadedTile(tile);
            //    }
            //}

        }

    }



    public void LoadTileDataFile()
    {
        tileSets = new Dictionary<int, IPTileSet>();
        tileLayers = new List<IPTileLayer>();
        objectLayers = new List<IPObjectLayer>();

        tileSetFirstGIDs = new List<int>();
        tileSetFoundGIDs = new Dictionary<int, IPTileSet>();

        TextAsset mapAsset = (TextAsset)Resources.Load(mapFile, typeof(TextAsset));

        if (!mapAsset)
        {
            //map asset not found, no error trapping though...
        }

        mapData = mapAsset.text.dictionaryFromJson();

        Resources.UnloadAsset(mapAsset);

        WidthInTiles = int.Parse(mapData["width"].ToString());
        HeightInTiles = int.Parse(mapData["height"].ToString());        
        TileWidth = int.Parse(mapData["tilewidth"].ToString());
        TileHeight = int.Parse(mapData["tileheight"].ToString());      

        List<object> tileSetsData = (List<object>)mapData["tilesets"];

        foreach (Dictionary<string, object> tileSetData  in tileSetsData)
        {
            IPTileSet tileSet = new IPTileSet();

            tileSet.FirstGID = int.Parse(tileSetData["firstgid"].ToString());
            tileSet.Image = tileSetData["image"].ToString();
            tileSet.Name = tileSetData["name"].ToString();
            tileSet.TileWidth = int.Parse(tileSetData["tilewidth"].ToString());
            tileSet.TileHeight = int.Parse(tileSetData["tileheight"].ToString());

            tileSet.SetupTileProperties((Dictionary<string, object>)tileSetData["tileproperties"]);

            tileSets.Add(tileSet.FirstGID, tileSet);
            tileSetFirstGIDs.Add(tileSet.FirstGID);
        }

        tileSetFirstGIDs.Sort();

        List<object> mapLayers = (List<object>)mapData["layers"];

        foreach (Dictionary<string, object> layerData in mapLayers)
        {

            IPMapLayer mapLayer = null;

            string layerType = layerData["type"].ToString();

            if (layerType.Equals("tilelayer")) {
                mapLayer = new IPTileLayer(this);
            }
            else if (layerType.Equals("objectgroup"))
            {
                mapLayer = new IPObjectLayer(this);
            }
            else
            {
                mapLayer = new IPMapLayer(this);
            }

            mapLayer.Name = layerData["name"].ToString();
            mapLayer.Visible = bool.Parse(layerData["visible"].ToString());
            mapLayer.WidthInTiles = int.Parse(layerData["width"].ToString());
            mapLayer.HeightInTiles = int.Parse(layerData["height"].ToString());
            mapLayer.Opacity = int.Parse(layerData["opacity"].ToString());
            mapLayer.LayerType = layerData["type"].ToString();
            mapLayer.LayerProperties = (Dictionary<string, object>)layerData["properties"];
            
            //these are being pulled from the map, assume same tile size for whole map
            mapLayer.TileWidth = TileWidth;
            mapLayer.TileHeight = TileHeight;            

            if (mapLayer is IPTileLayer) {

                IPTileLayer tileLayer = mapLayer as IPTileLayer;

                tileLayer.InitializeTileArray(mapLayer.WidthInTiles, tileLayer.HeightInTiles);
                //load the tile layer
                List<object> tileGIDs = (List<object>) layerData["data"];

                int x = 0;
                int y = 0;

                for (int i = 0; i < tileGIDs.Count; i++)
                {
                    int tileGID = int.Parse(tileGIDs[i].ToString());

                    //GID of 0 means there is no tile?
                    if (tileGID > 0)
                    {
                        int tileX = x;
                        int tileY = tileLayer.HeightInTiles - y - 1;

                        tileLayer.SetGID(tileX, tileY, tileGID);
                        //tileLayer.LoadTileData(tileX, tileY, tileGID, true);

                        if (LoadAllTilesAtInitialization)
                        {
                            tileLayer.LoadTile(tileX, tileY);

                            //IPTileData tileData = new IPTileData();

                            //tileData.Layer = tileLayer;
                            //tileData.GID = int.Parse(tileGIDs[i].ToString());
                            //tileData.TileSet = FindTileSetContainingGID(tileData.GID);

                            //tileData.TileX = x;
                            //tileData.TileY = tileLayer.HeightInTiles - y - 1; //TileY should count up from the bottom

                            //IPTile tile = new IPTile(tileData);

                            //tile.x = tileData.TileX * tileData.TileSet.TileWidth;
                            //tile.y = tileData.TileY * tileData.TileSet.TileHeight;

                            //tile.isVisible = false;

                            ////the tile physically resides in the TileLayer container
                            ////when the tile layer is added to the container, the tile will be as well
                            //tileLayer.AddTile(tile);
                        }
                    }

                    x++;

                    if (x % mapLayer.WidthInTiles == 0)
                    {
                        x = 0;
                        y++;
                    }
                }

                this.tileLayers.Add(tileLayer);
                //should now have a tile layer that contains all the layer info
                //plus a list of tiles, each referencing the TileSet that its tile comes from
                
            }
            else if (mapLayer is IPObjectLayer)
            {
                IPObjectLayer objectLayer = mapLayer as IPObjectLayer;

                //load the object group
                List<object> objectDefs = (List<object>)layerData["objects"];


                //currently only supports rectangular objects with no tile image (gid)
                foreach (Dictionary<string, object> objectDef in objectDefs)
                {                    
                    IPTiledObject tiledObject = new IPTiledObject();

                    tiledObject.Layer = objectLayer;
                    //tiledObject.GID = objectDef["gid"].ToString();
                    tiledObject.Name = objectDef["name"].ToString();
                    tiledObject.ObjType = objectDef["type"].ToString();
                    tiledObject.Visible = bool.Parse(objectDef["visible"].ToString());
                    tiledObject.x = float.Parse(objectDef["x"].ToString());
                    tiledObject.y = float.Parse(objectDef["y"].ToString());
                    tiledObject.ObjWidth = float.Parse(objectDef["width"].ToString());
                    tiledObject.ObjHeight = float.Parse(objectDef["height"].ToString());
                    tiledObject.ObjProperties = (Dictionary<string, object>)objectDef["properties"];

                    //adjust y value for Futile (count upwards instead of downwards like in Tiled)
                    tiledObject.y = objectLayer.Height - tiledObject.y - objectLayer.TileHeight - ((tiledObject.ObjHeight - objectLayer.TileHeight)/2); //objectLayer.TileHeight; 
                    tiledObject.x += ((tiledObject.ObjWidth - objectLayer.TileWidth) / 2);

                    //FSprite rectSprite = new FSprite("Futile_White");
                    //FSprite rectCenter = new FSprite("Futile_White");
                    //rectCenter.color = Color.black;
                    //rectCenter.width = 5;
                    //rectCenter.height = 5;
                    //rectCenter.x = tiledObject.x;
                    //rectCenter.y = tiledObject.y;

                    //rectSprite.x = tiledObject.x;
                    //rectSprite.y = tiledObject.y;
                    //rectSprite.width = tiledObject.ObjWidth;
                    //rectSprite.height = tiledObject.ObjHeight;

                    //rectSprite.alpha = 0.5f;

                    //objectLayer.AddChild(rectSprite);
                    //objectLayer.AddChild(rectCenter);

                    objectLayer.AddObject(tiledObject);                    

                }

                this.objectLayers.Add(objectLayer);
            }    
      
            //done loading the layer, add it to this map
            //tileLayers.Add(mapLayer);
            this.AddChild(mapLayer);
        }

        //may need these after all, due to dynamic tile loading instead of loading all at once
        //these are only used during set-up, don't need them anymore
        //tileSetFirstGIDs = null;
        //tileSetFoundGIDs = null;

    }    

    public IPTileSet FindTileSetContainingGID(int gid)
    {
        if (tileSetFirstGIDs.Contains(gid))
        {
            return tileSets[gid];
        }

        if (tileSetFoundGIDs.ContainsKey(gid))
        {
            return tileSetFoundGIDs[gid];
        }

        IPDebug.Log("Searching for gid: " + gid);
        //do not know what tileset contains this gid, so figure it out
        //add our gid to the list of firstGIDs, sort it, then take the previous value

        tileSetFirstGIDs.Add(gid);

        tileSetFirstGIDs.Sort();

        int gidIndex = tileSetFirstGIDs.IndexOf(gid);

        int tileSetFirstGID = tileSetFirstGIDs[gidIndex - 1];

        IPTileSet foundTileSet = tileSets[tileSetFirstGID];

        //found the tileset, now clean up and save this gid for later
        tileSetFirstGIDs.RemoveAt(gidIndex);

        tileSetFoundGIDs.Add(gid, foundTileSet);

        return foundTileSet;
    }    

    public IPTileLayer GetTileLayer(string layerName)
    {
        IPTileLayer requestedLayer = null;

        foreach (IPTileLayer layer in tileLayers)
        {
            if (layer.Name.Equals(layerName))
            {
                requestedLayer = layer;
                break;
            }
        }

        return requestedLayer;
    }

    public IPTileLayer GetTileLayerWithProperty(string propertyName, string propertyValue)
    {
        IPTileLayer requestedLayer = null;

        foreach (IPTileLayer layer in tileLayers)
        {
            //just return the first layer that matches the specified property value
            if (layer.GetPropertyValue(propertyName).Equals(propertyValue)) {
                requestedLayer = layer;
                break;
            }
        }

        return requestedLayer;
    }

    public List<IPTileLayer> GetTileLayersWithProperty(string propertyName, string propertyValue)
    {
        List<IPTileLayer> requestedLayers = new List<IPTileLayer>();

        foreach (IPTileLayer layer in tileLayers)
        {
            if (layer.GetPropertyValue(propertyName).Equals(propertyValue))
            {
                requestedLayers.Add(layer);
            }
        }

        return requestedLayers;
    }

    public IPObjectLayer GetObjectLayer(string layerName)
    {
        IPObjectLayer requestedLayer = null;

        foreach (IPObjectLayer layer in objectLayers)
        {
            if (layer.Name.Equals(layerName))
            {
                requestedLayer = layer;
                break;
            }
        }

        return requestedLayer;
    }

    public IPObjectLayer GetObjectLayerWithProperty(string propertyName, string propertyValue)
    {
        IPObjectLayer requestedLayer = null;

        foreach (IPObjectLayer layer in objectLayers)
        {
            //just return the first layer that matches the specified property value
            if (layer.GetPropertyValue(propertyName).Equals(propertyValue))
            {
                requestedLayer = layer;
                break;
            }
        }

        return requestedLayer;
    }

    public List<IPObjectLayer> GetObjectLayersWithProperty(string propertyName, string propertyValue)
    {
        List<IPObjectLayer> requestedLayers = new List<IPObjectLayer>();
        //TileLayer requestedLayer = null;

        foreach (IPObjectLayer layer in objectLayers)
        {
            if (layer.GetPropertyValue(propertyName).Equals(propertyValue))
            {
                requestedLayers.Add(layer);
            }
        }

        return requestedLayers;
    }

}
