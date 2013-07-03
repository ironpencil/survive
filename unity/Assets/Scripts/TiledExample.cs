using UnityEngine;
using System.Collections;

public class TiledExample : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        FutileParams fparams = new FutileParams(true, true, false, false);
        fparams.AddResolutionLevel(960.0f, 2.0f, 1.0f, "");
        fparams.origin = new Vector2(0.5f, 0.5f);
        fparams.backgroundColor = new Color(0.15f, 0.15f, 0.3f);
        Futile.instance.Init(fparams);

        // load image atlas (within Resources/Atlases folder)
        Futile.atlasManager.LoadAtlas("Atlases/survive");

        // Add tilemap 
        FTmxMap tmx1 = new FTmxMap();        
        tmx1.x = -120;
        tmx1.y = 80;
        tmx1.LoadTMX("CSVs/forestMapLarge"); // load tmx text file (within Resources/CSVs folder)
        Futile.stage.AddChild(tmx1);

        FSprite white1 = new FSprite("Futile_White");
        FSprite white2 = new FSprite("Futile_White");

        white2.color = new Color(255.0f, 0.0f, 0.0f);

        tmx1.AddChild(white1);
        white1.anchorX = 0;
        white1.anchorY = 1;
        white1.x = 0;
        white1.y = 0;
        Futile.stage.AddChild(white2);
        white2.x = 5;
        white2.y = 5;

        // load font atlas
        Futile.atlasManager.LoadAtlas("Atlases/Fonts");

        // Add large font textd
        Futile.atlasManager.LoadFont("Large", "Large Font", "Atlases/Large Font", 0.0f, 0.0f);
        FLabel label1 = new FLabel("Large", "LARGE FONT");
        label1.y = 26;
        Futile.stage.AddChild(label1);

        // Add small font text
        Futile.atlasManager.LoadFont("Small", "Small Font", "Atlases/Small Font", 0.0f, 0.0f);
        FLabel label2 = new FLabel("Small", "Small Font");
        label2.y = 12;
        Futile.stage.AddChild(label2);

        // Add tiny font text
        Futile.atlasManager.LoadFont("Tiny", "Tiny Font", "Atlases/Tiny Font", 0.0f, 0.0f);
        FLabel label3 = new FLabel("Tiny", "Tiny Font");
        label3.y = 3;
        Futile.stage.AddChild(label3);
    }

    // Update is called once per frame
    void Update()
    {

    }

}
