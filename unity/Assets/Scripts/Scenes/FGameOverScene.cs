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

public class FGameOverScene : FScene
{
   

    //Vector2 maxBounds;
    public FGameOverScene(string _name = "Default")
        : base(_name)
	{
		mName = _name;
	}
	
	public override void OnUpdate ()
	{
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //set new game scene
            FSceneManager.Instance.SetScene(new FNewGameScene("NewGame"));
        }
		
	}

    public override void OnEnter()
	{
        this.stage.ResetPosition();
        GameVars.Instance.GUIStage.RemoveAllChildren();
        FSprite gameOverImage = new FSprite("Futile_White");
        gameOverImage.color = Color.black;
        gameOverImage.width = Futile.screen.width;
        gameOverImage.height = Futile.screen.height;
        this.AddChild(gameOverImage);
        FLabel label = new FLabel(GameVars.Instance.FONT_NAME, "Game Over - Press Space to Try Again");
        this.AddChild(label);
	}

    public override void OnExit()
	{
        
	}

    
}

