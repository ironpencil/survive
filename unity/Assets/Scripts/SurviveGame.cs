using UnityEngine;
using System.Collections;

public class SurviveGame : MonoBehaviour {
    
    //FTmxMap background;    

    FSceneManager sceneManager;
    HUDFPS fps;
    //FStage guiStage;

	// Use this for initialization
	void Start () {

        //IPDebug.DoLog = true;

        FutileParams fparams = new FutileParams(true, true, false, false);
        fparams.AddResolutionLevel(960.0f, 1.0f, 1.0f, "");
        fparams.origin = new Vector2(0.5f, 0.5f);
        fparams.backgroundColor = Color.magenta;
        Futile.instance.Init(fparams);

        // load image atlas (within Resources/Atlases folder)
        Futile.atlasManager.LoadAtlas("Atlases/survive");

        Futile.atlasManager.LoadFont("ComicSans", "comic-sans", "Atlases/comic-sans", 0.0f, 0.0f);

        sceneManager = FSceneManager.Instance;

        FWorldScene gameScene = new FWorldScene("world");

        sceneManager.PushScene(gameScene);
        
        //IPDebug.Log("Player position = " + player.GetPosition());
        //IPDebug.Log("Half Width = " + Futile.screen.halfWidth + " | Half Height = " + Futile.screen.halfHeight);               

        //guiStage = new FStage("GUI");
        fps = new HUDFPS("ComicSans");
        fps.scale = 0.5f;
        fps.anchorX = 0;
        fps.anchorY = 1;
        //fps.x = -Futile.screen.halfWidth;
        //fps.y = Futile.screen.halfHeight;
        //guiStage.AddChild(fps);
        Futile.stage.AddChild(fps);
        //Futile.AddStage(guiStage);
	}
	
	// Update is called once per frame
    void Update()
    {
        fps.x = -Futile.stage.x - Futile.screen.halfWidth;
        fps.y = -Futile.stage.y + Futile.screen.halfHeight;
        
    }   

}
