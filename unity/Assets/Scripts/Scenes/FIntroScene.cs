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

public class FIntroScene : FScene
{

    private enum CurrentLogo
    {
        SAGD,
        IRON_PENCIL,
        STATE_PARK,
        PRESENTS
    }

    float currentLogoStartTime = 0.0f;
    float logoElapsedTime = 0.0f;
    bool introFinished = false;

    const float sagdDuration = 4.0f;
    const float ironPencilDuration = 2.0f;
    const float stateParkDuration = 5.0f;
    const float presentsDuration = 3.0f;

    FSprite sagdLogo;
    FLabel ironPencilLabel;
    FLabel stateParkLabel;
    FLabel presentsLabel;

    CurrentLogo currentLogo = CurrentLogo.SAGD;
    

    //Vector2 maxBounds;
    public FIntroScene(string _name = "Intro")
        : base(_name)
	{
		mName = _name;
	}
	
	public override void OnUpdate ()
	{
        //allow you to skip intro
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //set new game scene
            FTitleScene titleScene = new FTitleScene("Title");

            FSceneManager.Instance.SetScene(titleScene);
            return;
        }

        if (!introFinished)
        {
            logoElapsedTime = Time.time - currentLogoStartTime;

            switch (currentLogo)
            {
                case CurrentLogo.SAGD:
                    UpdateSAGDLogo();
                    break;
                case CurrentLogo.IRON_PENCIL:
                    UpdateIPLogo();
                    break;
                case CurrentLogo.STATE_PARK:
                    UpdateSPLogo();
                    break;
                case CurrentLogo.PRESENTS:
                    UpdatePresentsLogo();
                    break;
                default:
                    break;
            }
        }

        if (introFinished)
        {
            //set new game scene
            FTitleScene titleScene = new FTitleScene("Title");

            FSceneManager.Instance.SetScene(titleScene);
        }
		
	}

    private void UpdatePresentsLogo()
    {
        if (logoElapsedTime < presentsDuration * 0.5f)
        {
            presentsLabel.alpha += 1.5f * Time.deltaTime;
        }

        if (logoElapsedTime > presentsDuration * 0.75f)
        {
            presentsLabel.alpha -= 2 * Time.deltaTime;
        }

        //switch to next logo
        if (logoElapsedTime > presentsDuration)
        {
            introFinished = true;
            this.RemoveChild(presentsLabel);
        }
    }

    private void UpdateSPLogo()
    {
        if (logoElapsedTime < stateParkDuration * 0.5f)
        {
            stateParkLabel.alpha += 2.0f * Time.deltaTime;
        }

        if (logoElapsedTime > stateParkDuration * 0.85f)
        {
            stateParkLabel.alpha -= 2 * Time.deltaTime;
            ironPencilLabel.alpha -= 2 * Time.deltaTime;
        }

        //switch to next logo
        if (logoElapsedTime > stateParkDuration)
        {
            this.currentLogo = CurrentLogo.PRESENTS;
            currentLogoStartTime = Time.time;
            this.RemoveChild(stateParkLabel);
            this.RemoveChild(ironPencilLabel);
        }
    }

    private void UpdateIPLogo()
    {
        if (logoElapsedTime < ironPencilDuration * 0.5f)
        {
            ironPencilLabel.alpha += 2.0f * Time.deltaTime;
        }


        if (logoElapsedTime > ironPencilDuration)
        {            
            this.currentLogo = CurrentLogo.STATE_PARK;
            currentLogoStartTime = Time.time;
        }
    }

    private void UpdateSAGDLogo()
    {
        if (logoElapsedTime < sagdDuration * 0.5f)
        {
            sagdLogo.alpha += 1.5f * Time.deltaTime;
        }


        if (logoElapsedTime > sagdDuration * 0.75f)
        {
            sagdLogo.alpha -= 2 * Time.deltaTime;
        }

        //switch to next logo
        if (logoElapsedTime > sagdDuration)
        {
            this.currentLogo = CurrentLogo.IRON_PENCIL;
            currentLogoStartTime = Time.time;
            this.RemoveChild(sagdLogo);
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

        sagdLogo = new FSprite("sagdLogo");
        sagdLogo.alpha = 0.0f;

        ironPencilLabel = new FLabel(GameVars.Instance.FONT_NAME, "Iron Pencil Studios");
        ironPencilLabel.alpha = 0.0f;
        ironPencilLabel.y = (Futile.screen.halfHeight * 0.25f);

        stateParkLabel = new FLabel(GameVars.Instance.FONT_NAME,
            "in assocation with\n" +
            "the Parks and Wildlife Department of the\n" +
            "state of North Texarado");
        stateParkLabel.alpha = 0.0f;
        stateParkLabel.y = (-Futile.screen.halfHeight * 0.25f);

        presentsLabel = new FLabel(GameVars.Instance.FONT_NAME, "Presents");
        presentsLabel.alpha = 0.0f;

        this.AddChild(sagdLogo);
        this.AddChild(ironPencilLabel);
        this.AddChild(stateParkLabel);
        this.AddChild(presentsLabel);

        currentLogoStartTime = Time.time;
	}

    public override void OnExit()
	{

	}

    
}

