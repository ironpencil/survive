using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public abstract class FEncounterScene : FScene
{
    protected TreeNode<MenuNode> resultNode = null;
    protected FSelectionDisplayScene currentScene = null;

    public bool IsFinished { get; private set; }

    //Vector2 maxBounds;
    public FEncounterScene(string _name = "Default")
        : base(_name)
	{
		mName = _name;
	}
	
	public override void OnUpdate ()
	{
        base.OnUpdate();

        if (this.Paused)
        {
            return;
        }

        if (this.currentScene != null & currentScene.IsClosed)
        {
            if (currentScene.ItemWasSelected)
            {
                this.resultNode = currentScene.ResultPath;

                this.HandleResult();
            }
            else
            {
                this.HandleCancel();
            }
        }
		
	}

    public override void OnEnter()
	{
        base.OnEnter();
        IsFinished = false;
	}

    public override void OnExit()
	{        
        base.OnExit();
        IsFinished = true;
	}

    protected void LoadNewSelectionScene(string name, TreeNode<MenuNode> rootNode)
    {
        this.currentScene = new FSelectionDisplayScene(name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

    protected virtual void HandleResult() { }

    protected virtual void HandleCancel() { }    

}
