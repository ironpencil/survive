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
    FTilemap tileMap;
    FLabel textLabel;

    Vector2 maxBounds;
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
            if (this.Name != "menu")
            {
                FWorldScene newScene = new FWorldScene("menu");
                FSceneManager.Instance.PushScene(newScene);
            }
            else
            {
                FSceneManager.Instance.PopScene();
            }
        }

        bool playerMoved = false;

        if (Input.GetKeyDown("up"))
        {
            if (player.TileY < maxBounds.y)
            {
                player.TileY += 1;
                playerMoved = true;
            }
        }
        if (Input.GetKeyDown("down"))
        {
            if (player.TileY > 0)
            {
                player.TileY -= 1;
                playerMoved = true;
            }
        }
        if (Input.GetKeyDown("left"))
        {
            if (player.TileX > 0)
            {
                player.TileX -= 1;
                playerMoved = true;
            }
        }
        if (Input.GetKeyDown("right"))
        {
            if (player.TileX < maxBounds.x)
            {
                player.TileX += 1;
                playerMoved = true;
            }
        }

        if (playerMoved)
        {
            player.MoveToFront();
            //player.SetPosition(GetTilePosition(playerTile));
            player.SetPosition(GetTilePosition(player.TileCoordinates)); 
            Debug.Log("Player position = " + player.GetPosition());
            Debug.Log("Player tile = " + player.TileCoordinates);
            //Debug.Log("Background position = " + background.GetPosition());
        }
		
	}

    public override void OnEnter()
	{
        Debug.Log("WorldScene OnEnter()");
        player = new Player("player");


        // Add tilemap 
        //FTmxMap tmx1 = new FTmxMap();
        //tmx1.clipNode = player;
        //tmx1.LoadTMX("CSVs/forestMapLarge"); // load tmx text file (within Resources/CSVs folder)
        //tileMap = null;

        //bool tileMapFound = tmx1._tileMaps.TryGetValue(tmx1._layerNames[0], out tileMap);

        //Debug.Log("tileMapFound=" + tileMapFound);

        //tmx1.x = 0;
        //tmx1.y = 0;

        //Debug.Log("TileMap Size (x, y):" + tileMap.width + "," + tileMap.height);
        //Debug.Log("TileMap Tiles (x, y):" + tileMap.widthInTiles + "," + tileMap.heightInTiles);

        //Futile.stage.AddChild(tmx1);

        //tmx1.AddChild(player);

        this.AddChild(player);

        Futile.stage.Follow(player, true, true);

        //maxBounds = GetSizeInTiles(new Vector2(tileMap.width, tileMap.height));
        maxBounds = new Vector2(10, 10);
        maxBounds.x--;
        maxBounds.y--;

        player.anchorX = 0;
        player.anchorY = 0;

        Debug.Log("Stage position = " + Futile.stage.GetPosition());
        Debug.Log("Player position = " + player.GetPosition());

        player.TileCoordinates = GetCenterTile();
        player.SetPosition(GetTilePosition(player.TileCoordinates));        
	}

    public override void OnExit()
	{

	}


    #region TileHelpers

    const int TILE_WIDTH = 32;
    const int TILE_HEIGHT = 32;

    public Vector2 GetTilePosition(Vector2 tile)
    {
        Vector2 desiredTilePosition = Vector2.zero;

        desiredTilePosition.x += (tile.x * TILE_WIDTH);
        //desiredTilePosition.y += (tile.y * TILE_HEIGHT) - tileMap.height;
        desiredTilePosition.y += (tile.y * TILE_HEIGHT);

        //desiredTilePosition.x += Futile.screen.halfWidth + (background.width / 2);
        //desiredTilePosition.y -= Futile.screen.halfHeight + (background.height / 2);

        //desiredTilePosition -= GetTilePosition(tile);

        return desiredTilePosition;
    }

    public Vector2 GetSizeInTiles(Vector2 sizeInPixels)
    {
        int x = (int)(sizeInPixels.x / TILE_WIDTH);
        int y = (int)(sizeInPixels.y / TILE_HEIGHT);

        return new Vector2(x, y);
    }

    public Vector2 GetCenterTile()
    {
        Vector2 centerTile = new Vector2();

        centerTile.x = (int)(maxBounds.x / 2);
        centerTile.y = (int)(maxBounds.y / 2);

        return centerTile;
    }

    #endregion
}
