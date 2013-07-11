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

        #region GameConstants
        public string FONT_NAME = "ComicSans";
        public Color MENU_BACKGROUND_COLOR = new Color(0.0f, 0.5f, 0.0f);
        public Color MENU_FOREGROUND_COLOR = new Color(0.0f, 1.0f, 0.8f);
        public string MENU_BACKGROUND_ASSET = "Futile_White";
        public string MENU_FOREGROUND_ASSET = "Futile_White";
        public Rect MESSAGE_RECT = new Rect(0, (Futile.screen.halfHeight * 0.7f), Futile.screen.width * 0.9f, Futile.screen.height * 0.25f);
        public Rect INVENTORY_RECT = new Rect((Futile.screen.halfWidth * 0.7f), (-Futile.screen.halfHeight * 0.3f), Futile.screen.width * 0.25f, Futile.screen.height * 0.65f);
        #endregion
    
    }
