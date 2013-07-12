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

public class FSelectionDisplayScene : FScene
{

    Player player;
    FTilemap tileMap;
    FLabel textLabel;

    SelectionBox selectBox;

    List<string> choiceList;

    public string SelectedItem { get; private set; }
    public bool ItemWasSelected { get; private set; }

    Vector2 maxBounds;

    FStage guiStage;

    public float Width { get; set; }

    public float Height { get; set; }

    public Rect Bounds { get; set; }

    public FSelectionDisplayScene(string name, List<string> choiceList, Rect bounds)
        : base(name)
	{
		mName = name;
        this.choiceList = choiceList;
        this.Bounds = bounds;
	}
	
	public override void OnUpdate ()
	{
        if (this.Paused)
        {
            return;
        }

        //this.SetPosition(Futile.stage.GetPosition() * -1);
        if (selectBox.ItemIsSelected) {
            this.ItemWasSelected = true;
            this.SelectedItem = selectBox.SelectedItem;
            FSceneManager.Instance.PopScene();
        }	
	}

    public override void OnEnter()
	{
        guiStage = new FStage("GUI");

        ItemWasSelected = false;
        SelectedItem = "";

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
        selectBox = new SelectionBox(this, choiceList, Bounds.width, Bounds.height, GameVars.Instance.MESSAGE_TEXT_OFFSET);
        
        selectBox.x = Bounds.x;
        selectBox.y = Bounds.y;

        guiStage.AddChild(selectBox);

        Futile.AddStage(guiStage);
	}

    public override void OnExit()
	{

        Futile.RemoveStage(guiStage);

	}
}
