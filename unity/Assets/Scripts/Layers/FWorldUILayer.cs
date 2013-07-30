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
    
    public FWorldUILayer(FScene parent) : base(parent) { }

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
        }
	}

    public override void OnEnter()
	{
        player = GameVars.Instance.Player;

        playerStatus = new MessageBox(this.Parent, "", GameVars.Instance.STATUS_UI_RECT, GameVars.Instance.MESSAGE_TEXT_OFFSET, GameVars.Instance.STATUS_UI_RECT_ASSET);
        GameVars.Instance.GUIStage.AddChild(playerStatus);

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