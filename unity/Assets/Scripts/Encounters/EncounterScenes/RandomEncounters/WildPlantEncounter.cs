using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class WildPlantEncounter : FEncounterScene
{

    private const string CHOICE1_SAFE = "Choice1_A";
    private const string CHOICE1_NOT_SAFE = "Choice1_B";

    string plantName = "";
    string plantDescription = "";
    string plantInformation = "";
    string plantImage = "";
    bool plantIsSafe = true;

    public WildPlantEncounter(string _name = "WILD_PLANT")
        : base(_name)
    {

    }

    protected override void HandleResult()
    {
        MenuNode selectedNode = FSelectionDisplayScene.GetLastResultNode(this.resultNode).Value;

        switch (selectedNode.NodeTitle)
        {
            case CHOICE1_SAFE: //result of eating plant
                if (plantIsSafe)
                {
                    GameVars.Instance.Player.WildernessPoints += 5;
                    GameVars.Instance.Player.Energy += UnityEngine.Random.Range(5, 11);
                } else {
                    GameVars.Instance.Player.WildernessPoints -= 5;
                    GameVars.Instance.Player.Energy -= UnityEngine.Random.Range(5, 11);
                }
                break;
            case CHOICE1_NOT_SAFE: //result of not eating plant
                if (plantIsSafe)
                {
                    GameVars.Instance.Player.WildernessPoints -= 5;                    
                }
                else
                {
                    GameVars.Instance.Player.WildernessPoints += 5;
                }
                break;
            default: // item usage                
                break;
        }

        this.ShouldPop = true;
    }    

    protected override void HandleCancel()
    {
        IPDebug.Log("Selection cancelled");
    }

    public override void OnEnter()
    {
        GeneratePlantType();

        string questionSuffix = "\nShould you eat this plant?";
        MenuNode rootMenu = new MenuNode(MenuNodeType.TEXT, this.Name, this.Name, plantDescription + questionSuffix);

        if (plantImage.Length > 0)
        {
            rootMenu.DisplayImageAsset = plantImage;
        }

        TreeNode<MenuNode> rootNode = new TreeNode<MenuNode>(rootMenu);

        string safePrefix = "";
        string unsafePrefix = "";
        if (plantIsSafe)
        {
            safePrefix = "Correct! ";
            unsafePrefix = "Incorrect! ";
        }
        else
        {
            safePrefix = "Incorrect! ";
            unsafePrefix = "Correct! ";
        }

        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_SAFE, "Yes", safePrefix + plantInformation)));
        rootNode.AddChild(new TreeNode<MenuNode>(new MenuNode(MenuNodeType.TEXT, CHOICE1_NOT_SAFE, "No", unsafePrefix + plantInformation)));

        this.currentScene = new FSelectionDisplayScene(this.Name, rootNode);
        FSceneManager.Instance.PushScene(this.currentScene);
    }

    private void GeneratePlantType()
    {
        WildPlantTypes[] plantTypes = (WildPlantTypes[]) Enum.GetValues(typeof(WildPlantTypes));

        int plantMaxIndex = plantTypes.Length;

        int plantTypeIndex = UnityEngine.Random.Range(0, plantMaxIndex);

        WildPlantTypes plantType = plantTypes[plantTypeIndex];

        switch (plantType)
        {
            case WildPlantTypes.DOLLS_EYE:
                plantName = "Doll's Eye";
                plantImage = "dolls_eye";
                plantDescription = "You come across a plant with small white berries topped with a single black dot, giving them the appearance of eyeballs.";
                plantInformation = "This is the " + plantName + " plant, and its berries have a flavor reminiscent of warm milk before bed while listening to a story.";
                break;
            case WildPlantTypes.ANGEL_TRUMPET:
                plantName = "Angel's Trumpet";
                plantImage = "angel_trumpet";
                plantDescription = "You come across a plant with pendulous trumpet-shaped flowers covered in fine hairs hanging from the branches.";
                plantInformation = "This is the " + plantName + " plant, and its flowers have a flavor reminiscent of a child's wonder on Christmas morning.";
                break;
            case WildPlantTypes.CASTOR_OIL:
                plantName = "Castor";
                plantImage = "castor";
                plantDescription = "You come across a plant with glossy green leaves with soft, spiny, reddish-brown capsule seeds.";
                plantInformation = "This is the " + plantName + " plant, and its seeds have a flavor reminiscent of chocolate-covered raisins at a movie on a Friday night.";
                break;
            case WildPlantTypes.ROSARY_PEA:
                plantName = "Rosary Pea";
                plantImage = "rosary_pea";
                plantDescription = "You come across a plant with bright red berries with a single black dot where the stem is attached.";
                plantInformation = "This is the " + plantName + " plant, and its berries have a flavor reminiscent of strawberry shortcake topped with cream.";
                break;
            case WildPlantTypes.MONKSHOOD:
                plantName = "Monkshood";
                plantImage = "monkshood";
                plantDescription = "You come across a plant with dark green leaves with tall, erect stems crowned with large purple flowers.";
                plantInformation = "This is the " + plantName + " plant, and its flowers have a flavor reminiscent of gummy blue sharks from the convenience store by the community pool.";
                break;
            case WildPlantTypes.HEMLOCK:
                plantName = "Cicuta";
                plantImage = "hemlock";
                plantDescription = "You come across a plant with a green main stem with purple splotches and small white flowers in an umbrella-shaped cluster.";
                plantInformation = "This is the " + plantName + " plant, and its flowers have a flavor reminiscent of fondly recalling your high school prom night.";
                break;
            case WildPlantTypes.ENGLISH_YEW:
                plantName = "English Yew";
                plantImage = "english_yew";
                plantDescription = "You come across a medium-sized evergreen tree with bright red berries that are wide and open at the end.";
                plantInformation = "This is the " + plantName + ", and its berries have a flavor reminiscent of the rich smell of the leather books in your father's study.";
                break;
            case WildPlantTypes.MOONSEED:
                plantName = "Moonseed";
                plantImage = "moonseed";
                plantDescription = "You come across a climbing vine with small, drooping white flowers and clusters of red berries.";
                plantInformation = "This is the " + plantName + " plant, and its berries have a flavor reminiscent of your dearest grandmother's berry pie.";
                break;
            case WildPlantTypes.OLEANDER:
                plantName = "Oleander";
                plantImage = "oleander";
                plantDescription = "You come across a shrub with thick leathery dark-green leaves and sweet-smelling white flower clusters.";
                plantInformation = "This is the " + plantName + " plant, and its flowers have a flavor reminiscent of clean sheets drying on the line on a summer afternoon.";
                break;
            case WildPlantTypes.POKEWEED:
                plantName = "Pokeweed";
                plantImage = "pokeweed";
                plantDescription = "You come across a plant with a red stem, large, simple leaves, white flowers and dark purple berries.";
                plantInformation = "This is the " + plantName + ", and its berries have a flavor reminiscent of grape soda at a family reunion.";
                break;
            default:
                break;
        }
    }

}
