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

    private float fadeOutTime = 0.5f;
    private float fadeInTime = 0.5f;
    private float fadeStartTime = 0.0f;    

    private bool fadingIn = false;
    private bool fadingOut = false;

    //Vector2 maxBounds;
    public FGameOverScene(string _name = "Default")
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
                float alphaDiff = Time.deltaTime / fadeOutTime;
                this.alpha -= alphaDiff;
                FSoundManager.musicVolume -= alphaDiff;
            }

            if (Time.time - fadeStartTime >= fadeOutTime)
            {
                this.alpha = 0.0f;
                fadingOut = false;
                FSceneManager.Instance.SetScene(new FTitleScene("Title"));
            }

        }


        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            //set new game scene
            this.fadingOut = true;
            this.fadeStartTime = Time.time;
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
        FLabel label = new FLabel(GameVars.Instance.FONT_NAME, "You weren't able to make it back without help...\n\n\n\nPress [Space] to try again.");
        this.AddChild(label);

        FAnimatedSprite sadPlayer = new FAnimatedSprite("player_sad");
        sadPlayer.addAnimation(new FAnimation("cry", new int[2] { 0, 1 }, 500, true));
        this.AddChild(sadPlayer);

        this.fadeStartTime = Time.time;
        this.fadingIn = true;
	}

    public override void OnExit()
	{
        FSoundManager.StopMusic();
	}

    
}

