// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Title : MonoBehaviour {

    private AudioSource audioSource;
    public AudioClip title_bgm;
    public GUIStyle style;
    private bool  start_sw;

    void  OnGUI (){
    //	GUI.Label( new Rect((Screen.width-300)/2, (Screen.height-100)*7/10+100, 300, 100), "JESTER PRESENTS", style);
	    if(start_sw)
    		GUI.Label( new Rect((Screen.width-300)/2, Screen.height*9/10, 300, 100), "PLEASE WAIT", style);
	    else
		    if(Time.frameCount % 120 < 60)
#if UNITY_ANDROID || UNITY_IOS
        GUI.Label( new Rect((Screen.width-300)/2, Screen.height*9/10, 300, 100), "TAP TO START", style);
#else
	    GUI.Label( new Rect((Screen.width-300)/2, Screen.height*9/10, 300, 100), "CLICK MOUSE BUTTON", style);
#endif
    }

    static int play_count;
    public float time;
//private AsyncOperation async;

    private GameObject message;

    void  Start (){
	    if(Game.BGM_sw) {
            audioSource = gameObject.GetComponent<AudioSource>();
            audioSource.loop=true;
		    audioSource.clip = title_bgm;
		    audioSource.Play();
	    }
//	async = SceneManagement.SceneManager.LoadSceneAsync("Start",SceneManagement.LoadSceneMode.Single);
//	async.allowSceneActivation = false; 
    	start_sw = false;
	    time = 0.0f;
    }

    void  Update (){
    	if(Input.GetMouseButtonDown(0)) {
    //		SceneManagement.SceneManager.UnloadScene("Title");
    		start_sw = true;
    		SceneManager.LoadScene("StartGame");
    //		async.allowSceneActivation = true;
    //		SceneManagement.SceneManager.LoadScene("TopTen");
    //		SceneManagement.SceneManager.LoadScene("UnityAds");
	    }
    	if(Input.GetKey(KeyCode.Escape))
	    	Application.Quit();
	    time += Time.deltaTime;
	    if(Input.GetKey(KeyCode.T) || time>120.0f)
	    	SceneManager.LoadScene("TopTen");
    }   

}