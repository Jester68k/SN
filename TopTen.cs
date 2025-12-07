// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TopTen : MonoBehaviour {

public float time;
public GUIStyle style, style2, style3;
private int mode, width, height;

void  OnGUI (){
	width = Screen.width;
	height = Screen.height;
#if (UNITY_ANDROID || UNITY_IOS)
	if(mode==0)
		GUI.Label(new Rect(200 * width / 500, 60 * height / 900, 100 * width / 500, 60), "GAME A", style);
	else
        GUI.Label(new Rect(200 * width / 500, 60 * height / 900, 100 * width / 500, 60), "GAME B", style);
    GUI.Label( new Rect(20*width/500,110*height/900, 100*width/500, 60), "RANK", style);
	GUI.Label( new Rect(140*width/500,110*height/900, 250*width/500, 60), "SCORE", style);
	GUI.Label(new Rect(380 * width / 500, 110 * height / 900, 100 * width / 500, 60), "CHAIN", style);
        for (int i=0; i<10; i++) {
		GUI.Label( new Rect(20*width/500,(110+64)*height/900+i*(64*height/900), 100*width/500, 60), ""+(i+1), style);
		if(Game.top_ten[mode,i]>0)
			GUI.Label( new Rect(100*width/500,(110+64)*height/900+i*(64*height/900), 250*width/500, 60), ""+Game.top_ten[mode,i], style2);
		if(Game.top_ten_chain[mode,i]>0)
			GUI.Label( new Rect(380*width/500,(110+64)*height/900+i*(64*height/900), 100, 60), ""+Game.top_ten_chain[mode,i], style2);
	}
#else
        if (mode == 0)
            GUI.Label(new Rect(200 * width / 500, 120 * height / 900, 100 * width / 500, 60), "SOLO", style3);
        else
            GUI.Label(new Rect(200 * width / 500, 120 * height / 900, 100 * width / 500, 60), "VERSUS", style3);
        GUI.Label(new Rect(20 * width / 500, 170 * height / 900, 100 * width / 500, 60), "RANK", style3);
        GUI.Label(new Rect(140 * width / 500, 170 * height / 900, 250 * width / 500, 60), "SCORE", style);
        GUI.Label(new Rect(380 * width / 500, 170 * height / 900, 100 * width / 500, 60), "CHAIN", style);
        for (int i = 0; i < 10; i++)
        {
            GUI.Label(new Rect(20 * width / 500, 170 * height / 900 + (i+1) * (64 * height / 900), 100 * width / 500, 60), "" + (i + 1), style3);
            if (Game.top_ten[mode, i] > 0)
                GUI.Label(new Rect(140 * width / 500, (170 + 64) * height / 900 + i * (64 * height / 900), 250 * width / 500, 60), "" + Game.top_ten[mode, i], style2);
            if (Game.top_ten_chain[mode, i] > 0)
                GUI.Label(new Rect(380 * width / 500, (170 + 64) * height / 900 + i * (64 * height / 900), 100, 60), "" + Game.top_ten_chain[mode, i], style2);
        }
#endif
    }

    void  Start (){
	time = 0.0f;
	mode = 0;
}

void  Update (){
	time += Time.deltaTime;
		if (time > 10.0f)
			mode = 1;
	if(Input.GetMouseButtonDown(0) || Input.GetKey(KeyCode.Escape) || time>20.0f)
		SceneManager.LoadScene("Title");
}

}