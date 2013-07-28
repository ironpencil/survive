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

public class FCreditsScene : FScene
{

    private float fadeOutTime = 4.0f;
    private float fadeInTime = 1.0f;
    private float fadeStartTime = 0.0f;

    //private float sceneLength = 10.0f;
    //private float sceneStartTime = 0.0f;
    FAnimatedSprite player;

    FLabel creditsLabel;

    private bool fadingIn = false;
    private bool fadingOut = false;
    //private bool timingScene = false;

    int playerScreenWrapMargin = 160;

    List<string> credits = new List<string>();
    int creditsIndex = 0;
    bool creditsFinished = false;

    //Vector2 maxBounds;
    public FCreditsScene(string _name = "Credits")
        : base(_name)
	{
		mName = _name;
        this.alpha = 0.0f;
	}
	
	public override void OnUpdate ()
	{
        player.MoveToBack();
        if (creditsFinished)
        {
            if (player.y <= creditsLabel.y - (creditsLabel.textRect.height / 2) - playerScreenWrapMargin)
            {
                player.play("standing");
            }
            else
            {
                player.y -= 160 * Time.deltaTime;
            }
        }
        else
        {
            if (player.y <= -Futile.screen.halfHeight - playerScreenWrapMargin)
            {
                player.y = Futile.screen.halfHeight + playerScreenWrapMargin;
                //int spawnRange = (int) (Futile.screen.halfWidth - player.width);
                //player.x = UnityEngine.Random.Range(-spawnRange, spawnRange);

                creditsIndex++;
                creditsLabel.text = credits[creditsIndex];
                if (creditsIndex == credits.Count - 1)
                {
                    creditsFinished = true;
                }
            }
            else
            {
                player.y -= 160 * Time.deltaTime;
            }
        }

        creditsLabel.y = -player.y;


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
                //this.timingScene = true;
                //sceneStartTime = Time.time;
            }

        }

        //if (this.timingScene)
        //{
        //    if (Time.time - sceneStartTime >= sceneLength)
        //    {
        //        this.timingScene = false;
        //        this.fadingOut = true;
        //        this.fadeStartTime = Time.time;
        //    }
        //} 

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
                FSoundManager.musicVolume = 0.0f;
                fadingOut = false;
                FSceneManager.Instance.SetScene(new FTitleScene("Title"));
            }

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            //pressed space, speed up fade out
            //this.fadeOutTime = 1.0f;
            if (!this.fadingOut)
            {
                this.fadingIn = false;
                //this.timingScene = false;
                this.fadingOut = true;
                this.fadeStartTime = Time.time;
            }
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
        //this.AddChild(gameWonImage);

        credits.Add("Danger Aversion Training:\nAdventures in Survival Safety\n\nwas made possible by...");
        credits.Add("Programming\n\nJim South");
        credits.Add("Art\n\nJim South");
        credits.Add("Additional Art\n\nInternet Janitor\n\nMike Stiller");
        credits.Add("Music\n\nbagina");
        credits.Add("Best Font\n\nComic Sans");
        credits.Add("Long Nights\n\nMonster Energy");
        credits.Add("Motivation\n\nThe Something Awful Goons");
        credits.Add("Iron Pencil Studios would like to thank you for playing!\n\nWe hope you enjoyed DAT:ASS!");



        creditsLabel = new FLabel(GameVars.Instance.FONT_NAME, credits[creditsIndex]);
        this.AddChild(creditsLabel);

        player = new FAnimatedSprite("player");
        player.addAnimation(new FAnimation("standing", new int[1] { 0 }, 500, true));
        player.addAnimation(new FAnimation("walking", new int[2] { 1, 2 }, 200, true));

        player.y = Futile.screen.halfHeight + playerScreenWrapMargin;
        player.play("walking", true);
        this.AddChild(player);

        this.fadeStartTime = Time.time;
        this.fadingIn = true;
	}

    public override void OnExit()
	{
        FSoundManager.StopMusic();
	}

    
}

