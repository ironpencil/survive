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

    IPTileMap tileMap;
    FLabel textLabel;
    IPTileLayer terrainLayer;
    IPObjectLayer terrainObjects;    
    
    

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

        //Futile.stage.Follow(player, false, false);
               
        //only handle input if player is standing still
        if (player.IsMovingToPosition)
        {
            if (player.ApproachTarget()) {
                player.IsMovingToPosition = false;
                //Check Events at new position
                CheckEvents();
            }
        }
        else
        {

            if (Input.GetKeyDown("space"))
            {
                string msgText = "This is my text that I would like to be displayed on multiple lines and on multiple labels. Hopefully this shouldn't be a problem. Can you think of any reason that it would be a problem? I sure can't.";
                FMenuScene menu = new FMenuScene("menu", msgText);
                FSceneManager.Instance.PushScene(menu);
                return; // don't run any other update code if they are opening the menu because this scene will be paused
            }

            //handle player movement input
            //if (Time.time > player.NextMoveTime)
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
                        player.NextMoveTime = Time.time + player.MoveDelayTime;
                        //player.MoveToFront();
                        //player.SetPosition(GetTilePosition(playerTile));
                        MovePlayerToTile(targetTileX, targetTileY, true);
                        //ChangePlayerTile(tileXDelta, tileYDelta);
                        //player.SetPosition(tileMap.GetTilePosition(player.TileX, player.TileY));
                        IPDebug.Log("Player position = " + player.GetPosition());
                        IPDebug.Log("Player tile = " + player.TileCoordinates);
                        //IPDebug.Log("Background position = " + background.GetPosition());
                    }
                }
            }
        }
	}

    public override void OnEnter()
	{
        IPDebug.Log("WorldScene OnEnter()");
        player = new Player("player");

        tileMap = new IPTileMap("Forest", "JSON/forestMapLarge");
        tileMap.LoadTiles();

        this.AddChild(tileMap);
        tileMap.AddChild(player);

        terrainLayer = tileMap.GetTileLayerWithProperty(IPTileMapLayerProperties.LAYER_TYPE.ToString(), IPTileMapLayerTypes.TERRAIN.ToString());

        if (terrainLayer == null)
        {
            IPDebug.Log("No Terrain Layer found!");
        }

        terrainObjects = tileMap.GetObjectLayerWithProperty(IPTileMapLayerProperties.LAYER_TYPE.ToString(), IPTileMapLayerTypes.TERRAIN.ToString());

        if (terrainObjects == null)
        {
            IPDebug.Log("No Terrain Objects found!");
        }

        //Futile.stage.Follow(player, true, true);

        IPDebug.Log("Stage position = " + Futile.stage.GetPosition());
        IPDebug.Log("Player position = " + player.GetPosition());

        MovePlayerToTile((int)terrainLayer.WidthInTiles / 2, (int)terrainLayer.HeightInTiles / 2, false);    
	}

    public override void OnExit()
	{
		
	}

    public void ChangePlayerTile(int tileXDelta, int tileYDelta)
    {
        MovePlayerToTile(player.TileX + tileXDelta, player.TileY + tileYDelta, false);
    }

    private void MessageEvent(IPTiledObject tileObject)
    {
        IPDebug.Log("Message Event: " + tileObject.GetPropertyValue(IPTileMapTileObjectProperties.TEXT.ToString()));
        string msgText = tileObject.GetPropertyValue(IPTileMapTileObjectProperties.TEXT.ToString());
        FMenuScene menu = new FMenuScene("menu", msgText);
        FSceneManager.Instance.PushScene(menu);
    }

    private bool shallowStreamTriggered = false;
    private void ShallowStreamEvent(IPTiledObject tileObject)
    {
        if (!shallowStreamTriggered)
        {
            string msgText = tileObject.GetPropertyValue(IPTileMapTileObjectProperties.TEXT.ToString());
            FMenuScene menu = new FMenuScene("menu", msgText);
            FSceneManager.Instance.PushScene(menu);
            shallowStreamTriggered = true;
        }
        IPDebug.Log("Running Event: " + tileObject.GetPropertyValue(IPTileMapTileObjectProperties.EVENT_TYPE.ToString()));
    }

    public void MovePlayerToTile(int tileX, int tileY, bool doApproach)
    {
        player.TileX = tileX;
        player.TileY = tileY;

        Vector2 newPosition = terrainLayer.GetTilePosition(player.TileX, player.TileY);
        player.IsMovingToPosition = true;
        player.TargetPosition = newPosition;

        if (doApproach)
        {
            //don't set the position manually, setting TargetPosition will be enough
            //Go.to(player, player.NextMoveTime - Time.time, new TweenConfig().floatProp("x", newPosition.x).floatProp("y", newPosition.y));
        }
        else
        {
            player.SetPosition(newPosition);
        }
    }

    private void CheckEvents()
    {
        //check for events at player's location
        //get all objects that intersect with player
        List<IPTiledObject> tileObjects = terrainObjects.GetTiledObjectsIntersectingRect(player.GetRect().CloneAndScale(0.9f));

        foreach (IPTiledObject tileObject in tileObjects)
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

    private bool CanMoveToTile(int targetTileX, int targetTileY)
    {
        //get the tile we're trying to move to
        IPTile targetTile = terrainLayer.GetTileAt(targetTileX, targetTileY);
        //MapTile targetTile = tileMap.GetTile(targetTileX, targetTileY);

        //get all objects that intersect with this tile
        List<IPTiledObject> tileObjects = terrainObjects.GetTiledObjectsIntersectingRect(targetTile.GetRect().CloneAndScale(0.9f));

        string canWalkValue = "";
        bool canWalk = false;
        bool canWalkFound = false;

        foreach (IPTiledObject tileObject in tileObjects)
        {
            //if the object says it can be walked on, set canWalk to true
            if (tileObject.PropertyExists(IPTileMapTileProperties.CAN_WALK.ToString()))
            {
                canWalkValue = tileObject.GetPropertyValue(IPTileMapTileProperties.CAN_WALK.ToString());

                IPDebug.Log("Target Tile Object = " + tileObject.Name + " | canWalkOver = " + canWalkValue);

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
            IPTileData targetTileData = targetTile.TileData;

            canWalkValue = targetTileData.GetPropertyValue(IPTileMapTileProperties.CAN_WALK.ToString());

            IPDebug.Log("Target Tile Asset = " + targetTileData.GetAssetName() + " | canWalkOver = " + canWalkValue);

            bool.TryParse(canWalkValue, out canWalk);
        }

        return canWalk;
    }

    public override void Redraw(bool shouldForceDirty, bool shouldUpdateDepth)
    {
        Futile.stage.SetPosition(-player.GetPosition());
        base.Redraw(shouldForceDirty, shouldUpdateDepth);
    }
}