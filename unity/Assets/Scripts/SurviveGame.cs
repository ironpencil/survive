using UnityEngine;
using System.Collections;

public class SurviveGame : MonoBehaviour {
    
    //FTmxMap background;    

    FSceneManager sceneManager;
    FStage fpsStage;
    HUDFPS fps;
    bool showFPS = false;
    //FStage guiStage;

    public static bool ALLOW_DEBUG = true;
    


    // Use this for initialization
	void Start () {

        Screen.SetResolution(960, 640, false);

        //IPDebug.DoLog = true;

        FutileParams fparams = new FutileParams(true, true, false, false);
        fparams.shouldLerpToNearestResolutionLevel = false;
        fparams.AddResolutionLevel(960.0f, 1.0f, 1.0f, "");
        fparams.origin = new Vector2(0.5f, 0.5f);
        fparams.backgroundColor = Color.black;
        Futile.instance.Init(fparams);
        Futile.instance.shouldTrackNodesInRXProfiler = false;

        

        // load image atlas (within Resources/Atlases folder)
        Futile.atlasManager.LoadAtlas("Atlases/survive");

        Futile.atlasManager.LoadFont(GameVars.Instance.FONT_NAME, "comic-sans", "Atlases/comic-sans", 0.0f, 0.0f);

        GameVars.Instance.GUIStage = new FStage("GUI");
        GameVars.Instance.FadeStage = new FStage("Fader");

        //used to fade between scenes
        FSprite fadeSprite = new FSprite("Futile_White");
        fadeSprite.width = Futile.screen.width;
        fadeSprite.height = Futile.screen.height;
        fadeSprite.color = Color.black;
        GameVars.Instance.FadeStage.AddChild(fadeSprite);
        GameVars.Instance.FadeStage.alpha = 0.0f;

        GameData.Instance.LoadData();

        sceneManager = FSceneManager.Instance;

        //FWorldScene gameScene = new FWorldScene("world");

        FSceneManager.Instance.PushScene(new FDisclaimerScene("Disclaimer"));
        
        //IPDebug.Log("Player position = " + player.GetPosition());
        //IPDebug.Log("Half Width = " + Futile.screen.halfWidth + " | Half Height = " + Futile.screen.halfHeight);               

        //guiStage = new FStage("GUI");


        fpsStage = new FStage("FPSHud");
        fps = new HUDFPS(GameVars.Instance.FONT_NAME);
        fps.scale = 0.5f;
        fps.anchorX = 0;
        fps.anchorY = 1;
        fps.x = -Futile.screen.halfWidth;
        fps.y = Futile.screen.halfHeight;
        fpsStage.AddChild(fps);
        showFPS = false;

        IPDebug.DoLog = true;
        IPDebug.DoLog = false;
        IPDebug.ForceAllowed = true;

        if (showFPS)
        {
            Futile.AddStage(fpsStage);
        }
	}
	
	// Update is called once per frame
    void Update()
    {        
        Futile.AddStage(GameVars.Instance.GUIStage);
        Futile.AddStage(GameVars.Instance.FadeStage);

        if (showFPS)
        {
            Futile.AddStage(fpsStage);
        }

        if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus))
        {
            GameVars.Instance.MUSIC_VOLUME = GameVars.Instance.MUSIC_VOLUME - GameVars.Instance.MUSIC_VOLUME_CHANGE_INTERVAL;
            Debug.Log("Lowering volume to : " + GameVars.Instance.MUSIC_VOLUME);
        }

        if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus) ||
            (Input.GetKeyDown(KeyCode.Equals) && 
            (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))))
        {
            GameVars.Instance.MUSIC_VOLUME = GameVars.Instance.MUSIC_VOLUME + GameVars.Instance.MUSIC_VOLUME_CHANGE_INTERVAL;
            Debug.Log("Raising volume to : " + GameVars.Instance.MUSIC_VOLUME);
        }
    }   

}
