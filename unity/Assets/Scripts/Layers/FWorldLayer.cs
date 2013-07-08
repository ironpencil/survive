/*
- THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
- IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
- FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
- AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
- LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
- OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
- THE SOFTWARE.
*/

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FWorldLayer : FLayer
{
    Player player;

    TileMapData tileMap;
    FLabel textLabel;
    TileLayer terrainLayer;
    ObjectLayer terrainObjects;

    private float moveDelayTime = 0.1f;
    private float nextMoveTime = 0.0f;

    public FWorldLayer(FScene parent) : base(parent) { }

    public override void HandleMultiTouch(FTouch[] touches)
	{

	}

    public override void OnUpdate()
	{
        if (this.Parent.Paused)
        {
            return;
        }

        Futile.stage.Follow(player, false, false);

        if (Input.GetKeyDown("space"))
        {
            FMenuScene menu = new FMenuScene("menu");
            FSceneManager.Instance.PushScene(menu);
            return; // don't run any other update code if they are opening the menu because this scene will be paused
        }

        //player Movement
        if (Time.time > nextMoveTime)
        {
            bool playerMoved = false;

            int tileYDelta = 0;
            int tileXDelta = 0;

            if (Input.GetKey("up"))
            {
                if (player.TileY < tileMap.HeightInTiles - 1)
                {
                    tileYDelta += 1;
                    playerMoved = true;
                }
            }
            else if (Input.GetKey("down"))
            {
                if (player.TileY > 0)
                {
                    tileYDelta -= 1;
                    playerMoved = true;
                }
            }
            else if (Input.GetKey("left"))
            {
                if (player.TileX > 0)
                {
                    tileXDelta -= 1;
                    playerMoved = true;
                }
            }
            else if (Input.GetKey("right"))
            {
                if (player.TileX < tileMap.WidthInTiles - 1)
                {
                    tileXDelta += 1;
                    playerMoved = true;
                }
            }

            if (playerMoved)
            {

                int targetTileX = player.TileX + tileXDelta;
                int targetTileY = player.TileY + tileYDelta;

                //bool canMoveToTile = 

                bool canWalk = CanMoveToTile(targetTileX, targetTileY);

                if (canWalk)
                {
                    //player.MoveToFront();
                    //player.SetPosition(GetTilePosition(playerTile));
                    MovePlayerToTile(targetTileX, targetTileY);
                    //ChangePlayerTile(tileXDelta, tileYDelta);
                    //player.SetPosition(tileMap.GetTilePosition(player.TileX, player.TileY));
                    Debug.Log("Player position = " + player.GetPosition());
                    Debug.Log("Player tile = " + player.TileCoordinates);

                    nextMoveTime = Time.time + moveDelayTime;
                    //Debug.Log("Background position = " + background.GetPosition());
                }
            }
        }
	}

    public override void OnEnter()
	{
        Debug.Log("WorldScene OnEnter()");
        player = new Player("player");

        tileMap = new TileMapData("Forest", "JSON/forestMapLarge");
        tileMap.LoadTiles();

        this.AddChild(tileMap);
        tileMap.AddChild(player);

        terrainLayer = tileMap.GetTileLayerWithProperty(IPTileMapLayerProperties.LAYER_TYPE.ToString(), IPTileMapLayerTypes.TERRAIN.ToString());

        if (terrainLayer == null)
        {
            Debug.Log("No Terrain Layer found!");
        }

        terrainObjects = tileMap.GetObjectLayerWithProperty(IPTileMapLayerProperties.LAYER_TYPE.ToString(), IPTileMapLayerTypes.TERRAIN.ToString());

        if (terrainObjects == null)
        {
            Debug.Log("No Terrain Objects found!");
        }

        Futile.stage.Follow(player, true, true);

        Debug.Log("Stage position = " + Futile.stage.GetPosition());
        Debug.Log("Player position = " + player.GetPosition());

        MovePlayerToTile((int)terrainLayer.WidthInTiles / 2, (int)terrainLayer.HeightInTiles / 2);    
	}

    public override void OnExit()
	{
		
	}

    public void ChangePlayerTile(int tileXDelta, int tileYDelta)
    {
        MovePlayerToTile(player.TileX + tileXDelta, player.TileY + tileYDelta);
    }

    public void MovePlayerToTile(int tileX, int tileY)
    {
        player.TileX = tileX;
        player.TileY = tileY;
        player.SetPosition(terrainLayer.GetTilePosition(player.TileX, player.TileY));

        //check for events at player's location
        //get all objects that intersect with player
        List<TiledObject> tileObjects = terrainObjects.GetTiledObjectsIntersectingRect(player.GetRect().CloneAndScale(0.9f));

        foreach (TiledObject tileObject in tileObjects)
        {
            if (tileObject.ObjType.Equals(IPTileMapTileObjectTypes.EVENT.ToString()))
            {
                //this is an event object, run the event
                string eventType = tileObject.GetPropertyValue(IPTileMapTileObjectProperties.EVENT_TYPE.ToString());

                switch (eventType)
                {
                    case "MESSAGE": MessageEvent(tileObject);
                        break;
                    case "SHALLOW_STREAM_NORTH":
                    case "SHALLOW_STREAM_SOUTH": ShallowStreamEvent(tileObject);
                        break;
                    default:
                        break;
                }
            }
        }
    }

    private void MessageEvent(TiledObject tileObject)
    {
        Debug.Log("Message Event: " + tileObject.GetPropertyValue(IPTileMapTileObjectProperties.TEXT.ToString()));
    }

    private void ShallowStreamEvent(TiledObject tileObject)
    {
        Debug.Log("Running Event: " + tileObject.GetPropertyValue(IPTileMapTileObjectProperties.EVENT_TYPE.ToString()));
    }

    public void MovePlayerToTile(float tileX, float tileY)
    {
        MovePlayerToTile((int)tileX, (int)tileY);
    }

    //public void MovePlayerToTile(MapTile tile)
    public void MovePlayerToTile(LayerTile tile)
    {
        player.TileX = tile.TileData.TileX;
        player.TileY = tile.TileData.TileY;
        player.SetPosition(tile.GetPosition());
    }

    private bool CanMoveToTile(int targetTileX, int targetTileY)
    {
        //get the tile we're trying to move to
        LayerTile targetTile = terrainLayer.GetTileAt(targetTileX, targetTileY);
        //MapTile targetTile = tileMap.GetTile(targetTileX, targetTileY);

        //get all objects that intersect with this tile
        List<TiledObject> tileObjects = terrainObjects.GetTiledObjectsIntersectingRect(targetTile.GetRect().CloneAndScale(0.9f));

        string canWalkValue = "";
        bool canWalk = false;
        bool canWalkFound = false;

        foreach (TiledObject tileObject in tileObjects)
        {
            //if the object says it can be walked on, set canWalk to true
            if (tileObject.PropertyExists(IPTileMapTileProperties.CAN_WALK.ToString()))
            {
                canWalkValue = tileObject.GetPropertyValue(IPTileMapTileProperties.CAN_WALK.ToString());

                Debug.Log("Target Tile Object = " + tileObject.Name + " | canWalkOver = " + canWalkValue);

                //just use first object we find that contains a valid property value then break out
                if (bool.TryParse(canWalkValue, out canWalk))
                {
                    canWalkFound = true;
                    break;
                }
            }
        }

        //only check the tile if we didn't find any objects with walkable definitions
        if (!canWalkFound)
        {
            //check the canWalk value of the tile
            LayerTileData targetTileData = targetTile.TileData;

            canWalkValue = targetTileData.GetPropertyValue(IPTileMapTileProperties.CAN_WALK.ToString());

            Debug.Log("Target Tile Asset = " + targetTileData.GetAssetName() + " | canWalkOver = " + canWalkValue);

            bool.TryParse(canWalkValue, out canWalk);
        }

        return canWalk;
    }
}