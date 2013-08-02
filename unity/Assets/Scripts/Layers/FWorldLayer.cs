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

    private int randomEncounterSteps = 0;
    private int baseRandomEncounterInterval = 14;
    private int maxRandomEncounterVariance = 16;
    private int nextRandomEncounter = 0;

    private bool fadeMusicToSecretWorld = false;

    private bool startSecretWorldMusic = false;

    private int encounterBagFills = 0;
    private List<EncounterEvent> randomEncounterBag = new List<EncounterEvent>();
    bool fireRandomEncounters = true;    
    
    public FWorldLayer(FScene parent) : base(parent) { }

    public override void HandleMultiTouch(FTouch[] touches)
	{

	}
    
    public override void OnUpdate()
	{
        if (fadeMusicToSecretWorld)
        {
            if (FSoundManager.musicVolume > 0.0f)
            {
                FSoundManager.musicVolume -= (GameVars.Instance.MUSIC_VOLUME_CHANGE_INTERVAL * Time.deltaTime);                
            }
            else
            {
                fadeMusicToSecretWorld = false;                
            }
            startSecretWorldMusic = true;
        }

        //tileMap.LoadMoreTiles(5);

        if (elevatedLayer != null) { elevatedLayer.MoveToFront(); }

        CheckRockyBeaten();

        CheckGameOver();        

        if (SurviveGame.ALLOW_DEBUG)
        {
            if (Input.GetKeyDown(KeyCode.B))
            {
                ExecuteRandomEvent("BEAR");
            }

            if (Input.GetKeyDown(KeyCode.N))
            {
                player.MaxEnergy = 10000;
                GameVars.Instance.PLAYER_FULL_WATER = 1000;
                player.FullHeal();
            }

            if (Input.GetKeyDown(KeyCode.U))
            {
                player.Level++;
                player.FullHeal();
            }

            if (Input.GetKeyDown(KeyCode.L))
            {
                IPDebug.ForceAllowed = !IPDebug.ForceAllowed;
            }

            if (Input.GetKeyDown(KeyCode.V))
            {
                DoGameWon("Cheaters never lose.");
            }

            if (Input.GetKeyDown(KeyCode.G))
            {
                DoGameOver();
            }

            if (Input.GetKeyDown(KeyCode.PageUp))
            {
                //player.speed = new Vector2(1600, 1600);
                player.SpeedMultiplier = 10;
                player.IsFloating = true;
            }

            if (Input.GetKeyDown(KeyCode.PageDown))
            {
                player.SpeedMultiplier = 1;                
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

            if (Input.GetKeyDown(KeyCode.J))
            {
                SecretWorldEvent(null);                
            }

            if (Input.GetKeyDown(KeyCode.Keypad1))
            {
                string eventName = "BUTTER_BUG"; //generate random event from available events and tile type
                EncounterEvent randomEvent = EncounterEvent.CreateRandomEvent(eventName);
                eventQueue.Enqueue(randomEvent);
            }

            if (Input.GetKeyDown(KeyCode.Keypad2))
            {
                string eventName = "OPTORCHID"; //generate random event from available events and tile type
                EncounterEvent randomEvent = EncounterEvent.CreateRandomEvent(eventName);
                eventQueue.Enqueue(randomEvent);
            }

            if (Input.GetKeyDown(KeyCode.Keypad3))
            {
                string eventName = "FERAL_ANUM"; //generate random event from available events and tile type
                EncounterEvent randomEvent = EncounterEvent.CreateRandomEvent(eventName);
                eventQueue.Enqueue(randomEvent);
            }

            if (Input.GetKeyDown(KeyCode.Keypad4))
            {
                string eventName = "UNKNOWN"; //generate random event from available events and tile type
                EncounterEvent randomEvent = EncounterEvent.CreateRandomEvent(eventName);
                eventQueue.Enqueue(randomEvent);
            }

            if (Input.GetKeyDown(KeyCode.C))
            {
                DawnCrystalEvent(null);
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                FinalBattleEvent(null);
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                eventQueue.Enqueue(PullRandomEncounter());
            }
        }        

        if (this.Parent.Paused)
        {
            player.pause(true);
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
        //unpause player animation
        if (player.isPaused) { player.pause(); }

        if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
        {
            player.IsRunning = !player.IsRunning;
            if (player.IsRunning)
            {
                baseRandomEncounterInterval *= 2;
                maxRandomEncounterVariance *= 2;
            }
            else
            {
                baseRandomEncounterInterval = (int)(baseRandomEncounterInterval / 2);
                maxRandomEncounterVariance = (int)(maxRandomEncounterVariance / 2);
            }
        }

        if (startSecretWorldMusic)
        {
            Debug.Log("starting secret world music");
            FSoundManager.PlayMusic("03-Another World", GameVars.Instance.MUSIC_VOLUME, false);
            FSoundManager.CurrentMusicShouldLoop(true);
            startSecretWorldMusic = false;
            fadeMusicToSecretWorld = false;
        }

        //when the scene is unpaused, loadAddlTiles is set to true so that we will load more tiles on the next "pause"
        loadAddlTiles = true;
        //addlTileLoadRange = 1;        

        if (this.gameWon)
        {
            ((FWorldScene)this.Parent).DoGameWon();            
            return;
        }

        if (this.gameOver)
        {
            ((FWorldScene)this.Parent).DoGameOver();
            return;
        }

        if (RunEvents())
        {
            //if there are any events queued, keep running them until they are gone
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExecuteRandomEvent("GAME_MENU");
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

            if (Input.GetKey(KeyCode.UpArrow) ||
                Input.GetKey(KeyCode.W))
            {
                if (player.TileY < tileMap.HeightInTiles - 1)
                {
                    tileYDelta += 1;
                    playerMoved = true;
                }
            }
            else if (Input.GetKey(KeyCode.DownArrow) ||
                Input.GetKey(KeyCode.S))
            {
                if (player.TileY > 0)
                {
                    tileYDelta -= 1;
                    playerMoved = true;
                }
            }
            else if (Input.GetKey(KeyCode.LeftArrow) ||
                Input.GetKey(KeyCode.A))
            {
                if (player.TileX > 0)
                {
                    tileXDelta -= 1;
                    playerMoved = true;
                }
            }
            else if (Input.GetKey(KeyCode.RightArrow) ||
                Input.GetKey(KeyCode.D))
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
                    MovePlayerToTile(targetTileX, targetTileY, true);
                    //ChangePlayerTile(tileXDelta, tileYDelta);
                    //player.SetPosition(tileMap.GetTilePosition(player.TileX, player.TileY));
                    IPDebug.Log("Player position = " + player.GetPosition());
                    IPDebug.Log("Player tile = " + player.TileCoordinates);

                    if (player.IsRunning)
                    {
                        player.play("running");
                    }
                    else
                    {
                        player.play("walking");
                    }
                    //IPDebug.Log("Background position = " + background.GetPosition());
                }
                else { player.play("standing"); }
            }
            else { player.play("standing"); }
        }
	}

    private void CheckRockyBeaten()
    {
        if (!this.gameWon)
        {
            if (GameVars.Instance.GetParamValueBool(GameVarParams.ROCKY_BEATEN.ToString()))
            {
                DoGameWon("You have banished Esmudohr from this world forever!");
            }
        }
    }

    public override void OnEnter()
	{
        IPDebug.ForceAllowed = false;

        GameVars.Instance.ResetGame();

        IPDebug.Log("WorldScene OnEnter()");
        player = new Mob("player");
        GameVars.Instance.Player = player;
        //player.SetStat(MobStats.ENERGY, 100);
        //player.SetStat(MobStats.HP, 10);

        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.ATM_CARD));
        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.BUG_SPRAY));
        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.COMPASS));
        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.FIRST_AID_KIT));
        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.HONEY));
        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.LASER_POINTER));
        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.MARSHMALLOWS));
        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.RAW_MEAT));
        player.Inventory.Add(GameData.Instance.GetNewItem(ItemIDs.SALT));

        //currentRandomEncounterInterval = GameVars.Instance.RANDOM_ENCOUNTER_INTERVAL;
        FillEncounterBag();

        GenerateNextEncounterInterval();

        tileMap = new IPTileMap("Game", mapAsset, false);
        tileMap.LoadTileDataFile();

        GameVars.Instance.TileHelper = new TileMapHelper(tileMap);        

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

        player.play("standing", true);

        eventQueue.Enqueue(EncounterEvent.CreateTextEvent("Start", "You believe the Visitor Center should be somewhere to the northwest... maybe one of these paths will lead you back?"));
	}

    private void GenerateNextEncounterInterval()
    {
        randomEncounterSteps = 0;
        nextRandomEncounter = baseRandomEncounterInterval + UnityEngine.Random.Range(0, maxRandomEncounterVariance);
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


    private void DisplayMessage(string messageText)
    {
        FTextDisplayScene menu = new FTextDisplayScene("Message", messageText);
        FSceneManager.Instance.PushScene(menu);
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
            randomEncounterSteps++;
            if (randomEncounterSteps >= nextRandomEncounter) {

                eventQueue.Enqueue(PullRandomEncounter());
                GenerateNextEncounterInterval();
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
                case EncounterSource.TEXT:
                    DisplayMessage(gameEvent.Text);
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
            if (!GameVars.Instance.GetParamValueBool(GameVarParams.SECRET_WORLD.ToString()))
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
        }

        turnNumber++;

        CheckGameOver();
    }

    private void CheckGameOver()
    {
        if (!this.Parent.Paused)
        {
            if (!this.gameOver && canGameOver && player.Energy <= 0)
            {
                player.Energy = 0;
                DoGameOver();
            }
        }
    }

    private void DoGameOver()
    {
        string gameOverMessage = "";
        if (GameVars.Instance.GetParamValueBool(GameVarParams.SECRET_WORLD.ToString()))
        {
            gameOverMessage = "You were defeated... ";
        }
        else
        {
            gameOverMessage = "Oh no, you ran out of energy!\n\n\n\nThat's okay, luckily your friendly North Texarado State Park Rangers will always be there to make sure you're never in any real danger. They were quickly able to find and rescue you. Being outside is both fun AND safe!";
        }

        FTextDisplayScene gameOverScene = new FTextDisplayScene("GameOver", gameOverMessage);
        FSceneManager.Instance.PushScene(gameOverScene);
        this.gameOver = true;
        FSoundManager.PlayMusic("04-You Make Really Poor Choices", GameVars.Instance.MUSIC_VOLUME, false);
        FSoundManager.CurrentMusicShouldLoop(true);
        ((FWorldScene)this.Parent).musicShouldFadeOnNextScene = false;
    }

    private void WonGameEvent(IPTiledObject tileObject)
    {
        DoGameWon("You made it back to the Visitor Center safely!");
    }


    private void DoGameWon(string customMessage)
    {
        FTextDisplayScene wonGameMessage = new FTextDisplayScene("You Win", customMessage);
        FSceneManager.Instance.PushScene(wonGameMessage);
        this.gameWon = true;
        FSoundManager.PlayMusic("05-Way to Not Die", GameVars.Instance.MUSIC_VOLUME, false);
        FSoundManager.CurrentMusicShouldLoop(false);
        ((FWorldScene)this.Parent).musicShouldFadeOnNextScene = true;
    }



    private void FaerieRingEvent(IPTiledObject tileObject)
    {
        string displayMessage = "";

        if (GameVars.Instance.GetParamValueBool(GameVarParams.SECRET_WORLD.ToString()))
        {
            return;
        }
        else
        {
            displayMessage = "You feel a strange presence... as if you are being watched.\n\n\n" +
                "A voice seems to whisper inside your head...\n" +
                "\"Chosen one...\n" +
                "                      ...go back...\n" +
                "                                             ...follow...\n" +
                "                                                                   ...the mushrooms...\"";
        }

        FTextDisplayScene faerieRingMessage = new FTextDisplayScene("Faerie Ring", displayMessage);
        FSceneManager.Instance.PushScene(faerieRingMessage);

        GameVars.Instance.FAERIE_RING_FOUND = true;        
    }


    private void SecretWorldEvent(IPTiledObject tileObject)
    {
        if (GameVars.Instance.GetParamValueBool(GameVarParams.SECRET_WORLD.ToString()))
        {
            return;
        }

        ExecuteRandomEvent("SECRET_WORLD");

        GameVars.Instance.SECRET_WORLD_FOUND = true;
        GameVars.Instance.SetParamValue(GameVarParams.SECRET_WORLD.ToString(), true);

        //baseRandomEncounterInterval = (int) (baseRandomEncounterInterval * 1.5);
        GenerateNextEncounterInterval();

        //reset random encounter system
        encounterBagFills = 0;
        FillEncounterBag();
        fadeMusicToSecretWorld = true;
    }


    private void DawnCrystalEvent(IPTiledObject tileObject)
    {
        if (GameVars.Instance.GetParamValueBool(GameVarParams.DAWN_CRYSTAL.ToString()))
        {
            return;
        }

        ExecuteRandomEvent("DAWN_CRYSTAL");

        GameVars.Instance.DAWN_CRYSTAL_FOUND = true;
        GameVars.Instance.SetParamValue(GameVarParams.DAWN_CRYSTAL.ToString(), true);
    }

    private void FinalBattleEvent(IPTiledObject tileObject)
    {
        ExecuteRandomEvent("FINAL_BATTLE");
    }

    private void GroveEntranceEvent(IPTiledObject tileObject)
    {

        if (GameVars.Instance.GetParamValueBool(GameVarParams.GROVE_ENTERED.ToString()))
        {
            return;
        }

        string msgText = "Some kind of mysterious force is preventing you from entering the grove.";

        if (GameVars.Instance.GetParamValueBool(GameVarParams.DAWN_CRYSTAL.ToString()))
        {
            msgText = "The light from the Dawn Crystal flows out to the barrier at the entrance of the grove. " +
                "A bright light flashes, and the barrier is gone.";

            GameVars.Instance.SetParamValue(GameVarParams.GROVE_ENTERED.ToString(), true);
            GameVars.Instance.GROVE_ENTERED = true;
        }

        FTextDisplayScene menu = new FTextDisplayScene("Grove Entrance", msgText);
        FSceneManager.Instance.PushScene(menu);
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
            case "WON_GAME": WonGameEvent(tileObject);
                break;
            case "FAERIE_RING": FaerieRingEvent(tileObject);
                break;
            case "SECRET_WORLD": SecretWorldEvent(tileObject);
                break;
            case "DAWN_CRYSTAL": DawnCrystalEvent(tileObject);
                break;
            case "GROVE_ENTRANCE": GroveEntranceEvent(tileObject);
                break;
            case "FINAL_BATTLE": FinalBattleEvent(tileObject);
                break;
            case "NO_ENCOUNTERS": break; //used to not generate random encounters when moving
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
        if (GameVars.Instance.GetParamValueBool(GameVarParams.SECRET_WORLD.ToString()))
        {
            int bittyBugs = UnityEngine.Random.Range(5, 8);
            int feralAnums = UnityEngine.Random.Range(2, 5);
            int optorchids = UnityEngine.Random.Range(3, 7);            

            for (int i = 0; i < bittyBugs; i++)
            {
                randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("BUTTER_BUG"));
            }

            for (int i = 0; i < feralAnums; i++)
            {
                randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("FERAL_ANUM"));
            }

            for (int i = 0; i < optorchids; i++)
            {
                randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("OPTORCHID"));
            }

            for (int i = 0; i < optorchids; i++)
            {
                randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("OPTORCHID"));
            }

            //5% chance to spawn a single unknown
            int unknownChance = UnityEngine.Random.Range(0, 100);
            if (unknownChance < (10 * (encounterBagFills)))
            {
                randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("UNKNOWN"));
            }
            IPDebug.ForceLog("BittyBugs = " + bittyBugs + "\nFeralAnums = " + feralAnums + "\nOptorchids = " + optorchids + "\nUnknown = " + (unknownChance < 5).ToString());
        }
        else
        {
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

                int listSize = randomEncounterBag.Count / 2;
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

                int listSize = randomEncounterBag.Count / 2;
                //add a wild plant encounter for each other encounter - wild plants are common
                for (int i = 0; i < listSize; i++)
                {
                    randomEncounterBag.Add(EncounterEvent.CreateRandomEvent("WILD_PLANT"));
                }
                //do refill

            }
        }

        encounterBagFills++;
        //currentRandomEncounterInterval = GameVars.Instance.RANDOM_ENCOUNTER_INTERVAL * encounterBagFills;
    }

    private EncounterEvent PullRandomEncounter()
    {
        int randomEncounterIndex = UnityEngine.Random.Range(0, randomEncounterBag.Count);

        EncounterEvent pulledEncounter = randomEncounterBag[randomEncounterIndex];
        randomEncounterBag.RemoveAt(randomEncounterIndex);

        if (randomEncounterBag.Count == 0)
        {
            FillEncounterBag();
        }
        
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

        if (!canWalk && GameVars.Instance.Player.IsFloating)
        {
            string canFloatValue = "";
            bool canFloat = false;
            bool canFloatFound = false;

            //if we can't walk to the tile, and the player is floating,
            //check to see if we're allowed to float to the tile
            foreach (IPTiledObject tileObject in tileObjects)
            {
                //if the object says it can be floated on, set canFloat to true
                if (tileObject.PropertyExists(IPTileMapTileProperties.CAN_FLOAT.ToString()))
                {
                    canFloatValue = tileObject.GetPropertyValue(IPTileMapTileProperties.CAN_FLOAT.ToString());

                    IPDebug.Log("Target Tile Object = " + tileObject.Name + " | CAN_FLOAT = " + canFloatValue);

                    //just use first object we find that contains a valid property value then break out
                    if (bool.TryParse(canFloatValue, out canFloat))
                    {
                        canFloatFound = true;
                        break;
                    }
                }
            }

            //only check the tile if we didn't find any objects with floatable definitions
            if (!canFloatFound)
            {
                //check the canFloat value of the tile
                IPTileData targetTileData = targetTile.TileData;

                canFloatValue = targetTileData.GetPropertyValue(IPTileMapTileProperties.CAN_FLOAT.ToString());

                IPDebug.Log("Target Tile Asset = " + targetTileData.GetAssetName() + " | CAN_FLOAT = " + canFloatValue);

                bool.TryParse(canFloatValue, out canFloat);
            }

            canWalk = canFloat;
        }

        //if we STILL can't walk, then check if the player has the Dawn Crystal
        if (!canWalk && GameVars.Instance.GetParamValueBool(GameVarParams.DAWN_CRYSTAL.ToString()))
        {
            //if they have the dawn crystal, see if the tiles can be broken by it
            string canBreakValue = "";
            bool canBreak = false;
            bool canBreakFound = false;

            //if we can't walk to the tile, and the player is floating,
            //check to see if we're allowed to float to the tile
            foreach (IPTiledObject tileObject in tileObjects)
            {
                //if the object says it can be floated on, set canFloat to true
                if (tileObject.PropertyExists(IPTileMapTileProperties.CRYSTAL_BREAK.ToString()))
                {
                    canBreakValue = tileObject.GetPropertyValue(IPTileMapTileProperties.CRYSTAL_BREAK.ToString());

                    IPDebug.Log("Target Tile Object = " + tileObject.Name + " | CRYSTAL_BREAK = " + canBreakValue);

                    //just use first object we find that contains a valid property value then break out
                    if (bool.TryParse(canBreakValue, out canBreak))
                    {
                        canBreakFound = true;
                        break;
                    }
                }
            }

            //only check the tile if we didn't find any objects with floatable definitions
            if (!canBreakFound)
            {
                //check the canFloat value of the tile
                IPTileData targetTileData = targetTile.TileData;

                canBreakValue = targetTileData.GetPropertyValue(IPTileMapTileProperties.CAN_FLOAT.ToString());

                IPDebug.Log("Target Tile Asset = " + targetTileData.GetAssetName() + " | CRYSTAL_BREAK = " + canBreakValue);

                bool.TryParse(canBreakValue, out canBreak);
            }

            canWalk = canBreak;
        }

        return canWalk;
    }

    public override void Redraw(bool shouldForceDirty, bool shouldUpdateDepth)
    {
        Futile.stage.SetPosition(-player.GetPosition());
        base.Redraw(shouldForceDirty, shouldUpdateDepth);
    }
}