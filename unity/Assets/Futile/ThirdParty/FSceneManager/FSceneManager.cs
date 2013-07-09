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

public sealed class FSceneManager : FContainer
{
	private static readonly FSceneManager mInstance = new FSceneManager();
	public static FSceneManager Instance
	{
		get
		{
			return mInstance;
		}
	}
	
	private List<FScene> mScenes;
	public static FStage mStage;

	private FSceneManager() : base()
	{
		mScenes = new List<FScene>();
		mStage = Futile.stage;

		mStage.AddChild(this);
	}

    public void PushScene(FScene _scene)
    {
        // Pause scenes underneath
        foreach (FScene scene in mScenes)
            scene.Paused = true;

        mScenes.Add(_scene);
        this.AddChild(_scene);
    }

	public void PopScene()
	{
		if(mScenes.Count > 0)
		{
			FScene scene = mScenes[mScenes.Count - 1];
			scene.RemoveFromContainer();

			mScenes.Remove(scene);
		}
		
		// Unpause scene
		if(mScenes.Count > 0)
		{
			FScene scene = mScenes[mScenes.Count - 1];
			scene.Paused = false;

            //Jim: Seems like old scene is staying on screen, let's draw over it
            scene.MoveToFront();
		}
	}

	public void SetScene(FScene _scene)
	{
		while(mScenes.Count > 0)
			PopScene();

		PushScene(_scene);
	}

    private string GetSceneListDescription()
    {
        string sceneList = "";
        for (int i = 0; i < mScenes.Count; i++)
        {

            sceneList += "[" + i + "]" + mScenes[i].Name + "\r\n";
        }
        return sceneList;
    }
}