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

    private float fadeOutTime = 0.25f;
    private float fadeInTime = 0.25f;
    private float fadeStartTime = 0.0f;

    private bool fadingIn = false;
    private bool fadingOut = false;

    //Vector2 maxBounds;
    public FTitleScene(string _name = "Default")
        : base(_name)
	{
		mName = _name;
        this.alpha = 0.0f;
	}
	
	public override void OnUpdate ()
	{
        if (this.fadingIn)
        {
            if (fadeInTime > 0)
            {
                this.alpha += (Time.deltaTime / fadeInTime);
            }

            if (Time.time - fadeStartTime >= fadeInTime)
            {

                this.alpha = 1.0f;
                fadingIn = false;
            }
        }

        if (this.fadingOut)
        {
            if (fadeOutTime > 0)
            {
                this.alpha -= (Time.deltaTime / fadeOutTime);
            }

            if (Time.time - fadeStartTime >= fadeOutTime)
            {
                this.alpha = 0.0f;
                fadingOut = false;
                //set new game scene
                FNewGameScene gameScene = new FNewGameScene("Game");

                FSceneManager.Instance.SetScene(gameScene);
            }

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //set new game scene
            if (!this.fadingOut)
            {
                this.fadingOut = true;
                this.fadeStartTime = Time.time;
            }
            
        }
		
	}

    public override void OnEnter()
	{
        this.stage.ResetPosition();
        GameVars.Instance.GUIStage.RemoveAllChildren();
        FSprite newGameImage = new FSprite("titleScreen");
        //newGameImage.color = Color.black;
        newGameImage.width = Futile.screen.width;
        newGameImage.height = Futile.screen.height;
        this.AddChild(newGameImage);

        FLabel label = new FLabel(GameVars.Instance.FONT_NAME, "Press [Space] to Begin");
        //label.anchorX = 1.0f;
        label.anchorY = 0.0f;
        //label.x = (Futile.screen.halfWidth) - 20;
        label.y = (-Futile.screen.halfHeight) + 20;
        this.AddChild(label);

        FSoundManager.PlayMusic("05-Welcome to the Woods, Dunce", GameVars.Instance.MUSIC_VOLUME, true);
        FSoundManager.CurrentMusicShouldLoop(false);

        this.fadeStartTime = Time.time;
        this.fadingIn = true;
	}

    public override void OnExit()
	{

	}

    
}

