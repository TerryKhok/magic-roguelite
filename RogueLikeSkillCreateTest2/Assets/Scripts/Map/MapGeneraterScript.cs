using Mono.Cecil.Pdb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class MapGeneraterScript : MonoBehaviour //ミニマップを生成するためのスクリプト
{
    #region　変数
    public static MapGeneraterScript Instance;
    const int fieldy = 12;//フィールドの大きさＹ
    const int fieldx = 12;//フィールドの大きさＸ

    const int pointup = 1;//上方向
    const int pointright = 2;//右方向
    const int pointdown = 3;//下方向
    const int pointleft = 4;//右方向

    int _pointdir = 0;//マップを生成する方向
    public int g_pointy = 4;//マップ生成の現在位置Ｙ
    public int g_pointx = 4;//マップ生成の現在位置Ｘ
    [SerializeField]
    int _room = 15;//生成する部屋の最大数
    [SerializeField]
    int _roommin = 8;//生成する部屋の最小数

    int _trueroom = 0;

    public int g_playerdir;//部屋をまたいだプレイヤーの方向
    public int[,] g_field = new int[fieldy, fieldx];//フィールドの初期化

    public int g_nowpositionx = 4;//今のプレイヤーのポジションX
    public int g_nowpositiony = 4;//今のプレイヤーのポジションY

    public int g_nowfloor = 0;//現在の階層
    #endregion

    #region 型定義
    GameObject roadvar;//縦の道のプレハブ
    GameObject roadsid;//横の道のプレハブ
    GameObject froom;//部屋のプレハブ
    GameObject Player;//プレイヤーのプレハブ

    GameObject NewPlayer;//imageプレイヤー本体

    RectTransform rectTransform;//Canvas依存のトランスフォーム

    RightRoadScript rightroadsc;
    UpRoadScript uproadsc;
    DownRoadScript downroadsc;
    LeftRoadScript leftroadsc;
    GameObject TObj;//使いまわす用ゲームOBJ
    GameObject mainchar;//プレイヤーの本体
    #endregion

    #region Unity依存関数
    public void Awake()
    {
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
    }
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();//レクトトランスフォームのコンポーネントを取得
        roadvar = (GameObject)Resources.Load("MapResource/roadvar");//縦の道のプレハブを取得
        roadsid = (GameObject)Resources.Load("MapResource/roadsid");//横の道のプレハブを取得
        froom = (GameObject)Resources.Load("MapResource/froom");//部屋のプレハブを取得
        Player = (GameObject)Resources.Load("MapResource/Player");//プレイヤーのプレハブを取得
        mainchar = TObj = GameObject.Find("PlayerMoveR (1)");//メインキャラを取得
        SceneManager.sceneLoaded += OnSceneLoaded;
        Init();//更新処理
        Draw();//描画処理
        NewRoom();
    }
    #endregion

    #region　自作関数
    public void NewRoom()
    {
        TObj = GameObject.Find("UpRoadTrigger");
        uproadsc = TObj.GetComponent<UpRoadScript>();
        TObj = GameObject.Find("RightRoadTrigger");
        rightroadsc = TObj.GetComponent<RightRoadScript>();
        TObj = GameObject.Find("DownRoadTrigger");
        downroadsc = TObj.GetComponent<DownRoadScript>();
        TObj = GameObject.Find("LeftRoadTrigger");
        leftroadsc = TObj.GetComponent<LeftRoadScript>();
        uproadsc.UpRoadGenerate(g_field[g_nowpositiony + 1, g_nowpositionx]);
        rightroadsc.RightRoadGenerate(g_field[g_nowpositiony, g_nowpositionx + 1]);
        downroadsc.DownRoadGenerate(g_field[g_nowpositiony - 1, g_nowpositionx]);
        leftroadsc.LeftRoadGenerate(g_field[g_nowpositiony, g_nowpositionx - 1]);
    }
    public void Init()//更新処理
    {
        #region 迷走したアルゴリズム
        g_nowfloor += 1;    ///
        g_nowpositionx = 4; //
        g_nowpositiony = 4; //更新時に初期化する
        g_pointx = 4;　　　 //
        g_pointy = 4;       ///
        _trueroom = 0;
        for (int i = 0; i < fieldy; i++)
        {
            for (int j = 0; j < fieldx; j++)
            {
                g_field[i, j] = 0;//フィールド初期化処理
            }
        }
        for (int i = 0; i < _room; i++)
        {
            if(i % (_room / 2) == 0)                                                                                   //部屋の生成が二分の一おわったら
            {
                g_pointx = 4;                                                                                          //初期生成位置を初期化する
                g_pointy = 4;
            }
            _pointdir = UnityEngine.Random.Range(pointup, pointleft + 1);　　　　　　　　　　　　　　　　　　　　　　  //マップの生成ポイントが移動する向きを決定
            switch (_pointdir)
            {
                case pointup:　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　  //マップ移動ポイントが上の時
                    if (g_pointy > 2)　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　//マップの生成ポイントが上方向に上限に達していない時
                    {
                        if (g_field[g_pointy - 1, g_pointx] == 0 && g_field[g_pointy - 2,g_pointx] == 0 )　　　　　　　//生成ポイントの上にポイントがいる場合
                        {
                            g_field[g_pointy - 1, g_pointx] = 1;　　　　　　　　　　　　　　　　　　　　　　　　　　　 //道を生成
                            g_field[g_pointy - 2, g_pointx] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);//ランダムに部屋を生成
                            g_pointy -= 2;　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　//生成ポイントを一部屋分上にずらす
                            _trueroom += 1;
                        }
                        else if (g_field[g_pointy - 1, g_pointx] == 0)
                        {
                            g_field[g_pointy - 1, g_pointx] = 1;　　　　　　　　　　　　　　　　　　                //道を生成する
                            g_pointy -= 2;
                            i--;　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　                        //そうじゃなかったときルームの数をキープする
                        }
                        else                                                                 
                        {
                            g_pointy -= 2;                                                                          //ただし次の部屋には進む
                            //i--;　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　                        //そうじゃなかったときルームの数をキープする
                        }
                    }
  //                  else
  //                  {
  //                      i--;　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　　                          //マップ上上限の時に部屋の数を変えずにループに戻る
 //                   }
                    break;
                case pointright:　　　　　　　　　　　　　　　　　　　　　　　　　　　　　                          //以下ほぼ同文
                    if (g_pointx < fieldx - 3)
                    {
                        if (g_field[g_pointy, g_pointx + 1] == 0 && g_field[g_pointy, g_pointx + 2] == 0)
                        {
                            g_field[g_pointy, g_pointx + 1] = 2;
                            g_field[g_pointy, g_pointx + 2] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);
                            _trueroom += 1;
                            g_pointx += 2;
                        }
                        else if (g_field[g_pointy, g_pointx + 1] == 0)
                        {
                            g_field[g_pointy, g_pointx + 1] = 2;
                            g_pointx += 2;
                            i--;
                        }
                        else
                        {
                            g_pointx += 2;
                            //i--;
                        }
                    }
 //                   else
//                    {
 //                       i--;
//                    }
                    break;
                case pointdown:
                    if (g_pointy < fieldy - 3)
                    {
                        if (g_field[g_pointy + 1, g_pointx] == 0 && g_field[g_pointy + 2, g_pointx] == 0)
                        {
                            g_field[g_pointy + 1, g_pointx] = 1;
                            g_field[g_pointy + 2, g_pointx] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);
                            _trueroom+= 1;
                            g_pointy += 2;
                        }
                        else if (g_field[g_pointy + 1, g_pointx] == 0)
                        {
                            g_field[g_pointy + 1, g_pointx] = 1;
                            g_pointy += 2;
                            i--;
                        }
                        else
                        {
                            g_pointy += 2;
                            //i--;
                        }
                    }
//                    else
//                    {
//                        i--;
//                    }
                    break;
                case pointleft:
                    if (g_pointx > 2)
                    {
                        if (g_field[g_pointy, g_pointx - 1] == 0 && g_field[g_pointy, g_pointx - 2] == 0)
                        {
                            g_field[g_pointy, g_pointx - 1] = 2;
                            g_field[g_pointy, g_pointx - 2] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);
                            _trueroom += 1; ;
                            g_pointx -= 2;
                        }
                        else if (g_field[g_pointy, g_pointx - 1] == 0)
                        {
                            g_field[g_pointy, g_pointx - 1] = 2;
                            g_pointx -= 2;
                            i--;
                        }
                        else
                        {
                            g_pointx -= 2;
                            //i--;
                        }
                    }
//                    else
//                    {
//                        i--;
//                    }
                    break;
            }
        }
        if(_trueroom < _roommin)//無限ループが発覚したので例外処理　見ないで！！！
        {
            for (int j = 1; j < fieldy / 2; j++) {
                for (int k = 1; k < fieldx / 2; k++){
                    if (j > 0 && j < fieldy / 2 - 1 && k > 0 && k < fieldx / 2 - 1)
                    {
                        if (g_field[j * 2, k * 2] == 0 && g_field[j * 2 + 2, k * 2] >= 10)
                        {
                            g_field[j * 2, k * 2] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);
                            g_field[j * 2 + 1, k * 2] = 1;
                            _trueroom++;
                        }
                        else if (g_field[j * 2, k * 2] == 0 && g_field[j * 2, k * 2 + 2] >= 10)
                        {
                            g_field[j * 2, k * 2] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);
                            g_field[j * 2, k * 2 + 1] = 2;
                            _trueroom++;
                        }
                        else if (g_field[j * 2, k * 2] == 0 && g_field[j * 2 - 2, k * 2] >= 10)
                        {
                            g_field[j * 2, k * 2] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);
                            g_field[j * 2 - 1, k * 2] = 1;
                            _trueroom++;
                        }
                        else if (g_field[j * 2, k * 2] == 0 && g_field[j * 2, k * 2 - 2] >= 10)
                        {
                            g_field[j * 2, k * 2] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);
                            g_field[j * 2, k * 2 - 1] = 2;
                            _trueroom++;
                        }
                        if (_trueroom == _roommin)
                        {
                            k = fieldx;
                            j = fieldy;
                        }
                    }
                }
            }
        }
        #endregion
        g_field[g_nowpositiony, g_nowpositionx] = 1000 * g_nowfloor;//プレイヤーが生成される初期位置のナンバーを入力
    }

    public void Draw() //描画処理
    {
        if (NewPlayer)//もしマップ上にプレイヤーのimageがある時壊す
        {
            Destroy(NewPlayer);
        }
        for (int i = 0; i < fieldy; i++)
        {
            for (int j = 0; j < fieldx; j++)
            {
                switch (g_field[i, j])//生成されたフィールドを一マスごとにimageプレハブで描画していく
                {
                    case 0:
                        break;
                    case 1:
                        Instantiate(roadvar, new Vector2(this.rectTransform.anchoredPosition.x + (j * 20), this.rectTransform.anchoredPosition.y + (i * 20)), Quaternion.identity, this.transform);
                        break;
                    case 2:
                        Instantiate(roadsid, new Vector2(this.rectTransform.anchoredPosition.x + (j * 20), this.rectTransform.anchoredPosition.y + (i * 20)), Quaternion.identity, this.transform);
                        break;
                    default:
                        Instantiate(froom, new Vector2(this.rectTransform.anchoredPosition.x + (j * 20), this.rectTransform.anchoredPosition.y + (i * 20)), Quaternion.identity, this.transform);
                        break;
                }
            }
        }
        NewPlayer = Instantiate(Player, new Vector2(this.rectTransform.anchoredPosition.x + (g_nowpositionx * 20), this.rectTransform.anchoredPosition.y + (g_nowpositiony * 20)), Quaternion.identity, this.transform);//プレイヤープレハブをミニマップ上で生成する
    }

    public void RoomMet(int Dir)//新しい部屋に進む処理
    {
        switch (Dir)//上下左右の方向によって分岐
        {
            case pointup:
                if (g_field[g_nowpositiony + 1, g_nowpositionx] >= 1)//上方向に道があった場合
                {
                    g_nowpositiony += 2;                         //プレイヤーの位置を上の部屋にずらす
                    g_playerdir = pointup;                       //プレイヤーの進行方向を上に設定
                    uproadsc.UpSceneload();                    //上の道に設定されているスクリプトのシーン読み込み関数を選択
                }
                break;
            case pointright:                                   //以下ほぼ同文
                if (g_field[g_nowpositiony, g_nowpositionx] >= 1)
                {
                    g_nowpositionx += 2;
                    g_playerdir = pointright;
                    rightroadsc.RightSceneload();
                }
                break;
            case pointdown:
                if (g_field[g_nowpositiony, g_nowpositionx] >= 1)
                {
                    g_nowpositiony -= 2;
                    g_playerdir = pointdown;
                    downroadsc.DownSceneload();
                }
                break;
            case pointleft:
                if (g_field[g_nowpositiony, g_nowpositionx] >= 1)
                {
                    g_nowpositionx -= 2;
                    g_playerdir = pointleft;
                    leftroadsc.LeftSceneload();
                }
                break;
            default:
                break;
        }
        Destroy(NewPlayer);//プレイヤーのプレハブを壊す
        NewPlayer = Instantiate(Player, new Vector2(this.rectTransform.anchoredPosition.x + (g_nowpositionx * 20), this.rectTransform.anchoredPosition.y + (g_nowpositiony * 20)), Quaternion.identity, this.transform);//新しく移動後のプレイヤーを生成する
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)//シーンがロードされたとき
    {
        switch (g_playerdir)
        {//プレイヤーがシーンをまたいだ時の向きによって生成位置を変える
            case 1://上方向の時に
                TObj = GameObject.Find("DownRoadTrigger");//下の道を取得
                mainchar.transform.position = new Vector3(TObj.transform.position.x, TObj.transform.position.y + 2, TObj.transform.position.z);//対応した位置にプレイヤーを移動
                NewRoom();//マップジェネレートスクリプトにある新しい部屋に入った時の処理を追加
                break;//以下ほぼ同文
            case 2:
                TObj = GameObject.Find("LeftRoadTrigger");
                mainchar.transform.position = new Vector3(TObj.transform.position.x + 2, TObj.transform.position.y, TObj.transform.position.z);
                NewRoom();
                break;
            case 3:
                TObj = GameObject.Find("UpRoadTrigger");
                mainchar.transform.position = new Vector3(TObj.transform.position.x, TObj.transform.position.y - 2, TObj.transform.position.z);
                NewRoom();
                break;
            case 4:
                TObj = GameObject.Find("RightRoadTrigger");
                mainchar.transform.position = new Vector3(TObj.transform.position.x - 2, TObj.transform.position.y, TObj.transform.position.z);
                NewRoom();
                break;
            default:
                break;
        }
        if (g_pointx == g_nowpositionx && g_pointy == g_nowpositiony)//生成の最終ポイントと今のプレイやーの位置が同じ場合
        {
            //Instantiate(stairs, new Vector3(10,10,0), Quaternion.identity, this.transform);//次の階層に行く階段を生成する
        }
    }
    #endregion
}
