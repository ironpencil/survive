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

public class FMenuScene : FScene
{

    Player player;
    FTilemap tileMap;
    FLabel textLabel;

    Vector2 maxBounds;
    public FMenuScene(string _name = "Default")
        : base(_name)
	{
		mName = _name;
	}
	
	public override void OnUpdate ()
	{
        if (this.Paused)
        {
            return;
        }

        if (Input.GetKeyDown("space"))
        {
            FSceneManager.Instance.PopScene();
        }		
	}

    public override void OnEnter()
	{
        Debug.Log("MenuScene OnEnter()");


        FSprite menu = new FSprite("Futile_White");

        menu.width = Futile.screen.width * 0.8f;
        menu.height = Futile.screen.height * 0.8f;
        
        this.SetPosition(Futile.stage.GetPosition() * -1);

        Debug.Log("Stage position:" + stage.GetPosition());
        Debug.Log("scene position: " + this.GetPosition());

        this.AddChild(menu);

        FLabel textLabel = new FLabel("ComicSans", "Hello world! This is my blank menu!");
        //Futile.stage.AddChild(textLabel);

        this.AddChild(textLabel);
        textLabel.anchorY = 1;
        textLabel.y = Futile.screen.halfHeight * 0.8f;
	}

    public override void OnExit()
	{

	}
}
