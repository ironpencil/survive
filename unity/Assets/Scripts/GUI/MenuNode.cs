using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class MenuNode
{
    public MenuNodeType NodeType { get; private set; }

    public string NodeTitle { get; set; }

    public string NodeText { get; private set; }

    public string DisplayMessage { get; private set; }

    public MenuNode(MenuNodeType nodeType, string nodeTitle) : this(nodeType, nodeTitle, "") { }    

    public MenuNode(MenuNodeType nodeType, string nodeTitle, string nodeText) : this(nodeType, nodeTitle, nodeText, "") {}

    public MenuNode(MenuNodeType nodeType, string nodeTitle, string nodeText, string displayMessage)
    {
        NodeType = nodeType;
        NodeText = nodeText;
        DisplayMessage = displayMessage;
    }
    
}
