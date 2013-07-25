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

    private int viewDistanceX = 18;
    private int viewDistanceY = 14;


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
        if (elevatedLayer != null) { elevatedLayer.MoveToFront(); }

        CheckGameOver();

        if (this.Parent.Paused)
        {
            return;
        }

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

        //check to see if we just came out of our inventory
        if (inSelectionDialog)
        {
            if (selectionScene != null && selectionScene.ItemWasSelected)
            {
                TreeNode<MenuNode> selectedNode = selectionScene.ResultPath;
                List<MenuNode> selectedItems = selectionScene.ResultPath.Flatten().ToList();

                //List<MenuNode> selectedItems = new List<MenuNode>();
                //selectedItems.Add(selectedNode.Value);
                //while (selectedNode.Children.Count > 0)
                //{
                //    selectedNode = selectedNode.Children[0];
                //    selectedItems.Add(selectedNode.Value);
                //}
                
                string msgText = "You selected \"" + string.Join(" -> ", selectedItems.Select(node => node.NodeText).ToArray()) + "\"!";
                //MenuNode itemSelected = selectionScene.SelectedItem.Value;
                //string msgText = "You selected '" + itemSelected.NodeText + "'!";
                FTextDisplayScene message = new FTextDisplayScene("menu", msgText);
                FSceneManager.Instance.PushScene(message);
            }

            inSelectionDialog = false;
        }
        //Futile.stage.Follow(player, false, false);
               
        //only handle input if player is standing still
        if (player.IsMovingToPosition)
        {
            if (player.ApproachTarget()) {
                player.IsMovingToPosition = false;
                //Check Events at new position
                TakeTurn();
                CheckForEvents();
                RunEvents();
                tileMap.SetVisibleRange(new Vector2(player.TileX, player.TileY), new Vector2(viewDistanceX, viewDistanceY));
            }
        }
        else
        {
            //if (Input.GetKey("left"))
            //{
            //    player.x -= (player.speed.x * Time.deltaTime);
            //    return;
            //}

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
            //if (Input.GetKeyDown("m"))
            //{
            //    MushroomsEncounter encounter = new MushroomsEncounter("Mushrooms");

            //    FSceneManager.Instance.PushScene(encounter);
            //    return;

            //}
            //if (Input.GetKeyDown("h"))
            //{
            //    //ModifyMobEvent modifyMob = new ModifyMobEvent(player, MobStats.ENERGY, -5);
            //    //modifyMob.Execute();
            //    string msgText = "Player Energy = " + player.GetStat(MobStats.ENERGY);
            //    FTextDisplayScene menu = new FTextDisplayScene("energy", msgText);
            //    FSceneManager.Instance.PushScene(menu);
            //    return; // don't run any other update code if they are opening the menu because this scene will be paused
            //}
            ////if (Input.GetKey("r"))
            ////{                       
            ////    tileMap.rotation += 5 * Time.deltaTime;
            ////}

            //if (Input.GetKeyDown("space"))
            //{
            //    string msgText = "This is my text that I would like to be displayed on multiple lines and on multiple labels. " +
            //                     "Hopefully this shouldn't be a problem. Can you think of any reason that it would be a problem? " +
            //                     "I sure can't. But maybe you can. Can you think of anything? I'm sorry, I should have let you finish your thought." + 
            //                     "\n\nThat was very rude of me to interrupt.\n\nSorry.\n\n.....\n\n\n\n\n\n\n\nTruly sorry.";
            //    FTextDisplayScene menu = new FTextDisplayScene("menu", msgText);
            //    FSceneManager.Instance.PushScene(menu);
            //    return; // don't run any other update code if they are opening the menu because this scene will be paused
            //}

            //if (Input.GetKeyDown("i"))
            //{



            //    //ModifyMobEvent reduceEnergy = new ModifyMobEvent(player, MobStats.ENERGY, -5);
            //    //ModifyMobEvent increaseEnergy = new ModifyMobEvent(player, MobStats.ENERGY, 5);

            //    //TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, "Menu", "Menu"));

            //    //MenuNode[] menuSubNodes = new MenuNode[] {
            //    //    MenuNode.CreateChoiceNode("Status", "Status", "View character status.", ""),
            //    //    MenuNode.CreateChoiceNode("Skills", "Skills", "View character skills.", "")
            //    //};

            //    //TreeNode<MenuNode> inventoryNode = new TreeNode<MenuNode>(new MenuNode(MenuNodeType.INVENTORY, "Inventory", "Inventory", "Opening Inventory... "));                

            //    //MenuNode[] invItems = new MenuNode[] {
            //    //    MenuNode.CreateConfirmationNode("Use Item", "ATM Card", "", "This is an ATM Card. Your dad gave it to you."),
            //    //    MenuNode.CreateConfirmationNode("Use Item", "First Aid Kit", "", "Some leaves, a needle, and a dirty Band-Aid."),
            //    //    MenuNode.CreateConfirmationNode("Use Item", "Honey", "", "I hope it was worth it."),
            //    //    MenuNode.CreateConfirmationNode("Use Item", "Compass", "", "It's pointing that way."),
            //    //    MenuNode.CreateConfirmationNode("Use Item", "Hearteater", "", "Kiss it."),
            //    //    MenuNode.CreateConfirmationNode("Use Item", "Brad Pitt's Wife's Head in a Box", "", "What's in the box?\n\n\nOh.")};
                

            //    //TreeNode<MenuNode>[] invItemNodes = inventoryNode.AddChildren(invItems);

            //    //foreach (TreeNode<MenuNode> invItemNode in invItemNodes)
            //    //{
            //    //    TreeNode<MenuNode> useNode = invItemNode.AddChild(MenuNode.CreateChoiceNode("Use Item", "Use", "Use item?", ""));
            //    //    TreeNode<MenuNode> infoNode = invItemNode.AddChild(MenuNode.CreateChoiceNode("Item Info", "Info", invItemNode.Value.NodeDescription, ""));

            //    //    MenuNode okNode = MenuNode.CreateOKNode("Confirm Use", "Yes", "You used the " + invItemNode.Value.NodeText, "");
            //    //    if (invItemNode.Value.NodeText.Equals("Hearteater"))
            //    //    {
            //    //        okNode.OnSelectionEvents.Add(reduceEnergy);
            //    //    }
            //    //    else if (invItemNode.Value.NodeText.Equals("First Aid Kit"))
            //    //    {
            //    //        okNode.OnSelectionEvents.Add(increaseEnergy);
            //    //    }

            //    //    useNode.AddChild(okNode);
            //    //    useNode.AddChild(MenuNode.CreateChoiceNode("Cancel Use", "No", "", ""));
            //    //}

            //    //rootNode.AddChildren(menuSubNodes);
            //    //rootNode.AddChild(inventoryNode);

            //    TreeNode<MenuNode> rootNode = GameData.Instance.MainMenu;


                        
            //    //List<string> invItems = new List<string>(){"ATM Card", "First Aid Kit", "Honey", "Compass", "Hearteater", "Brad Pitt's Wife's Head in a Box"};
            //    //string msgText = "ATM Card\nFirst Aid Kit\nHoney\nCompass\nHearteater";
            //    selectionScene = new FSelectionDisplayScene("Menu", rootNode);
            //    FSceneManager.Instance.PushScene(selectionScene);
            //    inSelectionDialog = true;
            //    return; // don't run any other update code if they are opening the menu because this scene will be paused
            //}

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
            FTextDisplayScene gameOverMessage = new FTextDisplayScene("GameOver", "You ran out of energy!");
            FSceneManager.Instance.PushScene(gameOverMessage);
            this.gameOver = true;
        }
    }

    private void WonGameEvent(IPTiledObject tileObject)
    {
        FTextDisplayScene wonGameMessage = new FTextDisplayScene("You Win", "You made it back to the Ranger's Office safely!");
        FSceneManager.Instance.PushScene(wonGameMessage);
        this.gameWon = true;
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