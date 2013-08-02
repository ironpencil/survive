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

public class FWorldUILayer : FLayer
{
    Mob player;
    MessageBox playerStatus;

    FContainer miniMapContainer = new FContainer();
    FSprite miniMap;
    FSprite miniMapLocation;
    FSprite miniMapBackground;
    
    public FWorldUILayer(FScene parent) : base(parent) { }

    enum MiniMapState
    {
        INVISIBLE,
        VISIBLE,
        TRANSPARENT
    }

    private MiniMapState mapVisibility = MiniMapState.VISIBLE;

    public override void HandleMultiTouch(FTouch[] touches)
	{

	}
    
    public override void OnUpdate()
	{
        //if (this.Parent.Paused)
        //{
        //    return;
        //}

        if (player != null)
        {            
            UpdateUIText();
            miniMapLocation.x = (player.TileX / 2) + 1;
            miniMapLocation.y = (player.TileY / 2);
        }

        if (!this.Parent.Paused)
        {
            if (Input.GetKeyDown(KeyCode.M))
            {
                switch (mapVisibility)
                {
                    case MiniMapState.INVISIBLE:
                        //move to visible
                        mapVisibility = MiniMapState.VISIBLE;
                        miniMapContainer.alpha = 1.0f;
                        break;
                    case MiniMapState.VISIBLE:
                        //move to transparent
                        mapVisibility = MiniMapState.TRANSPARENT;
                        miniMapContainer.alpha = 0.5f;
                        break;
                    case MiniMapState.TRANSPARENT:
                        //move to invisible
                        mapVisibility = MiniMapState.INVISIBLE;
                        miniMapContainer.alpha = 0.0f;
                        break;
                    default:
                        break;
                }
            }
        }
	}

    public override void OnEnter()
	{
        player = GameVars.Instance.Player;

        playerStatus = new MessageBox(this.Parent, "", GameVars.Instance.STATUS_UI_RECT, GameVars.Instance.MESSAGE_TEXT_OFFSET, GameVars.Instance.STATUS_UI_RECT_ASSET);
        GameVars.Instance.GUIStage.AddChild(playerStatus);

        miniMapBackground = new FSprite("Futile_White");

        miniMap = new FSprite("miniMap");
        miniMap.anchorX = 0;
        miniMap.anchorY = 0;

        miniMapBackground.width = miniMap.width + 4;
        miniMapBackground.height = miniMap.height + 4;

        miniMapBackground.color = new Color(0.1f, 0.45f, 0.1f);

        miniMapBackground.anchorX = 0;
        miniMapBackground.anchorY = 0;
        miniMapBackground.x = miniMap.x - 2;
        miniMapBackground.y = miniMap.y - 2;

        miniMapLocation = new FSprite("Futile_White");

        miniMapLocation.color = new Color(0.5f, 0.0f, 0.7f);
        miniMapLocation.width = 3;
        miniMapLocation.height = 3;

        miniMapLocation.x = (player.TileX / 2) + 2;
        miniMapLocation.y = (player.TileY / 2);

        miniMapContainer.AddChild(miniMapBackground);
        miniMapContainer.AddChild(miniMap);
        miniMapContainer.AddChild(miniMapLocation);

        miniMapContainer.x = Futile.screen.halfWidth - (miniMap.width) - 20;
        miniMapContainer.y = Futile.screen.halfHeight - (miniMap.height) - 20;

        //miniMap.alpha = 0.75f;

        GameVars.Instance.GUIStage.AddChild(miniMapContainer);
        //MessageBox image = new MessageBox(this.Parent, "", GameVars.Instance.IMAGE_RECT, GameVars.Instance.MESSAGE_TEXT_OFFSET);
        //GameVars.Instance.GUIStage.AddChild(image);
	}

    public override void OnExit()
	{
		
	}

    private void UpdateUIText()
    {
        int energy = GameVars.Instance.Player.Energy;
        int water = GameVars.Instance.Player.Water;
        int points = GameVars.Instance.Player.WildernessPoints;       

        string uiFormat = "";
        string newText = "";

        if (GameVars.Instance.GetParamValueBool(GameVarParams.SECRET_WORLD.ToString()))
        {
            int level = GameVars.Instance.Player.Level;
            uiFormat = "HP : {0}     Level : {1}     XP : {2}";
            newText = string.Format(uiFormat, energy, level, points);
        }
        else
        {
            uiFormat = "Energy : {0}     Hydration : {1}     Points : {2}";
            newText = string.Format(uiFormat, energy, water, points);
        }

        if (!playerStatus.GetAllText().Equals(newText))
        {
            playerStatus.SetText(newText);
        }
    }

}