using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class TileMapData
{
    public string Name { get; set; }

    private Dictionary<int, TileSet> tileSets;
    private List<int> tileSetFirstGIDs;
    private Dictionary<int, TileSet> tileSetFoundGIDs;

    public int Width { get; set; }
    public int Height { get; set; }

    public int TileWidth { get; set; }
    public int TileHeight { get; set; }



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
        tileSetFirstGIDs = new List<int>();
        tileSetFoundGIDs = new Dictionary<int, TileSet>();

        TextAsset map = (TextAsset)Resources.Load(mapFile, typeof(TextAsset));

        if (!map)
        {
            Debug.Log("Map file '" + mapFile + "' not found!");
            Debug.Break();
        }

        mapData = map.text.dictionaryFromJson();

        Width = int.Parse(mapData["width"].ToString());
        Height = int.Parse(mapData["height"].ToString());
        TileWidth = int.Parse(mapData["tilewidth"].ToString());
        TileHeight = int.Parse(mapData["tileheight"].ToString());

        List<object> tileSetsData = (List<object>)mapData["tilesets"];

        foreach (Dictionary<string, object> tileSetData  in tileSetsData)
        {
            TileSet tileSet = new TileSet();

            tileSet.FirstGID = int.Parse(tileSetData["firstgid"].ToString());
            tileSet.Image = tileSetData["image"].ToString();
            tileSet.Name = tileSetData["name"].ToString();
            tileSet.TileProperties = (Dictionary<string, object>)tileSetData["tileproperties"];

            tileSets.Add(tileSet.FirstGID, tileSet);
            tileSetFirstGIDs.Add(tileSet.FirstGID);
        }

        tileSetFirstGIDs.Sort();

        List<object> mapLayers = (List<object>)mapData["layers"];

        foreach (Dictionary<string, object> layerData in mapLayers)
        {

            MapLayer mapLayer = new MapLayer();

            mapLayer.Name = layerData["name"].ToString();
            mapLayer.Visible = bool.Parse(layerData["visible"].ToString());
            mapLayer.Width = int.Parse(layerData["width"].ToString());
            mapLayer.Height = int.Parse(layerData["height"].ToString());
            mapLayer.Opacity = int.Parse(layerData["opacity"].ToString());
            mapLayer.LayerType = layerData["type"].ToString();

            if (mapLayer.LayerType.Equals("tilelayer"))
            {
                //load the tile layer
                List<object> tileGIDs = (List<object>) layerData["data"];

                int x = 0;
                int y = 0;

                for (int i = 0; i < tileGIDs.Count; i++)
                {
                    TileLayer tileLayer = (TileLayer)mapLayer;

                    TileData tile = new TileData();

                    tile.Layer = tileLayer;                                        
                    tile.GID = int.Parse(tileGIDs[i].ToString());
                    tile.TileSet = FindTileSetContainingGID(tile.GID);

                    tile.TileX = x;
                    tile.TileY = y;

                    x++;

                    if (x % mapLayer.Width == 0)
                    {
                        x = 0;
                        y++;
                    }

                    tileLayer.tiles.Add(tile);
                }

                //should now have a tile layer that contains all the layer info
                //plus a list of tiles, each referencing the TileSet that its tile comes from
                
            }
            else if (mapLayer.LayerType.Equals("objectgroup"))
            {
                //load the object group
                List<object> objectDefs = (List<object>)layerData["objects"];

                foreach (Dictionary<string, object> objectDef in objectDefs)
                {
                    ObjectLayer objectLayer = (ObjectLayer)mapLayer;

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

                }
                
            }          
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
}
