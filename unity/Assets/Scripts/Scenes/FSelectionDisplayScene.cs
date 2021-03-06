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

    public Mob player;

    public bool IsClosed { get; set; }
    private FSelectionDisplayScene childScene = null;

    private bool messageBoxInFocus = false;
    private bool selectBoxInFocus = false;
    MessageBox messageBox = null;
    SelectionBox selectBox = null;
    MessageBox imageBox = null;

    private bool fadeUnlessInventorySelected = true;

    List<string> choiceList;

    TreeNode<MenuNode> rootNode;

    FLabel cancelLabel = null;

    public TreeNode<MenuNode> SelectedItem { get; private set; }
    public bool ItemWasSelected { get; private set; }

    public TreeNode<MenuNode> ResultPath { get; private set; }

    public float Width { get; set; }

    public float Height { get; set; }

    public FSelectionDisplayScene(string name, TreeNode<MenuNode> rootNode)
        : base(name)
    {
        mName = name;
        if (rootNode.Value.NodeType == MenuNodeType.INVENTORY)
        {
            rootNode = new TreeNode<MenuNode>(rootNode.Value); //add inventory items to a copy of the root node
            rootNode.AddChildren(GameData.Instance.GenerateInventoryNodes());

            //show cancel label
            cancelLabel = new FLabel(GameVars.Instance.FONT_NAME, "Backspace to Cancel");
            //cancelLabel.scale = 0.9f;
        }
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
            if (messageBox.HasNext())
            {
                if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                {
                    //if (!messageBox.Next())
                    //{
                    //    ShowSelectBox();
                    //}
                    messageBox.Next();
                }
            }
            else
            {
                //display the select box
                //if there is no select box to display, await input to close the dialog
                if (!ShowSelectBox())
                {
                    if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
                    {
                        this.Close();
                    }
                }
            }
        }
        else if (selectBoxInFocus)
        {
            //can only currently cancel Inventory nodes
            if (Input.GetKeyDown(KeyCode.Backspace) &&
                this.rootNode.Value.NodeType == MenuNodeType.INVENTORY)
            {
                this.ItemWasSelected = false;
                this.SelectedItem = null;
                selectBoxInFocus = false;
            }

            if (selectBox.ItemIsSelected)
            {
                this.ItemWasSelected = true;
                this.SelectedItem = selectBox.SelectedItem;

                childScene = new FSelectionDisplayScene(this.SelectedItem.Value.NodeTitle, this.SelectedItem);
                FSceneManager.Instance.PushScene(childScene);
                selectBoxInFocus = false;
                //try fading out this selection box
                //TODO: Fade the selectBox out if the selected node has a HideAfterSelection value of true
                if (HasDisplayMessage()) { this.messageBox.alpha = this.SelectedItem.Value.ParentAlphaWhenSelected; }
                this.selectBox.alpha = this.SelectedItem.Value.ParentAlphaWhenSelected;

                if (fadeUnlessInventorySelected && this.SelectedItem.Value.NodeType != MenuNodeType.INVENTORY)
                {
                    if (this.selectBox != null) { this.selectBox.alpha = 0.0f; }
                    if (this.messageBox != null) { this.messageBox.alpha = 0.0f; }
                    if (this.imageBox != null) { this.imageBox.alpha = 0.0f; }
                }
                //this.selectBox.alpha = 0.5f;
                //this.selectBox.isVisible = false;
            }
        }
        else if (childScene == null)
        {
            FSceneManager.Instance.PopScene();
            this.IsClosed = true;
        }
        else
        {
            if (childScene.IsClosed)
            {
                //TODO: Decide whether a child scene closing should or should not close this scene. May want multiple branching paths.
                //Example: Select item in inventory, "Use item?", click No, get shown Inventory again.



                //if none of our display boxes are being displayed, scrap this scene         

                //return a tree with all of the "selected" items from child scenes
                bool shouldPopScene = true;
                switch (this.SelectedItem.Value.AfterSelection)
                {
                    case MenuNode.AfterSelectionBehavior.CLOSE_PARENT:
                        shouldPopScene = true;
                        break;
                    case MenuNode.AfterSelectionBehavior.CLOSE_ALL:
                        shouldPopScene = true;
                        break;
                    case MenuNode.AfterSelectionBehavior.KEEP_PARENT_OPEN:
                        shouldPopScene = false;
                        break;
                    default:
                        break;
                }

                this.ResultPath = new TreeNode<MenuNode>(this.SelectedItem.Value);

                if (childScene.ItemWasSelected)
                {
                    this.ResultPath.AddChild(childScene.ResultPath);
                    if (childScene.SelectedItem.Value.AfterSelection == MenuNode.AfterSelectionBehavior.CLOSE_ALL)
                    {
                        shouldPopScene = true;
                        this.SelectedItem.Value.AfterSelection = MenuNode.AfterSelectionBehavior.CLOSE_ALL;
                    }
                }
                else
                {
                    if (childScene.rootNode.Value.NodeType == MenuNodeType.INVENTORY)
                    {
                        //cancelled out of inventory, don't close this node
                        shouldPopScene = false;
                    }
                }

                if (shouldPopScene)
                {
                    FSceneManager.Instance.PopScene();
                    this.IsClosed = true;
                }
                else
                {
                    if (HasDisplayMessage()) { messageBox.alpha = 1.0f; }
                    if (HasSelectionOptions()) { selectBox.alpha = 1.0f; }
                    //TODO: Have to reset the selection box
                    this.Reset();
                    this.ShowSelectBox();
                }
            }
        }
    }

    public override void OnEnter()
    {
        ItemWasSelected = false;
        SelectedItem = null;

        List<IGameEvent> gameEvents = rootNode.Value.OnSelectionEvents;

        foreach (IGameEvent gameEvent in gameEvents)
        {
            gameEvent.Execute();
        }

        ShowImage();

        //show the message box. if none exists, show the select box.
        if (!ShowMessageBox())
        {
            ShowSelectBox();
        }
	}    

    public override void OnExit()
	{
        if (selectBox != null) { GameVars.Instance.GUIStage.RemoveChild(selectBox); }
        if (messageBox != null) { GameVars.Instance.GUIStage.RemoveChild(messageBox); }
        if (imageBox != null) { GameVars.Instance.GUIStage.RemoveChild(imageBox); }
        //Futile.RemoveStage(guiStage);

	}

    private bool ShowMessageBox()
    {
        messageBoxInFocus = false;
        if (HasDisplayMessage())
        {
            messageBox = new MessageBox(this, rootNode.Value.DisplayMessage, GameVars.Instance.MESSAGE_RECT, GameVars.Instance.MESSAGE_TEXT_OFFSET, GameVars.Instance.MESSAGE_RECT_ASSET);
            GameVars.Instance.GUIStage.AddChild(messageBox);
            messageBoxInFocus = true;
        }
        return messageBoxInFocus;
    }

    private bool HasDisplayMessage()
    {
        return rootNode.Value.DisplayMessage.Length > 0;
    }

    private bool ShowSelectBox()
    {
        selectBoxInFocus = false;
        if (HasSelectionOptions())
        {
            if (selectBox != null)
            {
                selectBox.stage.RemoveChild(selectBox);
            }
            Rect boundsRect = GameVars.Instance.SELECTION_RECT;
            string backgroundAsset = GameVars.Instance.SELECTION_RECT_ASSET;
            bool showSelectionDescriptions = false;
            switch (rootNode.Value.NodeType)
            {
                case MenuNodeType.TEXT:
                    boundsRect = GameVars.Instance.SELECTION_RECT;
                    backgroundAsset = GameVars.Instance.SELECTION_RECT_ASSET;
                    break;
                case MenuNodeType.INVENTORY:
                    boundsRect = GameVars.Instance.INVENTORY_RECT;
                    backgroundAsset = GameVars.Instance.INVENTORY_RECT_ASSET;
                    showSelectionDescriptions = true;
                    //rootNode = new TreeNode<MenuNode>(rootNode.Value); //add inventory items to a copy of the root node
                    //rootNode.AddChildren(GameData.Instance.GenerateInventoryNodes());
                    break;
                default:
                    break;
            }            
            selectBox = new SelectionBox(this, rootNode, boundsRect, GameVars.Instance.MESSAGE_TEXT_OFFSET, backgroundAsset, showSelectionDescriptions);
            if (cancelLabel != null)
            {
                selectBox.AddChild(cancelLabel);
                cancelLabel.x = 0.0f;
                cancelLabel.y = -(selectBox.Height / 2) + (selectBox.textAreaOffset / 2) + (cancelLabel.textRect.height / 2) + 2;
                cancelLabel.alpha = 0.5f;
            }
            GameVars.Instance.GUIStage.AddChild(selectBox);
            selectBoxInFocus = true;
            messageBoxInFocus = false;
        }
        return selectBoxInFocus;
    }

    private bool HasSelectionOptions()
    {
        return (rootNode.Children.Count > 0);
    }

    private void ShowImage()
    {
        if (HasImage())
        {
            FSprite image = new FSprite(rootNode.Value.DisplayImageAsset);
            imageBox = new MessageBox(this, "", GameVars.Instance.IMAGE_RECT, GameVars.Instance.MESSAGE_TEXT_OFFSET, GameVars.Instance.IMAGE_RECT_ASSET);
            imageBox.AddChild(image);
            image.y += 1;
            image.MoveToBack();
            GameVars.Instance.GUIStage.AddChild(imageBox);
        }
    }

    private bool HasImage()
    {
        return (rootNode.Value.DisplayImageAsset.Length > 0);
    }

    private void Close()
    {
        //remove focus from message/select boxes, Update() will close
        messageBoxInFocus = false;
        selectBoxInFocus = false;
    }

    private void Reset()
    {
        this.ItemWasSelected = false;
        this.SelectedItem = null;
        this.childScene = null;
    }

    public static TreeNode<MenuNode> GetLastResultNode(TreeNode<MenuNode> resultPath)
    {
        TreeNode<MenuNode> returnNode = resultPath;

        if (returnNode.Children.Count > 0)
        {
            returnNode = GetLastResultNode(returnNode.Children[0]);
        }

        return returnNode;
    }
}
