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
using System.Linq;

public class FWorldLayer : FLayer
{
    Mob player;

    IPTileMap tileMap;
    FLabel textLabel;
    IPTileLayer terrainLayer;
    IPObjectLayer terrainObjects;
    IPTileLayer elevatedLayer;

    string mapAsset = "JSON/gameMap";

    FSelectionDisplayScene selectionScene = null;
    bool inSelectionDialog = false;

    private bool shallowStreamTriggered = false;
    private bool gameWon = false;

    long turnNumber = 0;
    bool gameOver = false;
    bool canGameOver = true;

    private int turnsToLoseOneEnergy = 10; //10
    private int turnsToLoseOneWater = 45; //60

    private int viewDistanceX = 17;
    private int viewDistanceY = 12;

    private int addlTileLoadRange = 10;
    private bool loadAddlTiles = false;

    private Queue eventQueue = new Queue();

    private int nextRandomEncounterIndex = 0;
    private int currentRandomEncounterInterval = 0;
    private int encounterBagFills = 0;
    private int randomEncounterChance = 15;
    private List<EncounterEvent> randomEncounterBag = new List<EncounterEvent>();
    bool fireRandomEncounters = true;
    
    public FWorldLayer(FScene parent) : base(parent) { }

    public override void HandleMultiTouch(FTouch[] touches)
	{

	}
    
    public override void OnUpdate()
	{

        //tileMap.LoadMoreTiles(5);

        if (elevatedLayer != null) { elevatedLayer.MoveToFront(); }

        if (SurviveGame.ALLOW_DEBUG)
        {
            if (Input.GetKeyDown(KeyCode.N))
            {
                GameVars.Instance.PLAYER_FULL_ENERGY = 10000;
                GameVars.Instance.PLAYER_FULL_WATER = 1000;
                player.Energy = GameVars.Instance.PLAYER_FULL_ENERGY;
                player.Water = GameVars.Instance.PLAYER_FULL_WATER;
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                IPDebug.ForceAllowed = !IPDebug.ForceAllowed;
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                DoGameWon();
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                DoGameOver();
            }

            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                player.speed = new Vector2(1600, 1600);
            }

            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                player.speed = new Vector2(256, 256);
            }

            if (Input.GetKeyDown("0"))
            {
                fireRandomEncounters = false;
                canGameOver = false;
            }
            else if (Input.GetKeyDown("1"))
            {
                fireRandomEncounters = true;
                canGameOver = true;
            }

            if (Input.GetKeyDown(KeyCode.P))
            {

                string eventName = "WILD_PLANT"; //generate random event from available events and tile type
                EncounterEvent randomEvent = EncounterEvent.CreateRandomEvent(eventName);
                eventQueue.Enqueue(randomEvent);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                eventQueue.Enqueue(PullRandomEncounter());
            }
        }

        CheckGameOver();

        if (this.Parent.Paused)
        {
            //tileMap.LoadMoreTiles(5);
            //tileMap.LoadMoreTiles(16);
            //if (addlTileLoadRange <= maxAddlTileLoadRange)
            //{
            //    tileMap.LoadTilesInRange(new Vector2(player.TileX, player.TileY), new Vector2(viewDistanceX + addlTileLoadRange, viewDistanceY + addlTileLoadRange));
            //    addlTileLoadRange++;
            //}
            //if (Time.time > nextLoadTileTime)
            //{
            //    //allowed to load more tiles
            //    tileMap.LoadTilesInRange(new Vector2(player.TileX, player.TileY), new Vector2(viewDistanceX + addlTileLoadRange, viewDistanceY + addlTileLoadRange));
            //    addlTileLoadRange++;
            //    nextLoadTileTime = Time.time + loadTileInterval;
            //}

            //only load additional tiles once per "pause"
            if (loadAddlTiles)
            {
                //tileMap.LoadTilesInRange(new Vector2(player.TileX, player.TileY), new Vector2(viewDistanceX + addlTileLoadRange, viewDistanceY + addlTileLoadRange));
                loadAddlTiles = false;
            }
            return;
        }

        //when the scene is unpaused, loadAddlTiles is set to true so that we will load more tiles on the next "pause"
        loadAddlTiles = true;
        //addlTileLoadRange = 1;


        if (this.gameWon)
        {
            FSceneManager.Instance.SetScene(new FGameWonScene("GameWon"));
            return;
        }

        if (this.gameOver)
        {
            FSceneManager.Instance.SetScene(new FGameOverScene("GameOver"));
            return;
        }

        if (RunEvents())
        {
            //if there are any events queued, keep running them until they are gone
            return;
        }
               
        //only handle input if player is standing still
        if (player.IsMovingToPosition)
        {


            if (player.ApproachTarget())
            {
                player.IsMovingToPosition = false;
                //Check Events at new position
                TakeTurn();
                CheckForEvents();
                RunEvents();

                //tileMap.LoadTilesInRange(new Vector2(player.TileX, player.TileY), new Vector2(viewDistanceX + addlTileLoadRange, viewDistanceY + addlTileLoadRange));
                tileMap.SetVisibleRange(new Vector2(player.TileX, player.TileY), new Vector2(viewDistanceX, viewDistanceY));

            }
            else
            {
                //tileMap.LoadTilesInRange(new Vector2(player.TileX, player.TileY), new Vector2(viewDistanceX + addlTileLoadRange, viewDistanceY + addlTileLoadRange));
            }
        }
        else
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

    public override void OnEnter()
	{
        IPDebug.ForceAllowed = false;

        IPDebug.Log("WorldScene OnEnter()");
        player = new Mob("player");
        GameVars.Instance.Player = player;
        player.SetStat(MobStats.ENERGY, 100);
        player.SetStat(MobStats.HP, 10);

        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.ATM_CARD));
        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.BUG_SPRAY));
        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.COMPASS));
        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.FIRST_AID_KIT));
        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.HONEY));
        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.LASER_POINTER));
        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.MARSHMALLOWS));
        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.RAW_MEAT));
        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.SALT));

        currentRandomEncounterInterval = GameVars.Instance.RANDOM_ENCOUNTER_INTERVAL;
        FillEncounterBag();

        nextRandomEncounterIndex = UnityEngine.Random.Range(0, randomEncounterBag.Count);

        tileMap = new IPTileMap("Game", mapAsset, false);
        tileMap.LoadTileDataFile();

        GameVars.Instance.TileHelper = new TileMapHelper(tileMap);
        GameVars.Instance.ResetGame();

        //this.AddChild(tileMap);
        //tileMap.AddChild(player);

        //tileMap.SetAllVisible();

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

        elevatedLayer = tileMap.GetTileLayerWithProperty(IPTileMapLayerProperties.LAYER_TYPE.ToString(), IPTileMapLayerTypes.ELEVATED.ToString());

        if (elevatedLayer == null)
        {
            IPDebug.Log("No Elevated Layer found!");
        }

        //Futile.stage.Follow(player, true, true);

        IPDebug.Log("Stage position = " + Futile.stage.GetPosition());
        IPDebug.Log("Player position = " + player.GetPosition());

        MoveToStartPosition();

        //float loadTimeStart = Time.time;

        //IPDebug.ForceAllowed = false;
        //tileMap.LoadAllTiles();

        //float timeTaken = Time.time - loadTimeStart;
        //IPDebug.ForceAllowed = true;
        //IPDebug.ForceLog("Time to load all tiles: " + timeTaken);
        //IPDebug.ForceAllowed = false;

        //tileMap.LoadAllTileData();
        
        this.AddChild(tileMap);
        tileMap.AddChild(player);
	}

    private void MoveToStartPosition()
    {
        List<IPTiledObject> allObjects = terrainObjects.Objects;

        int startX = terrainLayer.WidthInTiles / 2;
        int startY = terrainLayer.HeightInTiles / 2;

        foreach (IPTiledObject tileObject in allObjects)
        {
            if (tileObject.GetPropertyValue("START_POSITION").Equals("true"))
            {
                startX = (int)tileObject.x / tileMap.TileWidth;
                startY = (int)tileObject.y / tileMap.TileHeight;               
                break;
            }
        }

        MovePlayerToTile(startX, startY, false);
        //MovePlayerToTile(30, 290, false); //this is by the cabin

        //tileMap.LoadTilesInRange(new Vector2(player.TileX, player.TileY), new Vector2(viewDistanceX * 2, viewDistanceY * 2));
        tileMap.SetVisibleRange(new Vector2(player.TileX, player.TileY), new Vector2(viewDistanceX, viewDistanceY));
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
        FTextDisplayScene menu = new FTextDisplayScene("menu", msgText);
        FSceneManager.Instance.PushScene(menu);
    }

    
    private void ShallowStreamEvent(IPTiledObject tileObject)
    {
        if (!shallowStreamTriggered)
        {
            string msgText = tileObject.GetPropertyValue(IPTileMapTileObjectProperties.TEXT.ToString());
            FTextDisplayScene menu = new FTextDisplayScene("menu", msgText);
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

    private void CheckForEvents()
    {
        if (!fireRandomEncounters) { return; }
        if (this.gameOver) { return; }
        //check for events at player's location
        //get all objects that intersect with player
        List<IPTiledObject> tileObjects = terrainObjects.GetTiledObjectsIntersectingRect(player.GetStandingRect().CloneAndScale(0.9f));

        foreach (IPTiledObject tileObject in tileObjects)
        {
            if (tileObject.ObjType.Equals(IPTileMapTileObjectTypes.EVENT.ToString()))
            {
                //this is an event object, run the event
                string eventName = tileObject.GetPropertyValue(IPTileMapTileObjectProperties.EVENT_TYPE.ToString());

                EncounterEvent objectEvent = EncounterEvent.CreateObjectEvent(eventName, tileObject);
                eventQueue.Enqueue(objectEvent);
            }
        }

        IPTile currentTile = terrainLayer.GetTileAt(player.TileX, player.TileY);
        IPTileData currentTileData = currentTile.TileData;

        if (currentTileData.PropertyExists("EVENT"))
        {
            string eventName = currentTileData.GetPropertyValue("EVENT");

            EncounterEvent tileEvent = EncounterEvent.CreateTileEvent(eventName, currentTile);
            eventQueue.Enqueue(tileEvent);
        }

        //only do a random event if there's not already an event on this tile
        if (eventQueue.Count == 0)
        {
            //check for random event
            bool randomEventHappens = false;
            if (turnNumber % currentRandomEncounterInterval == 0)
            {
                int randomRoll = UnityEngine.Random.Range(0, 100);
                randomEventHappens = (randomRoll < randomEncounterChance);

                IPDebug.ForceLog("Turn: " + turnNumber + " | Encounter Roll: " + randomRoll + " | Encounter: " + randomEventHappens);

            }

            if (randomEventHappens)
            {
                eventQueue.Enqueue(PullRandomEncounter());
            }
        }
    }    

    private bool RunEvents()
    {
        if (this.gameOver) { return false; }

        if (eventQueue.Count > 0)
        {
            IPDebug.Log("Event Queue Count : " + eventQueue.Count);
            EncounterEvent gameEvent = (EncounterEvent) eventQueue.Dequeue();
            IPDebug.Log("Running Event : " + gameEvent.Name);
            switch (gameEvent.Source)
            {
                case EncounterSource.TILE:
                    ExecuteTileEvent(gameEvent.Name, gameEvent.EventTile);
                    break;
                case EncounterSource.OBJECT:
                    ExecuteObjectEvent(gameEvent.Name, gameEvent.EventObject);
                    break;
                case EncounterSource.RANDOM:
                    ExecuteRandomEvent(gameEvent.Name);
                    break;
                default:
                    break;
            }

            //return true if we started an event
            return true;
        }

        return false;
    }

    private void TakeTurn()
    {        
   
        if (turnNumber > 0)
        {
            IPDebug.Log("Taking Turn: " + turnNumber);
            if (turnNumber % turnsToLoseOneEnergy == 0)
            {
                player.Energy--;
            }

            if (player.Water > 0)
            {
                if (turnNumber % turnsToLoseOneWater == 0)
                {
                    player.Water--;
                }
            }
            else
            {
                //lose 1 energy a turn when you don't have water in addition to the normal energy loss
                player.Energy--;
            }
        }

        turnNumber++;

        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (!this.gameOver && canGameOver && player.Energy <= 0)
        {
            player.Energy = 0;
            DoGameOver();
        }
    }

    private void DoGameOver()
    {
        FTextDisplayScene gameOverMessage = new FTextDisplayScene("GameOver", "You ran out of energy!");
        FSceneManager.Instance.PushScene(gameOverMessage);
        this.gameOver = true;
        FSoundManager.PlayMusic("game_over1", GameVars.Instance.MUSIC_VOLUME, false);
        FSoundManager.CurrentMusicShouldLoop(false);
    }

    private void WonGameEvent(IPTiledObject tileObject)
    {
        DoGameWon();
    }

    private void DoGameWon()
    {
        FTextDisplayScene wonGameMessage = new FTextDisplayScene("You Win", "You made it back to the Ranger's Office safely!");
        FSceneManager.Instance.PushScene(wonGameMessage);
        this.gameWon = true;
        FSoundManager.PlayMusic("game_won1", GameVars.Instance.MUSIC_VOLUME, false);
        FSoundManager.CurrentMusicShouldLoop(false);
    }



    private void ExecuteTileEvent(string eventName, IPTile eventTile)
    {
        IPDebug.Log("Executing Tile Event: " + eventName);
        Vector2 tileVector = new Vector2(eventTile.TileData.TileX, eventTile.TileData.TileY);
        FEventHandlerLayer eventHandler = new FEventHandlerLayer(this.Parent, eventName, eventTile, tileVector);

        this.Parent.AddChild(eventHandler);
    }    

    private void ExecuteObjectEvent(string eventName, IPTiledObject tileObject)
    {
        switch (eventName)
        {
            case "MESSAGE": MessageEvent(tileObject);
                break;
            case "SHALLOW_STREAM_NORTH":
            case "SHALLOW_STREAM_SOUTH": ShallowStreamEvent(tileObject);
                break;
            case "WON_GAME": WonGameEvent(tileObject);
                break;
            default:
                break;
        }
    }

    private void ExecuteRandomEvent(string eventName)
    {        
        FEventHandlerLayer eventHandler = new FEventHandlerLayer(this.Parent, eventName);
        this.Parent.AddChild(eventHandler);
    }

    private void FillEncounterBag()
    {
        randomEncounterBag.Clear();
        if (encounterBagFills == 0)
        {
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("BEAR"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("WOLF"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("SNAKE"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("COUGAR"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("FIND_FOOD_QUIZ"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("BATHROOM_QUIZ"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("FISHING_QUIZ"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("QUICKSAND_QUIZ"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("MAKE_FIRE_QUIZ"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("MAKE_CAMP_QUIZ"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("MUDSLIDE_QUIZ"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("BEAR_TRAP_QUIZ"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("NAVIGATION_QUIZ"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("LIGHTNING_QUIZ"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("TICK"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("BEES"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("RACCOON"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("ANTS"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("SPIDER"));

            int listSize = randomEncounterBag.Count;
            //add a wild plant encounter for each other encounter - wild plants are common
            for (int i = 0; i < listSize; i++)
            {
                randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("WILD_PLANT"));
            }
            //do initial fill

        }
        else
        {
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("BEAR"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("WOLF"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("SNAKE"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("COUGAR"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("TICK"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("BEES"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("RACCOON"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("ANTS"));
            randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("SPIDER"));

            int listSize = randomEncounterBag.Count;
            //add a wild plant encounter for each other encounter - wild plants are common
            for (int i = 0; i < listSize; i++)
            {
                randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("WILD_PLANT"));
            }
            //do refill

        }

        encounterBagFills++;
        currentRandomEncounterInterval = GameVars.Instance.RANDOM_ENCOUNTER_INTERVAL * encounterBagFills;
    }

    private EncounterEvent PullRandomEncounter()
    {
        EncounterEvent pulledEncounter = randomEncounterBag[nextRandomEncounterIndex];
        randomEncounterBag.RemoveAt(nextRandomEncounterIndex);

        if (randomEncounterBag.Count == 0)
        {
            FillEncounterBag();
        }

        nextRandomEncounterIndex = UnityEngine.Random.Range(0, randomEncounterBag.Count);

        return pulledEncounter;
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