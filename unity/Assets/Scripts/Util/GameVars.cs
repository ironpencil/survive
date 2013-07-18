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
    public Color MENU_HIGHLIGHT_COLOR = new Color(1.0f, 1.0f, 1.0f, 0.75f);
    public string MENU_BORDER_ASSET = "Futile_White";
    public string MENU_INNER_ASSET = "Futile_White";
    public Rect MESSAGE_RECT = new Rect(0, (Futile.screen.halfHeight * 0.7f), Futile.screen.width * 0.95f, Futile.screen.height * 0.25f);
    public Rect SELECTION_RECT = new Rect((-Futile.screen.halfWidth * 0.25f), (-Futile.screen.halfHeight * 0.585f), Futile.screen.width * 0.7f, Futile.screen.height * 0.25f);
    public Rect INVENTORY_RECT = new Rect((Futile.screen.halfWidth * 0.7f), (-Futile.screen.halfHeight * 0.19f), Futile.screen.width * 0.25f, Futile.screen.height * 0.64f);
    public Rect IMAGE_RECT = new Rect((-Futile.screen.halfWidth * 0.7f), (Futile.screen.halfHeight * 0.055f), Futile.screen.width * 0.25f, Futile.screen.height * 0.395f);
    public Rect STATUS_UI_RECT = new Rect(0, (-Futile.screen.halfHeight * 0.915f), Futile.screen.width * 0.95f, Futile.screen.height * 0.085f);
    public float MESSAGE_TEXT_OFFSET = 20.0f;
    #endregion

    #region GameText
    public string INVENTORY = "Inventory";


    #endregion

    public FStage GUIStage;

    public Mob Player;

    public TileMapHelper TileHelper;

    #region GameVariables

    public int PLAYER_FULL_ENERGY = 50;
    public int PLAYER_FULL_WATER = 10;


    #endregion

    #region GameParams
    private Dictionary<string, object> gameParams;

    public void ResetGame()
    {
        gameParams = new Dictionary<string, object>();
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

}
