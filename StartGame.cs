// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using UnityEngine.SceneManagement;
// using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;
//using UnityEngine.UIElements;

public class StartGame : MonoBehaviour {

    private AudioSource audioSource;
	public GameObject bgm_off;
	public GameObject bgm1;
	public GameObject bgm2;
	public GameObject bgm3;
	public AudioClip click_se;
	public SpriteRenderer se_off;
	public SpriteRenderer se_on;
	public SpriteRenderer wait_message;
	public GameObject eff_gamestart;
	public GameObject eff_gamestart_tapping;
	public ParticleSystem eff_gamestart_tapping_alpha;
	public bool  tap_flag;
    public ToggleGroup toggleGroup1, toggleGroup2;

    void Start (){
        audioSource = this.GetComponent<AudioSource>();
        if (Game.BGM_sw) {
	   	    audioSource.loop=true;
			audioSource.clip=Game.BGM[Game.BGM_no-1];
			audioSource.Play();
		}
		if(Game.BGM_sw) {
			switch(Game.BGM_no) {
			case 1:
				bgm_off.SetActive(false);
				bgm1.SetActive(true);
				bgm2.SetActive(false);
				bgm3.SetActive(false);
				break;
			case 2:
				bgm_off.SetActive(false);
				bgm1.SetActive(false);
				bgm2.SetActive(true);
				bgm3.SetActive(false);
				break;
			case 3:
				bgm_off.SetActive(false);
				bgm1.SetActive(false);
				bgm2.SetActive(false);
				bgm3.SetActive(true);
				break;
			}
		} else {
			bgm_off.SetActive(true);
			bgm1.SetActive(false);
			bgm2.SetActive(false);
			bgm3.SetActive(false);
		}
		if(Game.se_sw) {
			se_off.GetComponent<SpriteRenderer>().enabled=false;
			se_off.GetComponent<BoxCollider2D>().enabled=false;
			se_on.GetComponent<SpriteRenderer>().enabled=true;
			se_on.GetComponent<BoxCollider2D>().enabled=true;
		} else {
			se_off.GetComponent<SpriteRenderer>().enabled=true;
			se_off.GetComponent<BoxCollider2D>().enabled=true;
			se_on.GetComponent<SpriteRenderer>().enabled=false;
			se_on.GetComponent<BoxCollider2D>().enabled=false;
		}
		wait_message.GetComponent<SpriteRenderer>().enabled = false;
		tap_flag = false;
		eff_gamestart_tapping.SetActive(false);
		eff_gamestart.SetActive(true);
	}

    void  Update (){
	    Vector3 mousePos;
	    Vector2 tapPoint;
	    Collider2D collider2d;
	    GameObject obj;
	    bool  skip_sw;

    	if(Input.GetMouseButtonDown(0)){
    	//マウスの左ボタンを押した
	    //・カーソル位置取得
		    mousePos = Input.mousePosition;
//		    mousePos.z = 10.0ff;
		    tapPoint = GetComponent<Camera>().ScreenToWorldPoint(mousePos);
		    collider2d = Physics2D.OverlapPoint(tapPoint);
			if(collider2d) {
				obj = collider2d.transform.gameObject;
				Debug.Log("object: "+obj.name);
				if(obj.name=="eff_gamestart") {
					tap_flag = true;
					eff_gamestart.SetActive(false);
					eff_gamestart_tapping.SetActive(true);
					audioSource.PlayOneShot(click_se, 1.0f);

                    string selectedLabel;

                    selectedLabel = toggleGroup1.ActiveToggles()
                    .First().GetComponentsInChildren<Text>()
                    .First(t => t.name == "Label").text;

                    Debug.Log("selected " + selectedLabel);
					if (selectedLabel == "TRAINING")
						Game.game_type = 0;
                    if (selectedLabel == "VERSUS")
                        Game.game_type = 1;

                    selectedLabel = toggleGroup2.ActiveToggles()
                        .First().GetComponentsInChildren<Text>()
                        .First(t => t.name == "Label").text;

                    Debug.Log("selected " + selectedLabel);
					if (selectedLabel == "FIRST MOVE")
						Game.move_mode = 0;	// First Move
					if(selectedLabel=="SECOND MOVE")
						Game.move_mode = 1;	// Second Move
                }
                skip_sw = false;
				if(skip_sw==false&&(obj.name=="bgmoff" || (obj.name=="bgm"&&Game.BGM_no==4&&Game.BGM_sw==false))) {
					bgm_off.SetActive(false);
					bgm1.SetActive(true);
					Game.BGM_sw=true;
					Game.BGM_no=1;
					audioSource.loop=true;
					audioSource.clip=Game.BGM[Game.BGM_no-1];
					audioSource.Play();
					skip_sw = true;
				}
				if(skip_sw==false&&(obj.name=="bgm1"|| (obj.name=="bgm"&&Game.BGM_no==1&&Game.BGM_sw==true))) {
					bgm1.SetActive(false);
					bgm2.SetActive(true);
					Game.BGM_sw=true;
					Game.BGM_no=2;
					audioSource.loop=true;
					audioSource.clip=Game.BGM[Game.BGM_no-1];
					audioSource.Play();
					skip_sw = true;
				}
				if(skip_sw==false&&(obj.name=="bgm2"|| (obj.name=="bgm"&&Game.BGM_no==2&&Game.BGM_sw==true))) {
					bgm2.SetActive(false);
					bgm3.SetActive(true);
					Game.BGM_sw=true;
					Game.BGM_no=3;
					audioSource.loop=true;
					audioSource.clip=Game.BGM[Game.BGM_no-1];
					audioSource.clip= Game.BGM[Game.BGM_no-1];
					audioSource.Play();
					skip_sw = true;
				}
				if(skip_sw==false&&(obj.name=="bgm3"|| (obj.name=="bgm"&&Game.BGM_no==3&&Game.BGM_sw==true))) {
					bgm3.SetActive(false);
					bgm_off.SetActive(true);
					Game.BGM_sw=false;
					Game.BGM_no=4;
					audioSource.Stop();
					skip_sw = true;
				}
				if(obj.name=="seoff") {
					se_off.GetComponent<SpriteRenderer>().enabled=false;
					se_off.GetComponent<BoxCollider2D>().enabled=false;
					se_on.GetComponent<SpriteRenderer>().enabled=true;
					se_on.GetComponent<BoxCollider2D>().enabled=true;
					Game.se_sw=true;
				}
				if(obj.name=="seon") {
					se_off.GetComponent<SpriteRenderer>().enabled=true;
					se_off.GetComponent<BoxCollider2D>().enabled=true;
					se_on.GetComponent<SpriteRenderer>().enabled=false;
					se_on.GetComponent<BoxCollider2D>().enabled=false;
					Game.se_sw=false;
				}
			}
		}
		if(tap_flag==true && eff_gamestart_tapping_alpha.isStopped) { // caution: "gamesart_tapping_alpha"
            wait_message.GetComponent<SpriteRenderer>().enabled = false;
            eff_gamestart_tapping.SetActive(false);
			eff_gamestart.SetActive(true);
			SceneManager.LoadScene("Game");
		}
		if(Input.GetKey(KeyCode.Escape))
			SceneManager.LoadScene("Title");
    }
}
