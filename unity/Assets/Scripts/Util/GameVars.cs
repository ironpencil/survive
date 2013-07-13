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
        public Rect MESSAGE_RECT = new Rect(0, (Futile.screen.halfHeight * 0.7f), Futile.screen.width * 0.9f, Futile.screen.height * 0.25f);
        public Rect SELECTION_RECT = new Rect(0, (-Futile.screen.halfHeight * 0.7f), Futile.screen.width * 0.9f, Futile.screen.height * 0.25f);
        public Rect INVENTORY_RECT = new Rect((Futile.screen.halfWidth * 0.7f), (-Futile.screen.halfHeight * 0.3f), Futile.screen.width * 0.25f, Futile.screen.height * 0.65f);
        public float MESSAGE_TEXT_OFFSET = 20.0f;
        #endregion

        #region GameText
        public string INVENTORY = "Inventory";


        #endregion

        public FStage GUIStage;



    }
