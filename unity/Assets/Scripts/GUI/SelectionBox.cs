using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class SelectionBox : FLayer
{

    //private string messageText { get; set; }

    public float Width { get; set; }

    public float Height { get; set; }

    private float TextAreaWidth { get; set; }

    private float TextAreaHeight { get; set; }

    List<FLabel> labels = new List<FLabel>();
    private int currentlySelectedIndex = -1;
    private FLabel currentlySelectedLabel = null;
    private FLabel previouslySelectedLabel = null;

    private FSprite background;
    private FSprite foreground;
    private FSprite highlight;

    MessageBox itemDescBox = null;
    private bool showNodeDescriptions = false;

    public TreeNode<MenuNode> SelectedItem { get; private set; }
    public bool ItemIsSelected { get; private set; }

    public float textPadding = 4;
    public float textAreaOffset;    

    public SelectionBox(FScene parent, TreeNode<MenuNode> rootNode, Rect bounds, float textOffset)
        : base(parent)
    {
        this.Width = bounds.width;
        this.Height = bounds.height;
        TextAreaWidth = this.Width - textOffset;
        TextAreaHeight = this.Height - textOffset;
        this.textAreaOffset = textOffset;
        this.x = bounds.x;
        this.y = bounds.y;
        //if (backgroundAsset.Length > 0)
        //{
        background = new FSprite(GameVars.Instance.MENU_BORDER_ASSET);
        background.width = this.Width;
        background.height = this.Height;
        background.color = GameVars.Instance.MENU_BORDER_COLOR;
        this.AddChild(background);
        //}

        //if (foregroundAsset.Length > 0)
        //{
        foreground = new FSprite(GameVars.Instance.MENU_INNER_ASSET);
        foreground.width = TextAreaWidth + textPadding;
        foreground.height = TextAreaHeight + textPadding;
        foreground.color = GameVars.Instance.MENU_INNER_COLOR;
        this.AddChild(foreground);

        highlight = new FSprite(GameVars.Instance.MENU_INNER_ASSET);
        highlight.color = GameVars.Instance.MENU_HIGHLIGHT_COLOR;
        //}        

        //set up the labels        
        SetupLabels(rootNode);

        foreach (FLabel label in labels)
        {
            this.AddChild(label);
        }
    }

    public SelectionBox(FScene parent, TreeNode<MenuNode> rootNode, Rect bounds, float textOffset, bool showDescriptions)
        : this(parent, rootNode, bounds, textOffset)
    {
        showNodeDescriptions = showDescriptions;

        if (showNodeDescriptions)
        {
            this.itemDescBox = new MessageBox(parent, "", GameVars.Instance.SELECTION_DESC_RECT, GameVars.Instance.MESSAGE_TEXT_OFFSET);
        }
    }

    //Next() returns true if a new label was displayed
    //Will return true when you display the last label, but not the next time you call it
    //public bool Next()
    //{
    //    displayedLabelIndex++;

    //    if (displayedLabelIndex < labelText.Count())
    //    {
    //        //this.RemoveChild(displayedLabel);
    //        displayedLabel.text = labelText.ElementAt(displayedLabelIndex);
    //        //this.AddChild(displayedLabel);
    //        return true;
    //    }
    //    else
    //    {
    //        //don't auto-close anymore, can call Close() to remove label or just remove the whole box
    //        //this.RemoveChild(displayedLabel);            
    //        return false;
    //    }
    //}

    public void Close()
    {
        foreach (FLabel label in labels)
        {
            this.RemoveChild(label);
        }        
    }

    //public string GetAllText()
    //{
    //    return messageText;
    //}

    //public string GetDisplayedText()
    //{
    //    return displayedLabel.text;
    //}

    //public void SetText(string messageText)
    //{
    //    this.messageText = messageText;
    //    SetupLabels();
    //}    

    private void SetupLabels(TreeNode<MenuNode> rootNode)
    {

        //StringBuilder sb = new StringBuilder();
        labels.Clear();

        SelectedItem = null;
        ItemIsSelected = false;

        float heightRemaining = TextAreaHeight;

        foreach (TreeNode<MenuNode> childNode in rootNode.Children)
        {
            string choiceText = childNode.Value.NodeText;
            FLabel currentLabel = new FLabel(GameVars.Instance.FONT_NAME, "");

            string currentText = "";
            string newLine = "\n";
            Vector2 size = Vector2.zero;

            //this.RemoveAllLabels();
            string[] lines = choiceText.Split(newLine.ToCharArray());

            foreach (string line in lines)
            {
                string[] words = line.Split(' ');
                string space = ""; // don't start with a space

                IPDebug.Log("words length = " + words.Length);
                foreach (string word in words)
                {
                    string newText = currentText + space + word;
                    IPDebug.Log("newText = " + newText);
                    size = currentLabel.MeasureText(newText);

                    if (size.x <= TextAreaWidth)
                    {
                        currentText = newText;
                    }
                    else
                    {
                        //selection box choices can't wrap to a new dialog
                        newText = currentText + newLine + " " + word;
                        currentText = newText;
                    }

                    space = " "; //update space string to actually contain a space after the first time through
                }

                //at the end of a line
                currentText += newLine;
            }

            //add the label to the list
            currentLabel.text = currentText;
            currentLabel.data = childNode;
            labels.Add(currentLabel);

            currentLabel.anchorX = 0.0f; //left
            currentLabel.anchorY = 1.0f; //top
            currentLabel.x = -(this.Width - textAreaOffset) / 2;
            currentLabel.y = ((this.Height - textAreaOffset) / 2) - (TextAreaHeight - heightRemaining);
            heightRemaining -= currentLabel.textRect.height;
        }
        //this.AddChild(displayedLabel);
    }

    public override void OnEnter()
    {
        base.OnEnter();

        //if we have any labels, highlight the first one
        if (labels.Count > 0)
        {
            currentlySelectedIndex = 0;
            currentlySelectedLabel = labels[currentlySelectedIndex];

            highlight.width = TextAreaWidth;
            highlight.height = currentlySelectedLabel.textRect.height;

            highlight.anchorX = 0.0f;
            highlight.anchorY = 1.0f;
            highlight.x = currentlySelectedLabel.x;
            highlight.y = currentlySelectedLabel.y;
            this.AddChild(highlight);
            currentlySelectedLabel.MoveToFront();
        }
    }

    public override void OnExit()
    {
        if (this.itemDescBox != null)
        {
            this.stage.RemoveChild(this.itemDescBox);
        }
        base.OnExit();
    }

    public override void OnUpdate()
    {
        base.OnUpdate();

        if (ItemIsSelected) { return; }

        bool highlighted = false;

        if (labels.Count() > 0)
        {
            for (int i = 0; i < labels.Count(); i++)
            {

                FLabel currentLabel = labels[i];
                Vector2 mousePos = currentLabel.GetLocalMousePosition();
                Rect checkRect = new Rect(currentLabel.textRect.x, currentLabel.textRect.y, TextAreaWidth, currentLabel.textRect.height);
                if (checkRect.Contains(mousePos))
                {
                    highlighted = true;
                    currentlySelectedLabel = currentLabel;
                    currentlySelectedIndex = i;
                    break;
                }
            }

            //if nothing is being highlighted by the mouse, we can check for keyboard input
            if (!highlighted)
            {
                bool indexWasChanged = false;

                if (Input.GetKeyDown("down"))
                {
                    currentlySelectedIndex++;
                    if (currentlySelectedIndex >= labels.Count())
                    {
                        currentlySelectedIndex = 0;
                    }
                    indexWasChanged = true;
                }

                if (Input.GetKeyDown("up"))
                {
                    currentlySelectedIndex--;
                    if (currentlySelectedIndex < 0)
                    {
                        currentlySelectedIndex = labels.Count() - 1;
                    }
                    indexWasChanged = true;
                }

                if (indexWasChanged)
                {
                    currentlySelectedLabel = labels[currentlySelectedIndex];
                }
            }

            if (previouslySelectedLabel == null || previouslySelectedLabel != currentlySelectedLabel)
            {

                previouslySelectedLabel = currentlySelectedLabel;

                highlight.x = currentlySelectedLabel.x;
                highlight.y = currentlySelectedLabel.y;
                highlight.height = currentlySelectedLabel.textRect.height;
                currentlySelectedLabel.MoveToFront();

                if (showNodeDescriptions)
                {
                    TreeNode<MenuNode> currentlyHighlightedItem = (TreeNode<MenuNode>)currentlySelectedLabel.data;
                    string descText = currentlyHighlightedItem.Value.NodeDescription;
                    this.itemDescBox.SetText(descText);
                    this.itemDescBox.y = currentlySelectedLabel.y - (this.itemDescBox.Height/2);
                    this.stage.AddChild(itemDescBox);
                }
            }

        }

        if (Input.GetKeyDown("space"))
        {
            //select currently highlighted item
            SelectedItem = (TreeNode<MenuNode>)currentlySelectedLabel.data;
            ItemIsSelected = true;
        }
    }

    public override void HandleMultiTouch(FTouch[] touches)
    {
        base.HandleMultiTouch(touches);
               
        foreach (FTouch touch in touches)
        {
            if (ItemIsSelected) { return; }

            if (touch.phase == TouchPhase.Began)
            {
                foreach (FLabel label in labels)
                {
                    Vector2 touchPos = label.GlobalToLocal(touch.position);
                    Rect checkRect = new Rect(label.textRect.x, label.textRect.y, TextAreaWidth, label.textRect.height);
                    if (checkRect.Contains(touchPos))
                    {
                        //label.data is used to store the un-altered choice text. label.text may include additional linebreaks
                        SelectedItem = (TreeNode<MenuNode>)label.data;
                        ItemIsSelected = true;
                    }
                }
            }

        }
    }

    public void Reset()
    {
        this.SelectedItem = null;
        this.ItemIsSelected = false;
    }
}


