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

    public MessageBox(string messageText, float textAreaWidth, float textAreaHeight)
    {
        TextAreaWidth = textAreaWidth;
        TextAreaHeight = textAreaHeight;
        SetText(messageText);
        IPDebug.Log("Message Text = " + messageText);

        //displayedLabel = new FLabel("ComicSans", labelText.ElementAt(displayedLabelIndex));
        this.AddChild(displayedLabel);
    }

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
            this.RemoveChild(displayedLabel);
            return false;
        }
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
        //this.RemoveAllLabels();

        string[] words = messageText.Split(' ');

        IPDebug.Log("words length = " + words.Length);
        //StringBuilder sb = new StringBuilder();
        
        FLabel currentLabel = new FLabel("ComicSans", "");

        string currentText = "";
        string space = ""; // don't start with a space
        string newLine = "\r\n";

        foreach (string word in words)
        {
            string newText = currentText + space + word;
            IPDebug.Log("newText = " + newText);
            Vector2 size = currentLabel.MeasureText(newText);

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
                    //FLabel newLabel = new FLabel("ComicSans", currentLabel.text);
                    labelText.Add(currentText);
                    //labels.Add(newLabel);
                    currentText = word;
                }
            }

            space = " "; //update space string to actually contain a space after the first time through
        }

        //add the last updated label text
        //labels.Add(currentLabel);
        labelText.Add(currentText);

        displayedLabelIndex = 0;
        displayedLabel = new FLabel("ComicSans", labelText.ElementAt(displayedLabelIndex));
        //this.AddChild(displayedLabel);
    }

    private void RemoveAllLabels()
    {
        foreach (FLabel label in labels)
        {
            if (label.container == this)
            {
                this.RemoveChild(label);
            }
        }
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


