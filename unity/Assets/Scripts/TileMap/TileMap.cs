using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TileMapData : FContainer
{

    private MapTile[,] tiles;

    private List<MapLayer> layers;
    public List<MapLayer> Layers { get { return layers; } set { layers = value; } }

    public string Name { get; set; }

    private Dictionary<int, TileSet> tileSets;

    //these two collections are only used to cache tile data during setup
    private List<int> tileSetFirstGIDs;
    private Dictionary<int, TileSet> tileSetFoundGIDs;

    public int Width { get; set; }
    public int Height { get; set; }

    public int TileWidth { get; set; }
    public int TileHeight { get; set; }

    public int WidthInTiles { get; set; }
    public int HeightInTiles { get; set; }

    


    private string mapFile;
    Dictionary<string, object> mapData;

    public TileMapData(string name, string mapFile)
    {
        Name = name;
        this.mapFile = mapFile;
    }

    public void LoadTiles()
    {
        tileSets = new Dictionary<int, TileSet>();
        layers = new List<MapLayer>();

        tileSetFirstGIDs = new List<int>();
        tileSetFoundGIDs = new Dictionary<int, TileSet>();

        TextAsset mapAsset = (TextAsset)Resources.Load(mapFile, typeof(TextAsset));

        if (!mapAsset)
        {
            Debug.Log("Map file '" + mapFile + "' not found!");
            Debug.Break();
        }

        mapData = mapAsset.text.dictionaryFromJson();

        Resources.UnloadAsset(mapAsset);

        WidthInTiles = int.Parse(mapData["width"].ToString());
        HeightInTiles = int.Parse(mapData["height"].ToString());        
        TileWidth = int.Parse(mapData["tilewidth"].ToString());
        TileHeight = int.Parse(mapData["tileheight"].ToString());
        Width = WidthInTiles * TileWidth;
        Height = HeightInTiles * TileHeight;

        InitializeMapTiles();       

        List<object> tileSetsData = (List<object>)mapData["tilesets"];

        foreach (Dictionary<string, object> tileSetData  in tileSetsData)
        {
            TileSet tileSet = new TileSet();

            tileSet.FirstGID = int.Parse(tileSetData["firstgid"].ToString());
            tileSet.Image = tileSetData["image"].ToString();
            tileSet.Name = tileSetData["name"].ToString();
            tileSet.TileWidth = int.Parse(tileSetData["tilewidth"].ToString());
            tileSet.TileHeight = int.Parse(tileSetData["tileheight"].ToString());

            tileSet.SetupTileProperties((Dictionary<string, object>)tileSetData["tileproperties"]);

            Debug.Log(tileSet.GetTilePropertyDescription());

            tileSets.Add(tileSet.FirstGID, tileSet);
            tileSetFirstGIDs.Add(tileSet.FirstGID);
        }

        tileSetFirstGIDs.Sort();

        List<object> mapLayers = (List<object>)mapData["layers"];

        foreach (Dictionary<string, object> layerData in mapLayers)
        {

            MapLayer mapLayer = null;

            string layerType = layerData["type"].ToString();

            if (layerType.Equals("tilelayer")) {
                mapLayer = new TileLayer();
            }
            else if (layerType.Equals("objectgroup"))
            {
                mapLayer = new ObjectLayer();
            }
            else
            {
                Debug.Log("Unknown layer type!");
                mapLayer = new MapLayer();
            }

            mapLayer.Name = layerData["name"].ToString();
            mapLayer.Visible = bool.Parse(layerData["visible"].ToString());
            mapLayer.Width = int.Parse(layerData["width"].ToString());
            mapLayer.Height = int.Parse(layerData["height"].ToString());
            mapLayer.Opacity = int.Parse(layerData["opacity"].ToString());
            mapLayer.LayerType = layerData["type"].ToString();
            mapLayer.LayerProperties = (Dictionary<string, object>)layerData["properties"];

            if (mapLayer is TileLayer) {
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
                        TileLayer tileLayer = mapLayer as TileLayer;

                        LayerTileData tileData = new LayerTileData();

                        tileData.Layer = tileLayer;
                        tileData.GID = int.Parse(tileGIDs[i].ToString());
                        tileData.TileSet = FindTileSetContainingGID(tileData.GID);

                        tileData.TileX = x;
                        tileData.TileY = tileLayer.Height - y - 1; //TileY should count up from the bottom

                        x++;

                        if (x % mapLayer.Width == 0)
                        {
                            x = 0;
                            y++;
                        }

                        LayerTile tile = new LayerTile(tileData);

                        tile.x = tileData.TileX * tileData.TileSet.TileWidth; // -tileData.TileSet.TileWidth;
                        tile.y = tileData.TileY * tileData.TileSet.TileHeight; // -tileData.TileSet.TileHeight + tileLayer.Height;

                        //the tile physicall resides in the TileLayer container
                        tileLayer.tiles.Add(tile);
                        tileLayer.AddChild(tile);

                        //add a reference to this tile to our MapTiles array
                        tiles[tileData.TileX, tileData.TileY].LayerTiles.Add(tileLayer, tile);
                    }
                }

                //should now have a tile layer that contains all the layer info
                //plus a list of tiles, each referencing the TileSet that its tile comes from
                
            }
            else if (mapLayer is ObjectLayer)
            {
                //load the object group
                List<object> objectDefs = (List<object>)layerData["objects"];

                foreach (Dictionary<string, object> objectDef in objectDefs)
                {
                    ObjectLayer objectLayer = mapLayer as ObjectLayer;

                    TiledObject tiledObject = new TiledObject();

                    tiledObject.Layer = objectLayer;
                    tiledObject.Name = objectDef["name"].ToString();
                    tiledObject.ObjType = objectDef["type"].ToString();
                    tiledObject.Visible = bool.Parse(objectDef["visible"].ToString());
                    tiledObject.PosX = int.Parse(objectDef["x"].ToString());
                    tiledObject.PosY = int.Parse(objectDef["y"].ToString());
                    tiledObject.ObjWidth = int.Parse(objectDef["width"].ToString());
                    tiledObject.ObjHeight = int.Parse(objectDef["height"].ToString());
                    tiledObject.ObjProperties = (Dictionary<string, object>)objectDef["properties"];

                    objectLayer.objects.Add(tiledObject);

                    //should now have an object layer that contains all the object info

                    //TODO: Add references to the objects to the MapTile array for any tiles that they exist in?

                    //TODO: Create Colliders for the objects?

                }                
                
            }    
      
            //done loading the layer, add it to this map
            layers.Add(mapLayer);
            this.AddChild(mapLayer);
        }

        //these are only used during set-up, don't need them anymore
        tileSetFirstGIDs = null;
        tileSetFoundGIDs = null;

    }

    private void InitializeMapTiles()
    {
        tiles = new MapTile[WidthInTiles, HeightInTiles];

        //Debug.Log("Size of MapTiles is [" + WidthInTiles + "," + HeightInTiles + "]");

        int x = 0;
        int y = 0;

        for (int i = 0; i < Width; i++)
        {
            MapTile mapTile = new MapTile();

            mapTile.TileX = x;
            mapTile.TileY = HeightInTiles - y - 1; //TileY should count up from the bottom

            mapTile.Width = TileWidth;
            mapTile.Height = TileHeight;

            x++;

            //Debug.Log("New x = " + x);

            if (x % WidthInTiles == 0)
            {
                x = 0;
                y++;
            }

            //Debug.Log("Post modulo x = " + x);

            mapTile.x = mapTile.TileX * TileWidth; // -tileData.TileSet.TileWidth;
            mapTile.y = mapTile.TileY * TileHeight; // -tileData.TileSet.TileHeight + tileLayer.Height;

            //add to our map tile array, add to container
            //Debug.Log("Initializing map tile[" + mapTile.TileX + "," + mapTile.TileY + "]");
            tiles[mapTile.TileX, mapTile.TileY] = mapTile;
            this.AddChild(mapTile);

        }
    }

    private TileSet FindTileSetContainingGID(int gid)
    {
        if (tileSetFirstGIDs.Contains(gid))
        {
            return tileSets[gid];
        }

        if (tileSetFoundGIDs.ContainsKey(gid))
        {
            return tileSetFoundGIDs[gid];
        }

        //do not know what tileset contains this gid, so figure it out
        //add our gid to the list of firstGIDs, sort it, then take the previous value

        tileSetFirstGIDs.Add(gid);

        tileSetFirstGIDs.Sort();

        int gidIndex = tileSetFirstGIDs.IndexOf(gid);

        int tileSetFirstGID = tileSetFirstGIDs[gidIndex - 1];

        TileSet foundTileSet = tileSets[tileSetFirstGID];

        //found the tileset, now clean up and save this gid for later
        tileSetFirstGIDs.RemoveAt(gidIndex);

        tileSetFoundGIDs.Add(gid, foundTileSet);

        return foundTileSet;
    }

    public MapTile GetTile(int tileX, int tileY)
    {
        return tiles[tileX, tileY];
    }

    public Vector2 GetTilePosition(int tileX, int tileY)
    {
        return GetTile(tileX, tileY).GetPosition();
    }
}
