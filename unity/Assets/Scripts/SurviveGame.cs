using UnityEngine;
using System.Collections;

public class SurviveGame : MonoBehaviour {
    
    //FTmxMap background;    

    FSceneManager sceneManager;
	// Use this for initialization
	void Start () {

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

        //Debug.Log("Player position = " + player.GetPosition());
        //Debug.Log("Half Width = " + Futile.screen.halfWidth + " | Half Height = " + Futile.screen.halfHeight);

        
	}
	
	// Update is called once per frame
    void Update()
    {

        
    }

}
