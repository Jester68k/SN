// Converted from UnityScript to C# at http://www.M2H.nl/files/js_to_c.php - by Mike Hergaarden
// Do test the code! You usually need to change a few small bits.

using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Advertisements;
using Unity.VisualScripting;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Net.Sockets;
using System.Drawing;
using TMPro;
using System.Linq;
enum Mode
{
    Normal, FadeOut, Falling, StageClear, Congratulations, GameOver
};
enum WinLoseStat
{ 
	Playing, Win, Lose, Draw
};

enum GameType { 
	A, B
};

public class Game : MonoBehaviour {

	public AudioSource audioSource;
	public static AudioClip[] BGM = new AudioClip[3];
	public AudioClip stage_added_se;
	public AudioClip Erase_se;
	public AudioClip Land_se;
	public AudioClip Clear_BGM;
	public int base_points = 10, points_to_add;
	public int block_types = 6;
	public int playfield_width = 8;
	public int playfield_height = 8;
	public int[] score;    // score of first and second move
	public int[] max_chain = new int[2];    // max chain in the game
	public int chain_count = 0; // chain count
	public int num_excellent;
	public int high_score = 0;  // high score
	public bool high_score_flag = false;
	public static int play_count;   // count of play
	public static int[,] top_ten = new int[2, 10];
	public static int[,] top_ten_chain = new int[2, 10];
	public int[,] block = new int[20, 20];  // block array
	public int[,] copied_block = new int[20, 20]; // コピーされたブロック配列
	public int[,] search_block = new int[20, 20];       // search blcck array
	public int[,] erasing_block = new int[20, 20];  // erasing block array	
	public float[,] block_fall = new float[20, 20]; // ブロックが落下する距離
	public GameObject[,] obj_blocks = new GameObject[20, 20];   // game object array
	public Sprite[] block_sprites;              // sprite pattern of 12 types 
	public bool erase_flag;             // 消滅フラグ
	public int erase_count;                 // 連鎖カウンター
	public int before_erase_count;          // COMBO数カウンター
	public float disp_time = 0.0f;        // COMBO表示timer
	public bool land_se_flag;               // flag to land
	public bool erase_se_flag;
	public float longest_falling_distance;  // distance of longest falling
	public float falling_speed = 0.125f;        // speed of falling block
	public float count_to_fade_out;         //counter to fade out
	public float time_to_fade_out = 1.0f;       // time to fade out
	public int left_blocks;
	private Mode mode;
	private WinLoseStat winlose_stat;
	public int stage, num_stages, num_base_stages = 3;
	public int turn_num, turn_move;
	public static bool BGM_sw = true;
	public static int BGM_no = 1;
	public static bool se_sw = true;
	public static int game_type = 0;
	public static int move_mode = 0;
	public int ad_per_playcount = 5;
	public int pos_x, pos_y;
	public bool gameover_flag = false;
	bool top_ten_flag = false;

	public GUIStyle style;
	public GUIStyle style2;
	public GUIStyle style3;
	public bool end_flag = false;
	public GameObject gameover_message;
	public ParticleSystem gameover_message_alpha;
	public GameObject stageclear_message;
	public ParticleSystem stageclear_message_completed;
	public GameObject congratulations_message;
	public ParticleSystem congratulations_message_alpha;
	public GameObject block_prefab;
	public GameObject wall_prefab;
	public GameObject vanish_prefab;
	public TMPro.TMP_Text result_score;
	public string result_msg = "";

    //private string gameId = "5190009";
    //private string placementId = "Banner Android";
    //private bool testMode = false;

    /*	bool Check_End() {
            int x;
            int y;
            bool rensetsu_flag;

            rensetsu_flag = true;
            for (y = 0; y < playfield_height; y++)
                for (x = 0; x < playfield_width - 1; x++)
                    if (block[y, x] != 0 && block[y, x] == block[y, x + 1])
                        rensetsu_flag = false;
            for (x = 0; x < playfield_width; x++)
                for (y = 0; y < playfield_height - 1; y++)
                    if (block[y, x] != 0 && block[y, x] == block[y + 1, x])
                        rensetsu_flag = false;
            return rensetsu_flag;
        }*/

    void OnGUI()
	{
#if UNITY_ANDROID || UNITY_IOS
		GUI.Label(new Rect(20, 10, 400, 60), "SCORE", style);
		GUI.Label(new Rect(20, 10, 480, 60), "" + score[0], style2);
		if (game_type == 1)
		{
			GUI.Label(new Rect(20, 50, 480, 60), "" + score[1], style2);
		}
		GUI.Label(new Rect(20, 90, 400, 60), "HIGH SCORE", style);
		GUI.Label(new Rect(20, 90, 480, 60), "" + high_score, style2);
		GUI.Label(new Rect(Screen.width - 400, 130, 400, 60), "STAGE", style);
		if (game_type == 0)
			GUI.Label(new Rect(Screen.width - 400, 130, 400, 60), "" + stage + "/" + num_stages + "  ", style2);
		else
			GUI.Label(new Rect(Screen.width - 400, 130, 400, 60), "" + stage + "/3  ", style2);
		GUI.Label(new Rect(Screen.width - 400, 170, 400, 60), "MAX CHAIN", style);
		if((GameType)game_type==GameType.A)
			GUI.Label(new Rect(Screen.width - 400, 170, 400, 60), "" + max_chain[0] + "  ", style2);
		else
            GUI.Label(new Rect(Screen.width - 400, 170, 400, 60), "" + max_chain[0] + " - "+max_chain[1]+"  ", style2);
        if ((mode == Mode.Normal && before_erase_count >= 1 && disp_time > 0.0f) || (mode != Mode.Normal && erase_count >= 1))
		{
			GUI.Label(new Rect(Screen.width - 320 - 100, 210, 300, 60), "×" + chain_count, style);
			if (num_excellent <= 1)
				GUI.Label(new Rect(Screen.width - 320, 210, 300, 60), points_to_add + " PTS  ", style2);
			else
				GUI.Label(new Rect(Screen.width - 320, 210, 300, 60), points_to_add + " x" + num_excellent + " PTS  ", style2);
			if (chain_count >= 7)
				if ((GameType)game_type==GameType.A && num_excellent >= 2)
				{
					GUI.Label(new Rect(Screen.width - 320, 250, 300, 60), "EXCELLENT! x" + num_excellent, style3);
					if (num_excellent == 3)
						GUI.Label(new Rect(100, 120, 300, 60), "STAGE ADDED!", style3);
				}
				else
					GUI.Label(new Rect(Screen.width - 320, 250, 300, 60), "EXCELLENT!", style3);
		}
		if (gameover_flag == false && game_type == 1 && (mode == Mode.Normal || mode == Mode.FadeOut || mode == Mode.Falling || mode == Mode.StageClear))
		{
			if (turn_move == 0)
				GUI.Label(new Rect(0, Screen.height*3/4+50*0, Screen.width, 60), "TURN " + turn_num + " -  FIRST MOVE", style3);
			else
				GUI.Label(new Rect(0, Screen.height * 3 / 4 + 50*0, Screen.width, 60), "TURN " + turn_num + " -  SECOND MOVE", style3);
			if (turn_move == move_mode)
				GUI.Label(new Rect(0, Screen.height * 3 / 4 + 50*1, Screen.width, 60), "PLAYER SIDE", style3);
			else
				GUI.Label(new Rect(0, Screen.height * 3 / 4 + 50*1, Screen.width, 60), "COMPUTER SIDE", style3);
		}
/*		if((mode==Mode.Congratulations ||mode==Mode.GameOver)&& (GameType)game_type==GameType.B) { 
			switch (winlose_stat)
			{
				case WinLoseStat.Win:
					GUI.Label(new Rect(0, Screen.height * 3 / 4 + 50*3, Screen.width, 60), "PLAYER WINS!!", style3);
					break;
				case WinLoseStat.Lose:
					GUI.Label(new Rect(0, Screen.height * 3 / 4+50 * 3, Screen.width, 60), "COMPUTER WINS!!", style3);
					break;
				case WinLoseStat.Draw:
					GUI.Label(new Rect(0, Screen.height * 3 / 4 + 50*3, Screen.width, 60), "DRAW", style3);
					break;
			}
            GUI.Label(new Rect(0, Screen.height * 3 / 4 + 50 * (-1), Screen.width, 60), "FIRST MOVE: " + score[0], style3);
            GUI.Label(new Rect(0, Screen.height * 3 / 4 + 50 * 0, Screen.width, 60), "VS", style3);
            GUI.Label(new Rect(0, Screen.height * 3 / 4 + 50 * 1, Screen.width, 60), "SECOND MOVE: " + score[1], style3);
        }
*/
#else
        GUI.Label(new Rect(20, 10, 400, 60), "SCORE", style);
        GUI.Label(new Rect(20, 10, 480, 60), "" + score[0], style2);
        if (game_type == 1)
        {
            GUI.Label(new Rect(20, 50, 480, 60), "" + score[1], style2);
        }
        GUI.Label(new Rect(20, 90, 400, 60), "HIGH SCORE", style);
        GUI.Label(new Rect(20, 90, 480, 60), "" + high_score, style2);
        GUI.Label(new Rect(Screen.width - 400, 130, 400, 60), "STAGE", style);
        if (game_type == 0)
            GUI.Label(new Rect(Screen.width - 400, 130, 400, 60), "" + stage + "/" + num_stages + "  ", style2);
        else
            GUI.Label(new Rect(Screen.width - 400, 130, 400, 60), "" + stage + "/3  ", style2);
        GUI.Label(new Rect(Screen.width - 400, 170, 400, 60), "MAX CHAIN", style);
        if ((GameType)game_type == GameType.A)
            GUI.Label(new Rect(Screen.width - 400, 170, 400, 60), "" + max_chain[0] + "  ", style2);
        else
            GUI.Label(new Rect(Screen.width - 400, 170, 400, 60), "" + max_chain[0] + " - " + max_chain[1] + "  ", style2);
        if ((mode == Mode.Normal && before_erase_count >= 1 && disp_time > 0.0f) || (mode != Mode.Normal && erase_count >= 1))
        {
            GUI.Label(new Rect(Screen.width - 320 - 100, 210, 300, 60), "×" + chain_count, style);
            if (num_excellent <= 1)
                GUI.Label(new Rect(Screen.width - 320, 210, 300, 60), points_to_add + " PTS  ", style2);
            else
                GUI.Label(new Rect(Screen.width - 320, 210, 300, 60), points_to_add + " x" + num_excellent + " PTS  ", style2);
            if (chain_count >= 7)
                if ((GameType)game_type == GameType.A && num_excellent >= 2)
                {
                    GUI.Label(new Rect(Screen.width - 320, 250, 300, 60), "EXCELLENT! x" + num_excellent, style3);
                    if (num_excellent == 3)
                        GUI.Label(new Rect(100, 120, 300, 60), "STAGE ADDED!", style3);
                }
                else
                    GUI.Label(new Rect(Screen.width - 320, 250, 300, 60), "EXCELLENT!", style3);
        }
        if (gameover_flag == false && game_type == 1 && (mode == Mode.Normal || mode == Mode.FadeOut || mode == Mode.Falling || mode == Mode.StageClear))
        {
            if (turn_move == 0)
                GUI.Label(new Rect(0, Screen.height * 3 / 4 + 50 * 0, Screen.width, 60), "TURN " + turn_num + " -  FIRST MOVE", style3);
            else
                GUI.Label(new Rect(0, Screen.height * 3 / 4 + 50 * 0, Screen.width, 60), "TURN " + turn_num + " -  SECOND MOVE", style3);
            if (turn_move == move_mode)
                GUI.Label(new Rect(0, Screen.height * 3 / 4 + 50 * 1, Screen.width, 60), "PLAYER SIDE", style3);
            else
                GUI.Label(new Rect(0, Screen.height * 3 / 4 + 50 * 1, Screen.width, 60), "COMPUTER SIDE", style3);
        }
#endif
     }

            void Start() {
                int x;  // position X
                int y;  // position Y

                //Advertisement.Initialize(gameId, testMode);
                //StartCoroutine(ShowBannerAds());

                if (BGM_sw) {
                    audioSource = gameObject.GetComponent<AudioSource>();
                    audioSource.loop = true;
                    audioSource.clip = BGM[BGM_no - 1];
                    audioSource.Play();
                }
                for (int i = 0; i < 2; i++)
                    score[i] = 0;
                turn_num = 1;       // turn number
                turn_move = 0;  // Fisrt Move
                high_score = top_ten[game_type, 0];
                stage = 0;
                num_stages = num_base_stages;
                for (int i = 0; i < 2; i++)
                    max_chain[i] = 0;
                num_excellent = 0;
                gameover_flag = false;
                winlose_stat = WinLoseStat.Playing;
                //	gameover_message = GameObject.Find("gameover");
                //	gameover_renderer = gameover_message.GetComponent<SpriteRenderer>();
                //	gameover_renderer.enabled=false;
                //	stageclear_message = GameObject.Find("Eff_Completed");
                //	stageclear_renderer = stageclear_message.GetComponent<SpriteRenderer>();
                stageclear_message.SetActive(false);
                //	congratulations_message = GameObject.Find("congratulations");
                //	congratulations_renderer = congratulations_message.GetComponent<SpriteRenderer>();
                //	congratulations_renderer.enabled=false;
                for (y = -1; y < playfield_height; y++) {
                    Instantiate(wall_prefab, new Vector3(-1, y, 0), transform.rotation);
                    Instantiate(wall_prefab, new Vector3(playfield_width, y, 0), transform.rotation);
                }
                for (x = -1; x < playfield_width + 1; x++) {
                    Instantiate(wall_prefab, new Vector3(x, -1, 0), transform.rotation);
                    Instantiate(wall_prefab, new Vector3(x, playfield_height, 0), transform.rotation);
                }
                New_Stage();
            }



        void New_Stage ()
        { 
            int x;	// position X
            int y;  // position Y
            Vector3 pos;

            if (BGM_sw) {
                audioSource.Stop();
                audioSource.loop=true;
                audioSource.clip=BGM[BGM_no-1];
                audioSource.Play();
            }
            left_blocks=playfield_width*playfield_height;
            stage++;
            for(y=0; y<playfield_height; y++)
                for(x=0; x<playfield_width; x++) {
                    block[y,x]=0;	// initialize block array
                    erasing_block[y,x]=0;
                }
            for(y=0; y<playfield_height; y++) 
                for(x=0; x<playfield_width; x++) 
                        block[y,x]=Random.Range(1,block_types+1);
            for(y=0; y<playfield_height; y++) {
                for(x=0; x<playfield_width; x++) {
                    if(block[y,x]!=0) {
                        obj_blocks[y,x] = Instantiate(block_prefab);
                        pos = obj_blocks[y, x].transform.position;
                        pos.x = x;
                        pos.y = y;
                        obj_blocks[y, x].transform.position = pos;
                        SpriteRenderer block_renderer= obj_blocks[y,x].GetComponent<SpriteRenderer>();
                        int color_num = block[y,x] ;
                        block_renderer.sprite = block_sprites[color_num];
                        block_fall[y,x] = 0.0f;
                    }
                }
            }
            erase_flag = false;
            count_to_fade_out = 0.0f;
            longest_falling_distance = 0.0f;
            points_to_add=base_points;
            erase_count = 0;
            disp_time = 0.0f;
            before_erase_count=0;
            land_se_flag=false;
            erase_se_flag=false;
            mode=Mode.Normal;
            turn_num = 1;
            turn_move = 0;
            stageclear_message.SetActive(false);
        }

            void Update()
            {
                Vector3 pos;
                SpriteRenderer block_renderer;
                int x, y;

                if (disp_time > 0.0f)
                {
                    disp_time -= Time.deltaTime;
                }
                else
                {
                    disp_time = 0.0f;
                    erase_count = 0;
                    before_erase_count = 0;
                }
                if(gameover_flag==false) 
                    switch (mode)
                    {
                        case Mode.Normal:
                            if (game_type == 0 || turn_move == move_mode)
                            { 
                                if (Input.GetMouseButton(0))
                                {
                                    //マウスの左ボタンを押した
                                    //・カーソル位置取得
                                    RaycastHit2D hit;
                                    Collider2D collider;
                                    GameObject obj;
                                    Vector2 catblk_pos;

                                    catblk_pos = GetComponent<Camera>().ScreenToWorldPoint(Input.mousePosition);
                                    hit = Physics2D.Raycast(catblk_pos, Vector2.zero);
                                    collider = hit.collider;
                                    if (hit.collider != null && hit.collider.gameObject != null)
                                    {
                                        obj = hit.collider.gameObject;
                                        x = (int)obj.transform.position.x;
                                        y = (int)obj.transform.position.y;
                                        if (x >= 0 && x < playfield_width && y >= 0 && y < playfield_height && block[y, x] != 0)
                                        {
                                            Debug.Log("Block Player Selected x:" + x + " y:" + y);
                                            int color_num = block[y, x];
                                            Check_Erasing_Blocks(x, y, color_num);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                GameAI();	// pos_x, pos_yに代入してreturn
                                Debug.Log("X:" + pos_x + " y:" + pos_y+" block[y,x]="+block[pos_y,pos_x]);
                                if (pos_x >= 0 && pos_x < playfield_width && pos_y >= 0 && pos_y < playfield_height && block[pos_y, pos_x] != 0)
                                {
                                    int color_num = block[pos_y, pos_x];
                                    Check_Erasing_Blocks(pos_x, pos_y, color_num);
                                } else
                                {
                                    Debug.Log("Game AI selected block Erred!");
                                }
                            }
                            break;
                        case Mode.FadeOut:  // fade out blocks
                            UnityEngine.Color col;

                            Debug.Log("mode Fadeout: count_to_fade_out:" + count_to_fade_out+" left_blocks:"+left_blocks+" turn:"+turn_num+" turn_move:"+turn_move);
                            if (count_to_fade_out > 0.0f)
                            {
                                for (y = 0; y < playfield_height; y++)
                                {
                                    for (x = 0; x < playfield_width; x++)
                                    {
                                        if (erasing_block[y, x] > 0)
                                        {
                                            block_renderer = obj_blocks[y, x].GetComponent<SpriteRenderer>();
                                            col = block_renderer.color;
                                            col.a = count_to_fade_out;
                                            obj_blocks[y, x].gameObject.GetComponent<SpriteRenderer>().color = col;
                                        }
                                    }
                                }
                                count_to_fade_out -= Time.deltaTime * 4; // フェードアウトするカウントを減らす
                                Debug.Log("count_to_fade_out-= Time.deltaTime * 4; : mode Fadeout: count_to_fade_out:" + count_to_fade_out + " left_blocks:" + left_blocks + " turn:" + turn_num + " turn_move:" + turn_move);
                                if (count_to_fade_out <= 0.0f)
                                {
                                    count_to_fade_out = 0.0f;
                                    Debug.Log("count_to_fade_out = 0.0f; : mode Fadeout: count_to_fade_out:" + count_to_fade_out + " left_blocks:" + left_blocks + " turn:" + turn_num + " turn_move:" + turn_move);
                                }
                            }
                            else
                            {
                                Debug.Log("mode Fadeout: count_to_fade_out:" + count_to_fade_out + " left_blocks:" + left_blocks + " turn:" + turn_num + " turn_move:" + turn_move);
                                for (y = 0; y < playfield_height; y++)
                                {
                                    for (x = 0; x < playfield_width; x++)
                                    {
                                        if (block[y, x] > 0 && erasing_block[y, x] > 0)
                                        {
                                            block[y, x] = 0;
                                            Destroy(obj_blocks[y, x]);
                                            obj_blocks[y, x] = null;
                                            erasing_block[y, x] = 0;
                                            block_fall[y, x] = 0.0f;
                                            left_blocks--;
                                        }
                                    }
                                }
                                if ((GameType)game_type==GameType.B || ((GameType)game_type==GameType.A && num_excellent <= 1))
                                    score[turn_move] += points_to_add;
                                else
                                    score[turn_move] += points_to_add * num_excellent;
                                if(game_type==0)
                                    if (score[0] > high_score)
                                    {
                                        high_score = score[0];
                                        high_score_flag = true;
                                    }
                                mode = Mode.Falling;
                                Debug.Log("Mode.Fadeout to Mode.Falling");
                            }
                            break;
                        case Mode.Falling:
                            Debug.Log("Mode.Falling: longest_falling_distance:" + longest_falling_distance+" left_blocks="+left_blocks+" turn:"+turn_num+"  turn_move:"+turn_move);
                            if (longest_falling_distance > 0.0f)	{
                                longest_falling_distance -= Time.deltaTime * 8;
                                Debug.Log("Mode.Falling: longest_falling_distance:" + longest_falling_distance + " left_blocks=" + left_blocks + " turn:" + turn_num + "  turn_move:" + turn_move);
                                if (longest_falling_distance < 0.0f)
                                {
                                    longest_falling_distance = 0.0f;
                                    Debug.Log("Mode.Falling: longest_falling_distance:" + longest_falling_distance + " left_blocks=" + left_blocks + " turn:" + turn_num + "  turn_move:" + turn_move);
                                }
                                land_se_flag = false;
                                for (y = 0; y < playfield_height; y++)	{
                                    for (x = 0; x < playfield_width; x++)	{
                                        if (block_fall[y, x] > 0.0f){
                                            block_fall[y, x] -= Time.deltaTime * 8;
                                            pos = obj_blocks[y, x].transform.position;
                                            pos.y -= Time.deltaTime * 8;
                                            obj_blocks[y, x].transform.position = pos;
                                            if (block_fall[y, x] < 0.0f){
                                                pos = obj_blocks[y, x].transform.position;
                                                pos.y = Mathf.Round(pos.y);
                                                obj_blocks[y, x].transform.position = pos;
                                            }
                                            Debug.Log("Falling Positon Y:" + obj_blocks[y, x].transform.position.y + " longest_falling_distance:" + longest_falling_distance+" block_fall[" + y + "," + x + "]=" + block_fall[y,x]+" left_blocks="+left_blocks+" turn:"+turn_num+" turn_move:"+turn_move);
                                            if (block_fall[y, x] <= 0.0f)	{
                                                land_se_flag = true;
                                                block[(int)obj_blocks[y, x].transform.position.y, x] = block[y,x];
                                                block_fall[y, x] = 0.0f;
                                                obj_blocks[(int)obj_blocks[y,x].transform.position.y, x] = obj_blocks[y, x];
                                                obj_blocks[y, x] = null;
                                                block[y, x] = 0;
                                            }
                                        }
                                    }
                                }
                                if (land_se_flag)
                                {
                                    Debug.Log("land SE");
                                    if (se_sw)
                                        audioSource.PlayOneShot(Land_se, 1.0f);
                                }
                            } else {
                                Debug.Log("End of falling  TURN:" + turn_num + " MOVE:" + turn_move + " left_blocks:" + left_blocks);
                                mode = Mode.Normal;
                                Debug.Log("Mode.Falling to Mode.Normal");
                                if (left_blocks <= 0)
                                {
                                    if ((game_type == 0 && stage < num_stages) || (game_type == 1 && stage < 3))
                                    {
                                        mode = Mode.StageClear;
                                        Debug.Log("Mode.StageClear");
                                        stageclear_message.SetActive(true);
                                    }
                                    else
                                    {
                                        mode = Mode.Congratulations;
                                        if ((GameType)game_type == GameType.B)
                                        {
                                            result_msg = score[0] + "\nVS\n" + score[1] + "\n";
                                            if (score[move_mode] > score[(move_mode + 1) % 2])
                                            {
                                                winlose_stat = WinLoseStat.Win;
                                                result_msg += "PLAYER\nWINS!!";
                                            }
                                            else
                                                if (score[move_mode] < score[(move_mode + 1) % 2])
                                            {
                                                winlose_stat = WinLoseStat.Lose;
                                                result_msg += "COMPUTER\nWINS!!";
                                            }
                                            else
                                            {
                                                winlose_stat = WinLoseStat.Draw;
                                                result_msg += "DRAW\n"; // 同点
                                            }
                                            if (winlose_stat==WinLoseStat.Win && score[move_mode] > high_score)
                                            {
                                                high_score = score[move_mode];
                                                high_score_flag = true;
                                            }
                                        }
                                        //								gameover_flag = true;
                                        top_ten_flag = false;
                                        Debug.Log("Mode.Congratulations");
                                        if (winlose_stat==WinLoseStat.Win)
                                            congratulations_message.SetActive(true);
                                        else
                                            stageclear_message.SetActive(true);
                                    }
                                    if (BGM_sw)
                                    {
                                        audioSource.Stop();
                                        audioSource.PlayOneShot(Clear_BGM, 1.0f);
                                    }
                                }	else	{
                                    if ((GameType)game_type == GameType.B)
                                    {
                                        if (++turn_move >= 2)
                                        {
                                            turn_move = 0;
                                            ++turn_num;
                                        }
                                        Debug.Log("mode:" + mode + " longest_falling_distance:" + longest_falling_distance + " left_blocks=" + left_blocks + " turn:" + turn_num + "  turn_move:" + turn_move);
                                    }
                                }
                            }
                            break;
                        case Mode.StageClear:
                            if (stageclear_message_completed.isPlaying == false)
                            {
                                New_Stage();
                                mode = Mode.Normal;
                            }
                            break;
                    case Mode.Congratulations:
                            if ((winlose_stat==WinLoseStat.Win && congratulations_message_alpha.isStopped == true) || ((winlose_stat==WinLoseStat.Lose)||winlose_stat==WinLoseStat.Draw)&& stageclear_message_completed.isStopped==true) {
                                int i;

                                for (i = 0; i < 10; i++)
                                {
                                    if ((game_type ==(int)GameType.A && score[0] > top_ten[game_type, i]) || (game_type == 1 && score[move_mode] > score[(move_mode + 1) & 1] && score[move_mode] > top_ten[game_type, i]))
                                    {
                                        {
                                            for (int j = 9; j > i; j--)
                                            {
                                                top_ten[game_type, j] = top_ten[game_type, j - 1];
                                                top_ten_chain[game_type, j] = top_ten_chain[game_type, j - 1];
                                            }
                                            top_ten[game_type, i] = score[move_mode];
                                            top_ten_chain[game_type, i] = max_chain[move_mode];
                                            top_ten_flag = true;
                                            break;
                                        }
                                    }
                                    if (top_ten_flag)
                                    {
                                        for (i = 0; i < 10; i++)
                                        {
                                            PlayerPrefs.SetInt("TopTen" + game_type + "_" + i, top_ten[game_type, i]);
                                            PlayerPrefs.SetInt("TopTenChain" + game_type + "_" + i, top_ten_chain[game_type, i]);
                                        }
                                        PlayerPrefs.Save();
                                    }
                                }
                                play_count = PlayerPrefs.GetInt("play_count");
                                ++play_count;
                                if (play_count >= ad_per_playcount)
                                {  // ad_per_playcountは何回に１回広告動画を流すかの定数です。
                                    play_count = 0;
                                    PlayerPrefs.SetInt("play_count", play_count);
        #if (UNITY_ANDROID || UNITY_IOS)
                                    //	SceneManager.LoadScene("UnityAds");
        #else
                                if(top_ten_flag)
                                    SceneManager.LoadScene("TopTen");
                                else
                                    SceneManager.LoadScene("Title");
        #endif
                                }
                                else
                                {
                                    PlayerPrefs.SetInt("play_count", play_count);
                                }

                                result_score.SetText(result_msg);
                                mode = Mode.GameOver;
                            }
                            break;
                    case Mode.GameOver:
                        if (Input.GetMouseButtonDown(0))
                                if (top_ten_flag)
                                    SceneManager.LoadScene("TopTen");
                                else
                                    SceneManager.LoadScene("Title");
                        break;
                }
            else
                if (Input.GetMouseButtonDown(0))
                    if (top_ten_flag)
                        SceneManager.LoadScene("TopTen");
                    else
                        SceneManager.LoadScene("Title");

            if (Input.GetKey(KeyCode.Escape))
                SceneManager.LoadScene("Title");
            if(Input.GetKey(KeyCode.T))
                SceneManager.LoadScene("TopTen");
            if(Input.GetKey(KeyCode.G)) {
                    gameover_flag = true;
                mode = Mode.GameOver;
                gameover_message.SetActive(true);
            }
        }

        void  Check_Neighbor ( int x ,   int y  ,   int color  ){
            if(x<0 || x>=playfield_width || y<0 || y>=playfield_height)
                return;
            if(x+1<playfield_width && block[y,x+1]==color && search_block[y,x+1]==0) {
                search_block[y,x+1]=1;
                erasing_block[y,x+1]=1;
                points_to_add *= 2;
                chain_count++;
                Check_Neighbor(x+1,y,color);
            }
            if(x-1>=0 && block[y,x-1]==color && search_block[y,x-1]==0) {
                search_block[y,x-1]=1;
                erasing_block[y,x-1]=1;
                points_to_add *= 2;
                chain_count++;
                Check_Neighbor(x-1,y,color);
            }
            if(y+1<playfield_height && block[y+1,x]==color && search_block[y+1,x]==0) {
                search_block[y+1,x]=1;
                erasing_block[y+1,x]=1;
                points_to_add *= 2;
                chain_count++;
                Check_Neighbor(x,y+1,color);
            }
            if(y-1>=0 && block[y-1,x]==color && search_block[y-1,x]==0) {
                search_block[y-1,x]=1;
                erasing_block[y-1,x]=1;
                points_to_add *= 2;
                chain_count++;
                Check_Neighbor(x,y-1,color);
            }

        //	if(x+1<playfield_size && y+1<playfield_size && block[y+1,x+1]==color && !search_block[y+1,x+1]) {
        //		search_block[y+1,x+1]=1;
        //		erasing_block[y+1,x+1]=1;
        //		points_to_add *= 2;
        //		chain_count++;
        //		Check_Neighbor(x+1,y+1,color);
        //	}
        //	if(x-1>=0 && y+1<playfield_size && block[y+1,x-1]==color && !search_block[y+1,x-1]) {
        //		search_block[y+1,x-1]=1;
        //		erasing_block[y+1,x-1]=1;
        //		points_to_add *= 2;
        //		chain_count++;
        //		Check_Neighbor(x-1,y+1,color);
        //	}
        //	if(y-1>=0 && x+1<playfield_size && block[y-1,x+1]==color && !search_block[y-1,x+1]) {
        //		search_block[y-1,x+1]=1;
        //		erasing_block[y-1,x+1]=1;
        //		points_to_add *= 2;
        //		chain_count++;
        //		Check_Neighbor(x+1,y-1,color);
        //	}
        //	if(y-1>=0 && x-1>=0 && block[y-1,x-1]==color && !search_block[y-1,x-1]) {
        //		search_block[y-1,x-1]=1;
        //		erasing_block[y-1,x-1]=1;
        //		points_to_add *= 2;
        //		chain_count++;
        //		Check_Neighbor(x-1,y-1,color);
        //	}
        }

            void Check_Erasing_Blocks(int pos_x, int pos_y, int color) {
                int x, y, space;

            if (block[pos_y, pos_x] == 0)
            { // そこにブロックが無い場合はエラー
                Debug.LogError("No block x:"+pos_x+" y:"+pos_y); 	
                return;
            }
            points_to_add = base_points;	// points to add
            before_erase_count = erase_count;
            erase_count = 0;
            for(y=0; y<playfield_height; y++) 
                for(x=0; x<playfield_width; x++) {
                    erasing_block[y,x]=0;	// 消滅するブロックの配列の初期化
                    search_block[y,x]=0;
                }

            search_block[pos_y,pos_x]=1;
            erasing_block[pos_y,pos_x]=1;
            chain_count=1;
            Check_Neighbor(pos_x,pos_y,color);
            if (chain_count > max_chain[turn_move])
                    max_chain[turn_move]=chain_count;
            if ((GameType)game_type == GameType.A)
            {
                if (chain_count >= 7)
                {
                    num_excellent++;
                    if (num_excellent == 3)
                    {
                        ++num_stages;
                        if (BGM_sw)
                        {
                            audioSource.Stop();
                            audioSource.PlayOneShot(stage_added_se, 1.0f);
                        }
                    }
                } else {
                    num_excellent = 0;
                }
            }

            for(x=0; x<playfield_width; x++) 
                for(y=0; y<playfield_height; y++) 
                    if(erasing_block[y,x]!=0) 
                        Instantiate(vanish_prefab, new Vector3 (x, y, 0), Quaternion.identity);
            longest_falling_distance = 0.0f;
            for(y=0; y<playfield_height; y++)
                for(x=0; x<playfield_width; x++) 
                    block_fall[y,x]=0.0f;
            for(x=0; x<playfield_width; x++) {
                space=0;
                for(y=0; y<playfield_height; y++) {
                    if(block[y,x]!=0) {
                        block_fall[y,x]=space;
                        if(erasing_block[y,x]!=0) 
                            space++;
                    } else
                        break;
                }
                if(longest_falling_distance< (float)space )
                    longest_falling_distance = (float)space;
            }
            if(se_sw)
                audioSource.PlayOneShot(Erase_se,1.0f);
            erase_count ++;
            before_erase_count = erase_count;
            disp_time = 1.0f;
            mode=Mode.FadeOut;
            Debug.Log("Mode.Normal to Mode.FadeOut");
             count_to_fade_out = time_to_fade_out;
            }
            void Check_Erasing_Blocks_GameB(int pos_x, int pos_y, int color)
            {
                int x, y; // space;

                if (block[pos_y, pos_x] == 0)
                { // そこにブロックが無い場合はエラー
                    Debug.LogError("No block x:" + pos_x + " y:" + pos_y);
                    return;
                }
                points_to_add = base_points;    // points to add
                before_erase_count = erase_count;
                erase_count = 0;
                for (y = 0; y < playfield_height; y++)
                    for (x = 0; x < playfield_width; x++)
                    {
                        erasing_block[y, x] = 0;    // 消滅するブロックの配列の初期化
                        search_block[y, x] = 0;
                    }

                search_block[pos_y, pos_x] = 1;
                erasing_block[pos_y, pos_x] = 1;
                chain_count = 1;
                Check_Neighbor(pos_x, pos_y, color);
           //	if (chain_count > max_chain[turn_move])
           //	max_chain[turn_move] = chain_count;
                if ((GameType)game_type == GameType.A)
                {
                    if (chain_count >= 7)
                    {
                        num_excellent++;
                        if (num_excellent == 3)
                        {
                            ++num_stages;
                            if (BGM_sw)
                            {
                                audioSource.Stop();
                                audioSource.PlayOneShot(stage_added_se, 1.0f);
                            }
                        }
                    }
                    else
                    {
                        num_excellent = 0;
                    }
                }

              /*
                 for (x = 0; x < playfield_width; x++)
                    for (y = 0; y < playfield_height; y++)
                        if (erasing_block[y, x] != 0)
                            Instantiate(vanish_prefab, new Vector3(x, y, 0), Quaternion.identity);
                longest_falling_distance = 0.0f;
                for (y = 0; y < playfield_height; y++)
                    for (x = 0; x < playfield_width; x++)
                        block_fall[y, x] = 0.0f;
                for (x = 0; x < playfield_width; x++)
                {
                    space = 0;
                    for (y = 0; y < playfield_height; y++)
                    {
                        if (block[y, x] != 0)
                        {
                            block_fall[y, x] = space;
                            if (erasing_block[y, x] != 0)
                                space++;
                        }
                        else
                            break;
                    }
                    if (longest_falling_distance < (float)space)
                        longest_falling_distance = (float)space;
                }
                if (se_sw)
                    audioSource.PlayOneShot(Erase_se, 1.0f);
                erase_count++;
                before_erase_count = erase_count;
                disp_time = 1.0f;
                mode = Mode.FadeOut;
                Debug.Log("Mode.Normal to Mode.FadeOut");
                count_to_fade_out = time_to_fade_out;
              */
    }

    void GameAI()
	{
		int x, y, x2, y2, max_num_blocks=0;

		// まず初めに現在のブロック配列をコピーする
		for (y = 0; y < playfield_height; y++)
			for (x = 0; x < playfield_width; x++)
				copied_block[y, x] = block[y, x];
		for (y2 = 0; y2 < playfield_height; y2++)
			for (x2 = 0; x2 < playfield_width; x2++) { 
				for (y = 0; y < playfield_height; y++)
                    for (x = 0; x < playfield_width; x++)
						block[y, x] = copied_block[y, x];
				if (block[y2, x2] > 0){
					Check_Erasing_Blocks_GameB(x2, y2, block[y2, x2]);
					if (chain_count > max_num_blocks){
						max_num_blocks = chain_count;
						pos_x = x2;
						pos_y = y2;
					}
				}
            }
    }

    /*	void GameAI()
        {
            if (left_blocks < 10)
            {
                for (pos_y = 0; pos_y < playfield_height; pos_y++)
                    for (pos_x = 0; pos_x < playfield_width; pos_x++)
                        if (block[pos_y, pos_x] > 0)
                        {
                            Debug.Log("Selected Block x:" + pos_x + " y:" + pos_y + " block[y,x]=" + block[pos_y, pos_x]);
                            return;
                        }
            }
            else
            {
                do
                {
                    pos_x = Random.Range(0, playfield_width - 1);
                    pos_y = Random.Range(0, playfield_height - 1);
                    Debug.Log("Selected Random Block x:" + pos_x + " y:" + pos_y + " block[y,x]=" + block[pos_y, pos_x]);
                } while (pos_x <0 || pos_x >=playfield_width || pos_y <0 || pos_y >= playfield_height || block[pos_y, pos_x] == 0);
            }
        }*/
}