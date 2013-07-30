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

    private float titleFlickerTimer = 0.0f;
    List<float> titleFlickerIntervals = new List<float>() { 6.0f, 10.0f, 5.0f, 0.5f, 10.0f };
    int titleFlickerIndex = 0;

    float flickerTime = 0.05f;
    //int flickerCounter = 0;

    FLabel continueLabel;

    FSprite titleImage;
    FSprite altTitleImage;

    bool altDisplayed = false;

    //Vector2 maxBounds;
    public FTitleScene(string _name = "Default")
        : base(_name)
	{
		mName = _name;
        this.alpha = 0.0f;
	}
	
	public override void OnUpdate ()
	{
        altTitleImage.alpha = this.alpha;

        //flickerCounter++;
        //if (flickerCounter >= flickerFrames)
        if (altDisplayed)
        {
            if (Time.time - titleFlickerTimer > flickerTime)
            {
                altDisplayed = false;
                //titleImage.MoveToFront();
                titleImage.alpha = this.alpha;
                titleFlickerTimer = Time.time;
            }
        }

        if (!altDisplayed)
        {
            if (Time.time - titleFlickerTimer > titleFlickerIntervals[titleFlickerIndex])
            {
                //Debug.Log("flicker at " + titleFlickerIntervals[titleFlickerIndex]);
                titleFlickerIndex++;
                if (titleFlickerIndex >= titleFlickerIntervals.Count) { titleFlickerIndex = 0; }

                //flickerCounter = 0;
                altDisplayed = true;

                titleFlickerTimer = Time.time;
                //altTitleImage.MoveToFront();
                //altTitleImage.alpha = this.alpha;
                titleImage.alpha = this.alpha * 0.6f;
            }
        }

        if (this.fadingIn)
        {
            altTitleImage.alpha = 0.0f;
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
            altTitleImage.alpha = 0.0f;
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

        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            //set new game scene
            if (!this.fadingOut)
            {
                this.fadingOut = true;
                this.fadeStartTime = Time.time;
            }
            
        }

        continueLabel.MoveToFront();
	}

    public override void OnEnter()
	{
        this.stage.ResetPosition();
        GameVars.Instance.GUIStage.RemoveAllChildren();

        string titleScreenAsset = "titleScreen";

        if (GameVars.Instance.SHOW_ALTERNATE_TITLE)
        {
            titleScreenAsset = "titleScreen_alt";
        }

        altTitleImage = new FSprite("titleScreen_alt");

        altTitleImage.width = Futile.screen.width;
        altTitleImage.height = Futile.screen.height;
        altTitleImage.alpha = 0.0f;
        this.AddChild(altTitleImage);

        titleImage = new FSprite(titleScreenAsset);
        //newGameImage.color = Color.black;
        titleImage.width = Futile.screen.width;
        titleImage.height = Futile.screen.height;
        this.AddChild(titleImage);

        continueLabel = new FLabel(GameVars.Instance.FONT_NAME, "Press [Space] to Begin");
        //label.anchorX = 1.0f;
        continueLabel.anchorY = 0.0f;
        //label.x = (Futile.screen.halfWidth) - 20;
        continueLabel.y = (-Futile.screen.halfHeight) + 20;
        this.AddChild(continueLabel);

        FSoundManager.PlayMusic("01-Welcome to the Woods, Dunce", GameVars.Instance.MUSIC_VOLUME, true);
        FSoundManager.CurrentMusicShouldLoop(false);        

        this.fadeStartTime = Time.time;
        this.fadingIn = true;

        this.titleFlickerTimer = Time.time;

        altTitleImage.MoveToBack();
        titleImage.MoveToFront();
        continueLabel.MoveToFront();
	}

    public override void OnExit()
	{

	}

    
}

