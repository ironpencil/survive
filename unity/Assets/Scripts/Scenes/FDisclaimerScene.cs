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

public class FDisclaimerScene : FScene
{



    float introStartTime = 0.0f;
    float introLength = 5.0f;
    bool introFinished = false;
    bool sceneFading = false;

    float warningAlpha = 0.0f;
    FLabel warningLabel;
    FLabel disclaimerLabel;
    FLabel continueText;

    FScene nextScene = new FIntroScene("Intro");

    //Vector2 maxBounds;
    public FDisclaimerScene(string _name = "Default")
        : base(_name)
	{
		mName = _name;
	}
	
	public override void OnUpdate ()
	{
        if (Input.GetKeyDown(KeyCode.S))
        {
            if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)))
            {
                if (SurviveGame.ALLOW_DEBUG)
                {
                    //immediately skip to next scene
                    FSceneManager.Instance.SetScene(new FTitleScene("NewGame"));
                }
            }

        }

        if (sceneFading)
        {
            this.alpha -= 1.0f * Time.deltaTime;

            if (this.alpha <= 0.0f)
            {
                //FIntroScene gameScene = new FIntroScene("Intro");

                FSceneManager.Instance.SetScene(nextScene);
            }

            return;
        }
        
        if (!introFinished)
        {
            float timeDifference = Time.time - introStartTime;

            if (warningAlpha < 1.0f)
            {
                warningAlpha += 1.0f * Time.deltaTime;
                warningLabel.alpha = warningAlpha;
                disclaimerLabel.alpha = warningAlpha;
            }

            if (timeDifference > introLength)
            {
                introFinished = true;
            }
        }

        if (introFinished)
        {                                 
            this.AddChild(continueText);

            if (continueText.alpha < 1.0f)
            {
                continueText.alpha += 4.0f * Time.deltaTime;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {

                //fade this scene out
                sceneFading = true;
                return;
            }
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

        warningLabel = new FLabel(GameVars.Instance.FONT_NAME, "WARNING");
        warningLabel.y = (Futile.screen.halfHeight) * 0.7f;
        warningLabel.scale = 2.0f;
        warningLabel.alpha = 0.0f;

        this.AddChild(warningLabel);

        disclaimerLabel = new FLabel(GameVars.Instance.FONT_NAME, "");
        disclaimerLabel.text = 
            "By playing this game, you acknowledge that you understand it is intended\n" +
            "for entertainment purposes only. The suggestions presented herein are likely\n" + 
            "to result in injury or even death. Iron Pencil Studios makes no claim as to\n" + 
            "the safety or accuracy of the advice provided.\n\n" +
            "A video game is no replacement for the experience and knowledge that comes from\n" +
            "years of training in the wild. Be sure to consult a wilderness safety expert\n" +
            "before following ANY of the advice or performing ANY of the techniques suggested\n" + 
            "in this game, as there is ALWAYS a risk of danger when venturing outdoors.";

        disclaimerLabel.alpha = 0.0f;
        this.AddChild(disclaimerLabel);

        continueText = new FLabel(GameVars.Instance.FONT_NAME, "Press Space to Continue");
        continueText.y = (-Futile.screen.halfHeight) * 0.8f;
        continueText.alpha = 0.0f;

        introStartTime = Time.time;
	}

    public override void OnExit()
	{

	}

    
}

