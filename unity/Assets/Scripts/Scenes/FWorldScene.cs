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

public class FWorldScene : FScene
{

    FWorldLayer worldLayer;
    FWorldUILayer guiLayer;


    private float fadeOutTime = 1.0f;
    private float fadeInTime = 1.0f;
    private float fadeStartTime = 0.0f;

    private bool fadingIn = false;
    private bool fadingOut = false;

    FScene nextScene = null;
    
    public bool musicShouldFadeOnNextScene = false;
   

    //Vector2 maxBounds;
    public FWorldScene(string _name = "Default")
        : base(_name)
	{
		mName = _name;
        //this.alpha = 0.0f;        
	}
	
	public override void OnUpdate ()
	{
        if (this.fadingIn)
        {
            //pause this so the sub-layers can't do anything
            this.Paused = true;
            if (fadeInTime > 0)
            {
                float alphaDiff = Time.deltaTime / fadeOutTime;
                //this.alpha += alphaDiff;
                //worldLayer.alpha += alphaDiff;
                //guiLayer.alpha += alphaDiff;
                GameVars.Instance.FadeStage.alpha -= alphaDiff;
            }

            if (Time.time - fadeStartTime >= fadeInTime)
            {

                //this.alpha = 1.0f;
                //worldLayer.alpha = 1.0f;
                //guiLayer.alpha = 1.0f;
                GameVars.Instance.FadeStage.alpha = 0.0f;
                fadingIn = false;
                this.Paused = false;
            }
        }

        if (this.fadingOut)
        {
            //pause this so the sub-layers can't do anything
            this.Paused = true;
            if (fadeOutTime > 0)
            {
                float alphaDiff = Time.deltaTime / fadeOutTime;
                //this.alpha -= alphaDiff;
                //worldLayer.alpha -= alphaDiff;
                //guiLayer.alpha -= alphaDiff;
                GameVars.Instance.FadeStage.alpha += alphaDiff;
                if (musicShouldFadeOnNextScene)
                {
                    FSoundManager.musicVolume -= alphaDiff;
                }
            }

            if (Time.time - fadeStartTime >= fadeOutTime)
            {
                //this.alpha = 0.0f;
                //worldLayer.alpha = 0.0f;
                //guiLayer.alpha = 0.0f;
                GameVars.Instance.FadeStage.alpha = 1.0f;
                fadingOut = false;
                FSceneManager.Instance.SetScene(nextScene);
            }

        }
        
		
	}

    public override void OnEnter()
	{
        worldLayer = new FWorldLayer(this);
        this.AddChild(worldLayer);

        guiLayer = new FWorldUILayer(this);
        this.AddChild(guiLayer);

        GameVars.Instance.FadeStage.alpha = 1.0f;

        //GameVars.Instance.GUIStage.alpha = 0.0f;

        //worldLayer.alpha = 0.0f;
        //guiLayer.alpha = 0.0f;

        FSoundManager.PlayMusic("01-Overworld", GameVars.Instance.MUSIC_VOLUME, true);
        FSoundManager.CurrentMusicShouldLoop(true);

        this.fadeStartTime = Time.time;
        this.fadingIn = true;
	}

    public override void OnExit()
	{
        GameVars.Instance.FadeStage.alpha = 0.0f;
	}

    public void DoGameOver()
    {
        nextScene = new FGameOverScene("GameOver");
        this.fadingOut = true;
        this.fadeOutTime = 1.0f;
        fadeStartTime = Time.time;
    }

    public void DoGameWon()
    {
        nextScene = new FGameWonScene("GameWon");
        this.fadingOut = true;
        this.fadeOutTime = 2.0f;
        fadeStartTime = Time.time;
    }

    
}

