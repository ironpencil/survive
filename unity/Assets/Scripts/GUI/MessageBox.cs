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

    private int displayedLabelIndex = 0;
    private FLabel displayedLabel;

    public MessageBox(string messageText, float textAreaWidth, float textAreaHeight)
    {
        TextAreaWidth = textAreaWidth;
        TextAreaHeight = textAreaHeight;
        SetText(messageText);
        Debug.Log("Message Text = " + messageText);

        displayedLabel = labels.ElementAt(displayedLabelIndex);
        this.AddChild(displayedLabel);
    }

    public bool Next()
    {
        displayedLabelIndex++;

        if (displayedLabelIndex < labels.Count())
        {
            this.RemoveChild(displayedLabel);
            displayedLabel = labels.ElementAt(displayedLabelIndex);
            this.AddChild(displayedLabel);
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
        this.RemoveAllLabels();

        string[] words = messageText.Split(' ');

        Debug.Log("words length = " + words.Length);
        //StringBuilder sb = new StringBuilder();

        FLabel currentLabel = new FLabel("ComicSans", "");

        string space = ""; // don't start with a space
        string newLine = "\r\n";

        foreach (string word in words)
        {
            string newText = currentLabel.text + space + word;
            Debug.Log("newText = " + newText);
            Vector2 size = currentLabel.MeasureText(newText);

            if (size.x <= TextAreaWidth)
            {
                currentLabel.text = newText;
            }
            else
            {                
                newText = currentLabel.text + newLine + word;
                Debug.Log("newText new line = " + newText);
                size = currentLabel.MeasureText(newText);
                if (size.y <= TextAreaHeight)
                {
                    currentLabel.text = newText;
                }
                else
                {
                    Debug.Log("word doesn't fit, new label");
                    //word doesn't fit, add new label with current text to list, put new word on new label
                    FLabel newLabel = new FLabel("ComicSans", currentLabel.text);
                    labels.Add(newLabel);
                    currentLabel.text = word;
                }
            }

            space = " "; //update space string to actually contain a space after the first time through
        }

        //add the last updated word
        labels.Add(currentLabel);

        displayedLabelIndex = 0;
        displayedLabel = labels.ElementAt(displayedLabelIndex);
        this.AddChild(displayedLabel);
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

        Debug.Log("Label Count = " + labels.Count);
        foreach (FLabel label in labels)
        {
            Debug.Log("Label Text = " + label.text);
            sb.AppendLine("Label Text:\r\n" + label.text);
        }

        return sb.ToString();
    }
}


