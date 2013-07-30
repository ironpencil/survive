using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MenuNode
{
    public MenuNodeType NodeType { get; private set; }

    public List<IGameEvent> OnSelectionEvents = new List<IGameEvent>();

    public string NodeTitle { get; set; }

    public string NodeText { get; set; }

    public string DisplayMessage { get; set; }

    public string NodeDescription { get; set; }

    public string DisplayImageAsset { get; set; }

    public AfterSelectionBehavior AfterSelection { get; set; }

    public float ParentAlphaWhenSelected { get; private set; }

    public MenuNode(MenuNodeType nodeType, string nodeTitle) : this(nodeType, nodeTitle, "") { }    

    public MenuNode(MenuNodeType nodeType, string nodeTitle, string nodeText) : this(nodeType, nodeTitle, nodeText, "") {}

    public MenuNode(MenuNodeType nodeType, string nodeTitle, string nodeText, string displayMessage) : this(nodeType, nodeTitle, nodeText, displayMessage, "") {}

    public MenuNode(MenuNodeType nodeType, string nodeTitle, string nodeText, string displayMessage, string nodeDescription)
    {
        this.NodeType = nodeType;
        this.NodeTitle = nodeTitle;
        this.NodeText = nodeText;
        this.DisplayMessage = displayMessage;
        this.NodeDescription = nodeDescription;
        this.AfterSelection = AfterSelectionBehavior.CLOSE_PARENT;
        this.ParentAlphaWhenSelected = 1.0f;
        this.DisplayImageAsset = "";
    }

    public enum AfterSelectionBehavior {
        CLOSE_PARENT,
        CLOSE_ALL,
        KEEP_PARENT_OPEN
    }

    public static MenuNode CreateChoiceNode(string title, string text, string message, string description)
    {
        MenuNode node = new MenuNode(MenuNodeType.TEXT, title, text, message, description);
        node.AfterSelection = AfterSelectionBehavior.CLOSE_PARENT;
        node.ParentAlphaWhenSelected = 1.0f;
        return node;
    }
    
    public static MenuNode CreateConfirmationNode(string title, string text, string message, string description) {

        MenuNode node = new MenuNode(MenuNodeType.TEXT, title, text, message, description);
        node.AfterSelection = AfterSelectionBehavior.KEEP_PARENT_OPEN;
        node.ParentAlphaWhenSelected = 1.0f;
        return node;
    }

    public static MenuNode CreateOKNode(string title, string text, string message, string description)
    {
        MenuNode node = new MenuNode(MenuNodeType.TEXT, title, text, message, description);
        node.AfterSelection = AfterSelectionBehavior.CLOSE_ALL;
        node.ParentAlphaWhenSelected = 1.0f;
        return node;
    }
}
