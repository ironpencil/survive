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

public class FGameWonScene : FScene
{
   

    //Vector2 maxBounds;
    public FGameWonScene(string _name = "GameWon")
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
        FSprite gameWonImage = new FSprite("Futile_White");
        gameWonImage.color = Color.black;
        gameWonImage.width = Futile.screen.width;
        gameWonImage.height = Futile.screen.height;
        this.AddChild(gameWonImage);
        FLabel label = new FLabel(GameVars.Instance.FONT_NAME, "Congratulations! You won! Press Space to Play Again!");
        this.AddChild(label);
	}

    public override void OnExit()
	{
        
	}

    
}

