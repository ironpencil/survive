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
    FTilemap tileMap;
    FLabel textLabel;
    TileMapHelper mapHelper;

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
        }

        //player Movement
        if (Time.time > nextMoveTime)
        {
            bool playerMoved = false;

            if (Input.GetKey("up"))
            {
                if (player.TileY < tileMap.heightInTiles - 1)
                {
                    player.TileY += 1;
                    playerMoved = true;
                }
            } else if (Input.GetKey("down"))
            {
                if (player.TileY > 0)
                {
                    player.TileY -= 1;
                    playerMoved = true;
                }
            } else if (Input.GetKey("left"))
            {
                if (player.TileX > 0)
                {
                    player.TileX -= 1;
                    playerMoved = true;
                }
            } else if (Input.GetKey("right"))
            {
                if (player.TileX < tileMap.widthInTiles - 1)
                {
                    player.TileX += 1;
                    playerMoved = true;
                }
            }

            if (playerMoved)
            {
                player.MoveToFront();
                //player.SetPosition(GetTilePosition(playerTile));
                player.SetPosition(mapHelper.GetTilePosition(player.TileCoordinates));
                Debug.Log("Player position = " + player.GetPosition());
                Debug.Log("Player tile = " + player.TileCoordinates);

                nextMoveTime = Time.time + moveDelayTime;
                //Debug.Log("Background position = " + background.GetPosition());
            }
        }
		
	}

    public override void OnEnter()
	{
        Debug.Log("WorldScene OnEnter()");
        player = new Player("player");


        // Add tilemap 
        tmxMap = new FTmxMap();
        tmxMap.clipNode = player;
        tmxMap.LoadTMX("CSVs/forestMapLarge"); // load tmx text file (within Resources/CSVs folder)
        tileMap = null;

        //bool tileMapFound = tmxMap._tileMaps.TryGetValue(tmxMap._layerNames[0], out tileMap);

        //Debug.Log("tileMapFound=" + tileMapFound);

        //tmx1.x = 0;
        //tmx1.y = 0;

        foreach (TmxLayer tmxLayer in tmxMap._tmxLayers)
        {
            if (tmxLayer.LayerType == TmxLayerType.TILE_LAYER)
            {
                tileMap = (FTilemap)tmxLayer.Layer;
                mapHelper = new TileMapHelper(tileMap);
                break;
            }
        }

        Debug.Log("TileMap Size (x, y):" + tileMap.width + "," + tileMap.height);
        Debug.Log("TileMap Tiles (x, y):" + tileMap.widthInTiles + "," + tileMap.heightInTiles);

        this.AddChild(tmxMap);

        tmxMap.AddChild(player);

        //this.AddChild(player);

        Futile.stage.Follow(player, true, true);

        //maxBounds = GetSizeInTiles(new Vector2(tileMap.width, tileMap.height));
        //maxBounds.x--;
        //maxBounds.y--;

        player.anchorX = 0;
        player.anchorY = 0;

        Debug.Log("Stage position = " + Futile.stage.GetPosition());
        Debug.Log("Player position = " + player.GetPosition());

        player.TileCoordinates = mapHelper.GetCenterTile();
        player.SetPosition(mapHelper.GetTilePosition(player.TileCoordinates));        
	}

    public override void OnExit()
	{

	}



}

