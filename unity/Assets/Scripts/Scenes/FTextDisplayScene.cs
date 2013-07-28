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

public class FTextDisplayScene : FScene
{

    Mob player;
    FTilemap tileMap;
    FLabel textLabel;

    MessageBox msgBox = null;

    string messageText = "";

    Vector2 maxBounds;

    public float Width { get; set; }

    public float Height { get; set; }

    public FTextDisplayScene(string name, string messageText)
        : base(name)
	{
		mName = name;
        this.messageText = messageText;
	}
	
	public override void OnUpdate ()
	{
        if (this.Paused)
        {
            return;
        }

        //this.SetPosition(Futile.stage.GetPosition() * -1);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!msgBox.Next())
            {
                FSceneManager.Instance.PopScene();
            }
        }		
	}

    public override void OnEnter()
	{
        IPDebug.Log("MenuScene OnEnter()");       

        //FSprite menu = new FSprite("Futile_White");

        //menu.width = Futile.screen.width * 0.25f;
        //menu.height = Futile.screen.height * 0.25f;
        
        //this.SetPosition(Futile.stage.GetPosition() * -1);

        IPDebug.Log("Stage position:" + stage.GetPosition());
        IPDebug.Log("scene position: " + this.GetPosition());

        //this.AddChild(menu);
        //guiStage.AddChild(menu);

        //FLabel textLabel = new FLabel(GameVars.Instance.FONT_NAME, "Hello world! This is my blank menu!");
        //Futile.stage.AddChild(textLabel);

        //string msgText = "This is my text that I would like to be displayed on multiple lines and on multiple labels. Hopefully this shouldn't be a problem. Can you think of any reason that it would be a problem? I sure can't.";
        msgBox = new MessageBox(this, messageText, GameVars.Instance.MESSAGE_RECT, GameVars.Instance.MESSAGE_TEXT_OFFSET, GameVars.Instance.MESSAGE_RECT_ASSET);
        
        //msgBox.x = Bounds.x;
        //msgBox.y = Bounds.y;

        GameVars.Instance.GUIStage.AddChild(msgBox);

	}

    public override void OnExit()
	{
        if (msgBox != null) { GameVars.Instance.GUIStage.RemoveChild(msgBox); }

	}
}
