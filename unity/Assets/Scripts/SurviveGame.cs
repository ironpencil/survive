using UnityEngine;
using System.Collections;

public class SurviveGame : MonoBehaviour {
    
    //FTmxMap background;    

    FSceneManager sceneManager;
    FStage fpsStage;
    HUDFPS fps;
    bool showFPS = false;
    //FStage guiStage;


    


    // Use this for initialization
	void Start () {

        //IPDebug.DoLog = true;

        FutileParams fparams = new FutileParams(true, true, false, false);
        fparams.AddResolutionLevel(960.0f, 1.0f, 1.0f, "");
        fparams.origin = new Vector2(0.5f, 0.5f);
        fparams.backgroundColor = Color.black;
        Futile.instance.Init(fparams);

        // load image atlas (within Resources/Atlases folder)
        Futile.atlasManager.LoadAtlas("Atlases/survive");

        Futile.atlasManager.LoadFont(GameVars.Instance.FONT_NAME, "comic-sans", "Atlases/comic-sans", 0.0f, 0.0f);

        GameVars.Instance.GUIStage = new FStage("GUI");

        GameData.Instance.LoadData();

        sceneManager = FSceneManager.Instance;

        //FWorldScene gameScene = new FWorldScene("world");

        FSceneManager.Instance.PushScene(new FDisclaimerScene("Disclaimer"));
        
        //IPDebug.Log("Player position = " + player.GetPosition());
        //IPDebug.Log("Half Width = " + Futile.screen.halfWidth + " | Half Height = " + Futile.screen.halfHeight);               

        //guiStage = new FStage("GUI");


        //fpsStage = new FStage("FPSHud");
        //fps = new HUDFPS(GameVars.Instance.FONT_NAME);
        //fps.scale = 0.5f;
        //fps.anchorX = 0;
        //fps.anchorY = 1;
        //fps.x = -Futile.screen.halfWidth;
        //fps.y = Futile.screen.halfHeight;
        //fpsStage.AddChild(fps);
        //showFPS = true;

        IPDebug.DoLog = true;
        IPDebug.DoLog = false;
        IPDebug.ForceAllowed = true;

        //Futile.AddStage(fpsStage);
	}
	
	// Update is called once per frame
    void Update()
    {        
        Futile.AddStage(GameVars.Instance.GUIStage);

        //if (showFPS)
        //{
        //    Futile.AddStage(fpsStage);
        //}
    }   

}
