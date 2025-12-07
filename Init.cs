using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Init : MonoBehaviour
{
	public AudioClip bgm1_data;
    public AudioClip bgm2_data;
	public AudioClip bgm3_data;
   
    // Start is called before the first frame update
    void Start()
    {
        // 広告を子供向けに設定する
        //
        Game.BGM[0] = bgm1_data;
    	Game.BGM[1] = bgm2_data;
    	Game.BGM[2] = bgm3_data;
        int mode, i;

        for (mode = 0; mode < 2; mode++)
        {
            for (i = 0; i < 10; i++)
            {
                Game.top_ten[mode, i] = PlayerPrefs.GetInt("TopTen" + mode + "_" + i);
                Game.top_ten_chain[mode, i] = PlayerPrefs.GetInt("TopTenChain" + mode + "_" + i);
            }
            if (Game.top_ten[0, 0] == 0)
            {
                for (i = 0; i < 10; i++)
                {
                    Game.top_ten[0, i] = (10 - i) * 1000;
                    Game.top_ten_chain[0, i] = 10 - i;
                }
            }
            if (Game.top_ten[1, 0] == 0)
            {
                for (i = 0; i < 10; i++)
                {
                    Game.top_ten[0, i] = (10 - i) * 100;
                    Game.top_ten_chain[0, i] = 1;
                }
            }
        }
        DontDestroyOnLoad(this);
        SceneManager.LoadScene("Title");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
