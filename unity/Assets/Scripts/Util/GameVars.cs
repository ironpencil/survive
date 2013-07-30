using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public sealed class GameVars
{
    private static readonly GameVars instance = new GameVars();

    public static GameVars Instance
    {
        get
        {
            return instance;
        }
    }

    private GameVars() { }

    #region UIConstants
    public string FONT_NAME = "ComicSans";
    public Color MENU_BORDER_COLOR = new Color(0.0f, 0.5f, 0.0f);
    public Color MENU_INNER_COLOR = new Color(0.0f, 1.0f, 0.8f);
    public Color MENU_HIGHLIGHT_COLOR = new Color(0.0f, 0.0f, 0.0f, 0.15f);
    public string MESSAGE_RECT_ASSET = "message_rect";
    public string SELECTION_RECT_ASSET = "selection_rect";
    public string INVENTORY_RECT_ASSET = "inventory_rect";
    public string IMAGE_RECT_ASSET = "image_rect";
    public string STATUS_UI_RECT_ASSET = "status_rect";
    public string SELECTION_DESC_RECT_ASSET = "selection_desc_rect";
    public Rect MESSAGE_RECT = new Rect(0, (Futile.screen.halfHeight * 0.7f) + 2, Futile.screen.width * 0.95f, Futile.screen.height * 0.25f);
    public Rect SELECTION_RECT = new Rect((-Futile.screen.halfWidth * 0.25f), (-Futile.screen.halfHeight * 0.585f) + 1, Futile.screen.width * 0.7f, Futile.screen.height * 0.25f);
    public Rect INVENTORY_RECT = new Rect((Futile.screen.halfWidth * 0.7f), (-Futile.screen.halfHeight * 0.19f) + 1, Futile.screen.width * 0.25f, Futile.screen.height * 0.64f);
    public Rect IMAGE_RECT = new Rect((-Futile.screen.halfWidth * 0.7f), (Futile.screen.halfHeight * 0.055f) + 2, Futile.screen.width * 0.25f, Futile.screen.height * 0.395f);
    public Rect STATUS_UI_RECT = new Rect(0, (-Futile.screen.halfHeight * 0.915f), Futile.screen.width * 0.95f, Futile.screen.height * 0.085f);
    public Rect SELECTION_DESC_RECT = new Rect((-Futile.screen.halfWidth * 0.25f), 0, Futile.screen.width * 0.7f, Futile.screen.height * 0.25f);
    public float MESSAGE_TEXT_OFFSET = 20.0f;
    #endregion

    #region GameText
    public string INVENTORY = "Inventory";


    #endregion

    public FStage GUIStage;

    public FStage FadeStage;

    public Mob Player;

    public TileMapHelper TileHelper;

    #region Secrets

    public int SECRETS_FOUND = 0;
    public int TOTAL_SECRETS = 1;

    public bool FAERIE_RING_FOUND = false;
    public bool SECRET_WORLD_FOUND = false;
    public bool EATEN_BY_BEARS = false;
    public bool DAWN_CRYSTAL_FOUND = false;
    public bool GROVE_ENTERED = false;
    public bool ROCKY_BEATEN = false;
    public bool UNKNOWN_SEEN = false;
    public bool UNKNOWN_BEATEN = false;

    #endregion
    #region GameVariables

    public bool BLUR_BUGS = false;

    public int PLAYER_STARTING_ENERGY = 100;
    public int PLAYER_STARTING_WATER = 10;
    public int PLAYER_FULL_WATER;

    public bool SHOW_ALTERNATE_TITLE = false;

    public int RANDOM_ENCOUNTER_INTERVAL = 6;

    public float MUSIC_VOLUME_CHANGE_INTERVAL = 0.1f;

    private float musicVolume = 0.5f;
    public float MUSIC_VOLUME
    {
        get { return musicVolume; }
        set
        {
            if (value < 0) { value = 0; }
            if (value > 1.0f) { value = 1.0f; }

            musicVolume = value;
            FSoundManager.volume = value;
        }
    }

    #endregion

    #region GameParams
    private Dictionary<string, object> gameParams;

    public void ResetGame()
    {
        gameParams = new Dictionary<string, object>();
        PLAYER_FULL_WATER = PLAYER_STARTING_WATER;
        SECRETS_FOUND = 0;
    }

    public object GetParamValue(string paramName)
    {
        object paramValue = null;

        gameParams.TryGetValue(paramName, out paramValue);

        return paramValue;
    }

    public string GetParamValueString(string paramName)
    {
        string returnValue = "";
        object paramValue = this.GetParamValue(paramName);

        try
        {
            returnValue = paramValue.ToString();
        }
        catch { }

        return returnValue;
    }

    public bool GetParamValueBool(string paramName)
    {
        bool returnValue = false;
        object paramValue = this.GetParamValue(paramName);

        try
        {
            returnValue = bool.Parse(paramValue.ToString());
        }
        catch { }

        return returnValue;
    }

    public void SetParamValue(string paramName, object paramValue)
    {
        if (gameParams.ContainsKey(paramName))
        {
            gameParams[paramName] = paramValue;
        }
        else
        {
            gameParams.Add(paramName, paramValue);
        }
    }
    #endregion


    #region battles

    public bool RollToHit(int attackerHitChance, int defenderEvadeChance)
    {
        int attackRoll = UnityEngine.Random.Range(0, 100);

        if (attackRoll >= attackerHitChance) { return false; }

        int evadeRoll = UnityEngine.Random.Range(0, 100);

        if (evadeRoll < defenderEvadeChance) { return false; }

        return true;

    }

    public bool RollCritChance(int attackerCritChance, int defenderCritEvadeChance)
    {
        int attackRoll = UnityEngine.Random.Range(0, 100);

        if (attackRoll >= attackerCritChance) { return false; }

        int evadeRoll = UnityEngine.Random.Range(0, 100);

        if (evadeRoll < defenderCritEvadeChance) { return false; }

        return true;

    }

    public int RollAttackDamage(int attackerPower, int attackerMultiplier, int defenderDefense)
    {

        int damageRoll = UnityEngine.Random.Range(0, (attackerPower / 5) + 1);
        int damage = (attackerPower + damageRoll - defenderDefense) * attackerMultiplier;

        if (damage < 0) { damage = 0; }

        return damage;

    }

    #endregion

}
