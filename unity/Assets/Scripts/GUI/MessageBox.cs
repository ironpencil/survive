using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class MessageBox : FContainer
{

    private string messageText { get; set; }

    public float Width { get; set; }

    public float Height { get; set; }

    private float TextAreaWidth { get; set; }

    private float TextAreaHeight { get; set; }

    List<FLabel> labels = new List<FLabel>();
    List<string> labelText = new List<string>();

    private int displayedLabelIndex = 0;
    private FLabel displayedLabel;

    private FSprite background;
    private FSprite foreground;

    public float textAreaOffset = 20;
    
    public MessageBox(string messageText, float width, float height)
    {
        this.Width = width;
        this.Height = height;
        TextAreaWidth = width-20;
        TextAreaHeight = height-20;

        //if (backgroundAsset.Length > 0)
        //{
            background = new FSprite(GameVars.Instance.MENU_BACKGROUND_ASSET);            
            background.width = this.Width;
            background.height = this.Height;
            background.color = GameVars.Instance.MENU_BACKGROUND_COLOR;
            this.AddChild(background);            
        //}

        //if (foregroundAsset.Length > 0)
        //{
            foreground = new FSprite(GameVars.Instance.MENU_FOREGROUND_ASSET);
            foreground.width = TextAreaWidth;
            foreground.height = TextAreaHeight;
            foreground.color = GameVars.Instance.MENU_FOREGROUND_COLOR;
            this.AddChild(foreground);
        //}

        SetText(messageText);
        IPDebug.Log("Message Text = " + messageText);

        //displayedLabel = new FLabel(GameVars.Instance.FONT_NAME, labelText.ElementAt(displayedLabelIndex));
        this.AddChild(displayedLabel);
    }

    //Next() returns true if a new label was displayed
    //Will return true when you display the last label, but not the next time you call it
    public bool Next()
    {
        displayedLabelIndex++;

        if (displayedLabelIndex < labelText.Count())
        {
            //this.RemoveChild(displayedLabel);
            displayedLabel.text = labelText.ElementAt(displayedLabelIndex);
            //this.AddChild(displayedLabel);
            return true;
        }
        else
        {
            //don't auto-close anymore, can call Close() to remove label or just remove the whole box
            //this.RemoveChild(displayedLabel);            
            return false;
        }
    }

    public void Close()
    {
        this.RemoveChild(displayedLabel);
    }

    public string GetAllText()
    {
        return messageText;
    }

    public string GetDisplayedText()
    {
        return displayedLabel.text;
    }

    public void SetText(string messageText)
    {
        this.messageText = messageText;
        SetupLabels();
    }

    private void SetupLabels()
    {
        
        //StringBuilder sb = new StringBuilder();

        FLabel currentLabel = new FLabel(GameVars.Instance.FONT_NAME, "");

        string currentText = "";        
        string newLine = "\n";
        Vector2 size = Vector2.zero;
        
        //this.RemoveAllLabels();
        string[] lines = messageText.Split(newLine.ToCharArray());

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
                    newText = currentText + newLine + word;
                    IPDebug.Log("newText new line = " + newText);
                    size = currentLabel.MeasureText(newText);
                    if (size.y <= TextAreaHeight)
                    {
                        currentText = newText;
                    }
                    else
                    {
                        IPDebug.Log("word doesn't fit, new label");
                        //word doesn't fit, add new label with current text to list, put new word on new label
                        //FLabel newLabel = new FLabel(GameVars.Instance.FONT_NAME, currentLabel.text);
                        labelText.Add(currentText);
                        //labels.Add(newLabel);
                        currentText = word;
                    }
                }

                space = " "; //update space string to actually contain a space after the first time through
            }

            //at the end of a line, see if we can start the next line on the current label or if we have to start a new one

            size = currentLabel.MeasureText(currentText + newLine + " ");
            if (size.y <= TextAreaHeight)
            {
                currentText += newLine;
            }
            else
            {
                labelText.Add(currentText);
                currentText = "";
            }
        }

        //add the last updated label text
        //labels.Add(currentLabel);
        if (currentText.Length > 0)
        {
            labelText.Add(currentText);
        }

        displayedLabelIndex = 0;
        displayedLabel = new FLabel(GameVars.Instance.FONT_NAME, labelText.ElementAt(displayedLabelIndex));
        displayedLabel.anchorX = 0.0f; //left
        displayedLabel.anchorY = 1.0f; //top
        displayedLabel.x = -(this.Width / 2) + textAreaOffset;
        displayedLabel.y = (this.Height / 2) - textAreaOffset; // *0.9f;
        //this.AddChild(displayedLabel);
    }

    public string GetLabelsDescription()
    {
        StringBuilder sb = new StringBuilder();

        IPDebug.Log("Label Count = " + labels.Count);
        foreach (FLabel label in labels)
        {
            IPDebug.Log("Label Text = " + label.text);
            sb.AppendLine("Label Text:\r\n" + label.text);
        }

        return sb.ToString();
    }
}


