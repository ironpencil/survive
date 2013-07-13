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

public class FSelectionDisplayScene : FScene
{

    public Player player;

    public bool IsClosed { get; set; }
    private FSelectionDisplayScene childScene = null;

    private bool messageBoxInFocus = false;
    private bool selectBoxInFocus = false;
    MessageBox messageBox = null;
    SelectionBox selectBox = null;

    List<string> choiceList;

    TreeNode<MenuNode> rootNode;

    public TreeNode<MenuNode> SelectedItem { get; private set; }
    public bool ItemWasSelected { get; private set; }

    public float Width { get; set; }

    public float Height { get; set; }

    public FSelectionDisplayScene(string name, TreeNode<MenuNode> rootNode)
        : base(name)
    {
        mName = name;
        this.rootNode = rootNode;
    }

    public override void OnUpdate()
    {
        if (this.Paused)
        {
            return;
        }

        if (messageBoxInFocus)
        {
            if (Input.GetKeyDown("space"))
            {
                if (!messageBox.Next())
                {
                    ShowSelectBox();
                }
            }
        }
        else if (selectBoxInFocus)
        {
            if (selectBox.ItemIsSelected)
            {
                this.ItemWasSelected = true;
                this.SelectedItem = selectBox.SelectedItem;

                //TODO: When the subScene is done, have it pass back its selectedItem to this parent

                childScene = new FSelectionDisplayScene(this.SelectedItem.Value.NodeTitle, this.SelectedItem);
                FSceneManager.Instance.PushScene(childScene);
                selectBoxInFocus = false;
            }
        }
        else
        {
            if (childScene == null || childScene.IsClosed)
            {
                //if none of our display boxes are being displayed, scrap this scene                
                FSceneManager.Instance.PopScene();
                this.IsClosed = true;
            }
        }
    }

    public override void OnEnter()
    {
        ItemWasSelected = false;
        SelectedItem = null;
        
        if (rootNode.Value.DisplayMessage.Length > 0)
        {
            ShowMessageBox();
        }
        else
        {
            ShowSelectBox();
        }
        //Futile.AddStage(guiStage);
	}    

    public override void OnExit()
	{
        if (selectBox != null) { GameVars.Instance.GUIStage.RemoveChild(selectBox); }
        if (messageBox != null) { GameVars.Instance.GUIStage.RemoveChild(messageBox); }
        //Futile.RemoveStage(guiStage);

	}

    private void ShowMessageBox()
    {
        messageBox = new MessageBox(this, rootNode.Value.DisplayMessage, GameVars.Instance.MESSAGE_RECT, GameVars.Instance.MESSAGE_TEXT_OFFSET);
        GameVars.Instance.GUIStage.AddChild(messageBox);
        messageBoxInFocus = true;
    }

    private void ShowSelectBox()
    {
        messageBoxInFocus = false;
        if (rootNode.Children.Count > 0)
        {
            Rect boundsRect = GameVars.Instance.SELECTION_RECT;
            switch (rootNode.Value.NodeType)
            {
                case MenuNodeType.TEXT:
                    boundsRect = GameVars.Instance.SELECTION_RECT;
                    break;
                case MenuNodeType.INVENTORY:
                    boundsRect = GameVars.Instance.INVENTORY_RECT;
                    break;
                default:
                    break;
            }
            selectBox = new SelectionBox(this, rootNode, boundsRect, GameVars.Instance.MESSAGE_TEXT_OFFSET);
            GameVars.Instance.GUIStage.AddChild(selectBox);
            selectBoxInFocus = true;
        }
    }
}
