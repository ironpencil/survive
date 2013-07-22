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

public class FTitleScene : FScene
{
   

    //Vector2 maxBounds;
    public FTitleScene(string _name = "Default")
        : base(_name)
	{
		mName = _name;
	}
	
	public override void OnUpdate ()
	{
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //set new game scene
            FNewGameScene gameScene = new FNewGameScene("Game");

            FSceneManager.Instance.SetScene(gameScene);
        }
		
	}

    public override void OnEnter()
	{
        this.stage.ResetPosition();
        GameVars.Instance.GUIStage.RemoveAllChildren();
        FSprite newGameImage = new FSprite("Futile_White");
        newGameImage.color = Color.black;
        newGameImage.width = Futile.screen.width;
        newGameImage.height = Futile.screen.height;
        this.AddChild(newGameImage);

        FLabel label = new FLabel(GameVars.Instance.FONT_NAME, "Title Screen - Press Space to Start New Game");
        this.AddChild(label);
	}

    public override void OnExit()
	{

	}

    
}

