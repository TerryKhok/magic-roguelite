using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomGeneraterScript : MonoBehaviour
{
    public static RoomGeneraterScript Instance;

    const int EnemyNull = 0;//敵の情報未登録
    const int EnemyDie = 1;//敵死んでる
    const int EnemyLive = 2;//敵生きてる

    bool StartFlag = false;

    GameObject stairs;//階段のプレハブ

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        // シングルトンの呪文
        if (Instance == null)
        {
            // 自身をインスタンスとする
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(Instance);//このオブジェクトを壊さない
        stairs = (GameObject)Resources.Load("MapResource/Stairs");//階段のプレハブを取得
    }
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (MapGeneraterScript.Instance.g_nowpositionx == MapGeneraterScript.Instance.g_pointx && MapGeneraterScript.Instance.g_nowpositiony == MapGeneraterScript.Instance.g_pointy && StartFlag)
        {
            Instantiate(stairs, new Vector2(5, 5), Quaternion.identity);
        }
        else
        {
            StartFlag = true;
        }
    }

    void EnemyState()
    {

    }
}
