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

    private float fadeOutTime = 2.5f;
    private float fadeInTime = 1.0f;
    private float fadeStartTime = 0.0f;

    //private float sceneLength = 5.0f;
    //private float sceneStartTime = 0.0f;

    private bool fadingIn = false;
    private bool fadingOut = false;
    private bool walkingOff = false;
    //private bool timingScene = false;

    FAnimatedSprite player;

    FLabel gameWonLabel;

    //Vector2 maxBounds;
    public FGameWonScene(string _name = "GameWon")
        : base(_name)
	{
		mName = _name;
        this.alpha = 0.0f;
	}
	
	public override void OnUpdate ()
	{
        player.MoveToBack();
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
        //        return;
        //    }
        //} 

        if (this.walkingOff)
        {
            player.y -= 160 * Time.deltaTime;
            gameWonLabel.y += 160 * Time.deltaTime;

            if (player.y < -Futile.screen.halfHeight)
            {
                if (!this.fadingOut)
                {
                    this.fadingIn = false;
                    //this.timingScene = false;
                    this.fadingOut = true;
                    this.fadeStartTime = Time.time;
                }
            }
        }

        if (this.fadingOut)
        {
            if (fadeOutTime > 0)
            {
                float alphaDiff = Time.deltaTime / fadeOutTime;
                this.alpha -= alphaDiff;
                //FSoundManager.musicVolume -= alphaDiff;
            }

            if (Time.time - fadeStartTime >= fadeOutTime)
            {
                this.alpha = 0.0f;
                fadingOut = false;
                FSceneManager.Instance.SetScene(new FCreditsScene("Credits"));
            }
        }


        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
        {
            //pressed [Space], make the scene transition faster
            //this.fadeOutTime = 0.5f;
            player.play("walking");
            this.walkingOff = true;

            //if (!this.fadingOut)
            //{
            //    this.fadingIn = false;
            //    //this.timingScene = false;
            //    this.fadingOut = true;
            //    this.fadeStartTime = Time.time;
            //}
            return;
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

        string winMessage = GameVars.Instance.GetParamValueString(GameVarParams.WIN_MESSAGE.ToString());

        if (winMessage.Length == 0)
        {
            winMessage = "You made it back to the Visitor Center!";
        }

        string points = GameVars.Instance.GetParamValueString(GameVarParams.POINTS.ToString());

        string secretsFound = GameVars.Instance.SECRETS_FOUND.ToString();

        string totalSecrets = GameVars.Instance.TOTAL_SECRETS.ToString();

        gameWonLabel = new FLabel(GameVars.Instance.FONT_NAME, 
            "Congratulations! " + winMessage + "\n\n" +
            "Wilderness Survival Points: " + points + "\n\n" + 
            "Secrets Found: " + secretsFound + " / " + totalSecrets + "\n\n" + 
            "Press [Space] to Continue.");
        this.AddChild(gameWonLabel);

        player = new FAnimatedSprite("player");
        player.addAnimation(new FAnimation("standing", new int[1] { 0 }, 500, true));
        player.addAnimation(new FAnimation("walking", new int[2] { 1, 2 }, 200, true));

        //player.y = (gameWonLabel.textRect.height / 2) + 64;
        player.y = -(gameWonLabel.textRect.height / 2) - 64;
        player.play("standing", true);
        this.AddChild(player);

        this.fadeStartTime = Time.time;
        this.fadingIn = true;


        string musicAsset = "07-Strange Papers";

        if (GameVars.Instance.GetParamValueBool("MUSIC_FOUND"))
        {
            musicAsset = "06-The Long Walk Home";
        }

        FSoundManager.PlayMusic(musicAsset, GameVars.Instance.MUSIC_VOLUME, true);
        FSoundManager.CurrentMusicShouldLoop(false);
	}

    public override void OnExit()
	{
        //FSoundManager.StopMusic();
	}

    
}

