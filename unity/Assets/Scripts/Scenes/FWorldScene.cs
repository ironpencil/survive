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

public class FWorldScene : FScene
{

    Player player;
    FTmxMap tmxMap;
    //FTilemap tileMap;
    TileMapData tileMap;
    FLabel textLabel;
    TileLayer terrainLayer;
    ObjectLayer terrainObjects;

    private float moveDelayTime = 0.1f;
    private float nextMoveTime = 0.0f;

    //Vector2 maxBounds;
    public FWorldScene(string _name = "Default")
        : base(_name)
	{
		mName = _name;
	}
	
	public override void OnUpdate ()
	{
        if (this.Paused)
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
            } else if (Input.GetKey("down"))
            {
                if (player.TileY > 0)
                {
                    tileYDelta -= 1;
                    playerMoved = true;
                }
            } else if (Input.GetKey("left"))
            {
                if (player.TileX > 0)
                {
                    tileXDelta -= 1;
                    playerMoved = true;
                }
            } else if (Input.GetKey("right"))
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

        terrainLayer = tileMap.GetTileLayerWithProperty("layer_type", "terrain");

        if (terrainLayer == null)
        {
            Debug.Log("No Terrain Layer found!");
        }

        terrainObjects = tileMap.GetObjectLayerWithProperty("layer_type", "terrain");

        if (terrainObjects == null)
        {
            Debug.Log("No Terrain Objects found!");
        }

        Futile.stage.Follow(player, true, true);

        Debug.Log("Stage position = " + Futile.stage.GetPosition());
        Debug.Log("Player position = " + player.GetPosition());

        MovePlayerToTile((int)terrainLayer.WidthInTiles/2, (int)terrainLayer.HeightInTiles/2);    
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
            canWalkValue = tileObject.GetPropertyValue("canWalkOver");

            Debug.Log("Target Tile Object = " + tileObject.Name + " | canWalkOver = " + canWalkValue);
            //just use first object we find
            if (canWalkValue.Length > 0)
            {
                canWalk = bool.Parse(canWalkValue);
                canWalkFound = true;
                break;
            }
        }

        //only check the tile if we didn't find any objects with walkable definitions
        if (!canWalkFound)
        {
            //check the canWalk value of the tile
            LayerTileData targetTileData = targetTile.TileData;

            canWalkValue = targetTileData.GetPropertyValue("canWalkOver");

            Debug.Log("Target Tile Asset = " + targetTileData.GetAssetName() + " | canWalkOver = " + canWalkValue);

            canWalk = (canWalkValue.Length > 0 ? bool.Parse(canWalkValue) : false);
        }

        return canWalk;
    }
}

public static class IPRectExtensions
{
    public static Rect CloneAndScale(this Rect rect, float scale)
    {
        float left = rect.xMin;
        float bottom = rect.yMin;
        float width = rect.width;
        float height = rect.height;

        //we now have the original rect, check if we need to scale it
        if (scale != 1.0f)
        {
            //scale the width and height
            float newWidthMag = rect.width * scale;
            float newHeightMag = rect.height * scale;

            //find the difference
            float widthDelta = rect.width - newWidthMag;
            float heightDelta = rect.height - newHeightMag;

            width = newWidthMag;
            height = newHeightMag;

            //move the left and top to accomodate the new size (grow/shrink around center)
            left += widthDelta / 2;
            bottom += heightDelta / 2;
        }

        //return a rect with the calculated top, left, and sizes
        return new Rect(left, bottom, width, height);
    }
}