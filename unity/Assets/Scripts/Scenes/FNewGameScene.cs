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

public class FNewGameScene : FScene
{

    private float fadeOutTime = 0.5f;
    private float fadeInTime = 0.25f;
    private float fadeStartTime = 0.0f;

    private bool fadingIn = false;
    private bool fadingOut = false;

    //Vector2 maxBounds;
    public FNewGameScene(string _name = "Default")
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
                FWorldScene gameScene = new FWorldScene("Game");

                FSceneManager.Instance.SetScene(gameScene);
            }

        }


        if (Input.GetKeyDown(KeyCode.Space))
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
        FSprite newGameImage = new FSprite("howToPlayScreen");
        //newGameImage.color = Color.black;
        newGameImage.width = Futile.screen.width;
        newGameImage.height = Futile.screen.height;
        this.AddChild(newGameImage);

        FLabel headerLabel = new FLabel(GameVars.Instance.FONT_NAME, "How to Play:");

        headerLabel.anchorY = 1.0f;
        headerLabel.y = (Futile.screen.halfHeight) * 0.85f;
        this.AddChild(headerLabel);

        FLabel howToPlayLabel1 = new FLabel(GameVars.Instance.FONT_NAME,
            "You have become lost in the wilderness, and must make\n" +
            "your way back to the safety of the Visitor Center!\n\n" +
            "Along the way you will find yourself in many dangerous\n" +
            "situations, and it's up to you to decide what to do!");

        FLabel howToPlayLabel2 = new FLabel(GameVars.Instance.FONT_NAME,
            "You will also be quizzed on general survival safety.\n" +
            "Answer correctly to earn Wilderness Survival Points!\n\n" +
            "Use the Arrow Keys to move, and\n" +
            "use Space to make selections and interact.\n\n" +
            "Good luck!");

        //howToPlayLabel1.anchorY = 1.0f;
        howToPlayLabel1.y = Futile.screen.halfHeight * 0.35f;
        //howToPlayLabel2.anchorY = 1.0f;
        howToPlayLabel2.y = -Futile.screen.halfHeight * 0.35f;
        this.AddChild(howToPlayLabel1);
        this.AddChild(howToPlayLabel2);

        //howToPlay.anchorY = 1.0f;

        FLabel label = new FLabel(GameVars.Instance.FONT_NAME, "Press Space to Continue");
        //label.anchorX = 1.0f;
        label.anchorY = 0.0f;
        //label.x = (Futile.screen.halfWidth) - 20;
        label.y = (-Futile.screen.halfHeight) + 20;
        this.AddChild(label);

        FSoundManager.PlayMusic("intro1", GameVars.Instance.MUSIC_VOLUME, false);
        FSoundManager.CurrentMusicShouldLoop(true);

        this.fadeStartTime = Time.time;
        this.fadingIn = true;
    }

    public override void OnExit()
	{

	}

    
}

