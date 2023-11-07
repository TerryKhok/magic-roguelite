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

    [SerializeField]
    int _startpointx;
    [SerializeField]
    int _startpointy;

    int _pointdir = 0;//マップを生成する方向
    public int g_pointy;//マップ生成の現在位置Ｙ
    public int g_pointx;//マップ生成の現在位置Ｘ
    [SerializeField]
    int _room;//生成する部屋の最大数
    [SerializeField]
    int _roommin;//生成する部屋の最小数
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
    GameObject bossroom;//ボスのプレハブ
    GameObject shoproom;//ショップのプレハブ
    GameObject sclroompre;//隠し部屋のプレハブ

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
    void Awake()
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
        rectTransform = GetComponent<RectTransform>();//レクトトランスフォームのコンポーネントを取得
        roadvar = (GameObject)Resources.Load("MapResource/roadvar");//縦の道のプレハブを取得
        roadsid = (GameObject)Resources.Load("MapResource/roadsid");//横の道のプレハブを取得
        froom = (GameObject)Resources.Load("MapResource/froom");//部屋のプレハブを取得
        Player = (GameObject)Resources.Load("MapResource/Player");//プレイヤーのプレハブを取得
        bossroom = (GameObject)Resources.Load("MapResource/bossroom");//ボスのプレハブを取得
        mainchar = TObj = GameObject.Find("player_test");//メインキャラを取得
        shoproom = (GameObject)Resources.Load("MapResource/shoproom");//ボスのプレハブを取得
        sclroompre = (GameObject)Resources.Load("MapResource/sclroom");//ボスのプレハブを取得
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void Start()
    {
        Init();//初期更新処理
        Draw();//描画処理
        NewRoom();//新しい部屋に入った処理
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
        if (g_nowpositiony > 1)//存在しない配列を参照しないため
        {
            uproadsc.UpRoadGenerate(g_field[g_nowpositiony - 1, g_nowpositionx]);//カベがあるかどうかの確認を引数にあったらカベを生成なかったらトリガーになる
        }
        else
        {
            uproadsc.UpRoadGenerate(0);//存在しない配列を参照したらカベを生成
        }
        if (g_nowpositionx < fieldx - 1)
        {
            rightroadsc.RightRoadGenerate(g_field[g_nowpositiony, g_nowpositionx + 1]);
        }
        else
        {
            rightroadsc.RightRoadGenerate(0);
        }
        if (g_nowpositiony < fieldy - 1)
        {
            downroadsc.DownRoadGenerate(g_field[g_nowpositiony + 1, g_nowpositionx]);
        }
        else
        {
            downroadsc.DownRoadGenerate(0);
        }
        if (g_nowpositionx > 1)
        {
            leftroadsc.LeftRoadGenerate(g_field[g_nowpositiony, g_nowpositionx - 1]);
        }
        else
        {
            leftroadsc.LeftRoadGenerate(0);
        }
    }
    public void Init()//初期更新処理
    {
        int roadcounter;
        int trueroom;//実際に生成する部屋の数
        int mapbranch;//実際に生成する部屋の分岐数
        int storage;
        int branchcount;
        int bossroomx = _startpointx;
        int bossroomy = _startpointy;
        int setbossstorage = 0;
        int getbossstorage = 0;
        int shoproom = 1;
        int shoproomx = _startpointx;
        int shoproomy = _startpointy;
        g_nowfloor += 1;    ///
        g_nowpositionx = _startpointx; //
        g_nowpositiony = _startpointy; //更新時に初期化する
        g_pointx = _startpointx;　　　 //
        g_pointy = _startpointy;       ///
        trueroom = UnityEngine.Random.Range(_roommin,_room + 1);//生成する部屋の数の設定
        mapbranch = UnityEngine.Random.Range(2,5);//初期部屋の分岐数を設定
        branchcount = trueroom / mapbranch;
        for (int i = 0; i < fieldy; i++)
        {
            for (int j = 0; j < fieldx; j++)
            {
                g_field[i, j] = 0;//フィールド初期化処理
            }
        }
        for(int i = 0;i < trueroom;i++)//ルームの数だけループ
        {
            roadcounter = 0;
            if (i % branchcount == 0 && i > 1)//必要分岐数に応じて生成位置を変える
            {
                g_pointx = _startpointx;
                g_pointy = _startpointy;
            }

            //なんか考えるの面倒になったから力技行きます
            //道が生成されてないところを記憶
            if (g_pointy > 1)//存在しない配列を参照しないためのネスト
            {
                if (g_field[g_pointy - 1, g_pointx] == 0)
                {
                    roadcounter += 1;
                }
            }
            if (g_pointx < fieldx - 2)
            {
                if (g_field[g_pointy, g_pointx + 1] == 0)
                {
                    roadcounter += 10;
                }
            }
            if (g_pointy < fieldy - 2) {
                if (g_field[g_pointy + 1, g_pointx] == 0)
                {
                    roadcounter += 100;
                }
            }
            if (g_pointx > 1) {
                if (g_field[g_pointy, g_pointx - 1] == 0)
                {
                    roadcounter += 1000;
                }
            }
            //その道に応じて乱数で次の部屋につながる道を生成する方向を指定
            switch (roadcounter) {
                case 0:
                    if (g_pointy > 1)
                    {
                        g_pointy -= 2;
                    }
                    else
                    {
                        g_pointx += 2;
                    }
                    //i -= 1;
                    break;
                case 1:
                    MapRoom(1);//上の道しか選択肢がない
                    break;
                case 11:
                    MapRoom(UnityEngine.Random.Range(pointup,pointright + 1));
                    break;
                case 101:
                    storage = UnityEngine.Random.Range(pointup, pointright + 1);
                        switch (storage) {
                        case 1:
                            MapRoom(1);
                            break;
                        case 2:
                            MapRoom(3);
                            break;
                        default:
                            Debug.Log("101エラー");
                            break;
                        }
                    break;
                case 1001:
                    storage = UnityEngine.Random.Range(pointup, pointright + 1);
                        switch (storage)
                        {
                        case 1:
                            MapRoom(1);
                            break;
                        case 2:
                            MapRoom(4);
                            break;
                        default:
                            Debug.Log("1001エラー");
                        break;
                    }
                    break;
                case 111:
                    storage = UnityEngine.Random.Range(pointup, pointright + 2);
                    switch (storage)
                    {
                        case 1:
                            MapRoom(1);
                            break;
                        case 2:
                            MapRoom(2);
                            break;
                        case 3:
                            MapRoom(3);
                            break;
                        default:
                            Debug.Log("111エラー");
                        break;
                    }
                    break;
                case 1111:
                    MapRoom(UnityEngine.Random.Range(pointup, pointleft + 1));
                    break;
                case 1011:
                    storage = UnityEngine.Random.Range(pointup, pointright + 2);
                    switch (storage)
                    {
                        case 1:
                            MapRoom(1);
                            break;
                        case 2:
                            MapRoom(2);
                            break;
                        case 3:
                            MapRoom(4);
                            break;
                        default:
                            Debug.Log("1011エラー");
                            break;
                    }
                    break;
                case 1101:
                    storage = UnityEngine.Random.Range(pointup, pointright + 1);
                    switch (storage)
                    {
                        case 1:
                            MapRoom(1);
                            break;
                        case 2:
                            MapRoom(3);
                            break;
                        case 3:
                            MapRoom(4);
                            break;
                        default:
                            Debug.Log("1101エラー");
                            break;
                    }
                    break;
                case 10:
                    MapRoom(2);//右の道しか選択肢がない
                    break;
                case 100:
                    MapRoom(3);//下の道しか選択肢がない
                    break;
                case 1000:
                    MapRoom(4);//左の道しか選択肢がない
                    break;
                case 110:
                    storage = UnityEngine.Random.Range(pointup, pointright + 1);
                    switch (storage)
                    {
                        case 1:
                            MapRoom(2);
                            break;
                        case 2:
                            MapRoom(3);
                            break;
                        default:
                            Debug.Log("110エラー");
                            break;
                    }
                    break;
                case 1010:
                    storage = UnityEngine.Random.Range(pointup, pointright + 1);
                    switch (storage)
                    {
                        case 1:
                            MapRoom(2);
                            break;
                        case 2:
                            MapRoom(4);
                            break;
                        default:
                            Debug.Log("1010エラー");
                            break;
                    }
                    break;
                case 1100:
                    storage = UnityEngine.Random.Range(pointup, pointright + 1);
                    switch (storage)
                    {
                        case 1:
                            MapRoom(3);
                            break;
                        case 2:
                            MapRoom(4);
                            break;
                        default:
                            Debug.Log("1100エラー");
                            break;
                    }
                    break;
                case 1110:
                    storage = UnityEngine.Random.Range(pointup, pointright + 2);
                    switch (storage)
                    {
                        case 1:
                            MapRoom(2);
                            break;
                        case 2:
                            MapRoom(3);
                            break;
                        case 3:
                            MapRoom(4);
                            break;
                        default:
                            Debug.Log("1110エラー");
                            break;
                    }
                    break;
            }
            //ボス部屋の生成先の設定
            getbossstorage = 0;
            if (g_pointx <= _startpointx)
            {
                getbossstorage += (g_pointx - _startpointx) * -1;
            }
            else
            {
                getbossstorage += g_pointx - _startpointx;
            }
            if (g_pointy <= _startpointy)
            {
                getbossstorage += (g_pointy - _startpointy) * -1;
            }
            else
            {
                getbossstorage += g_pointy - _startpointy;
            }
            if (setbossstorage <= getbossstorage)//ボスルームの遠さを査定
            {
                setbossstorage = getbossstorage;
                bossroomx = g_pointx;
                bossroomy = g_pointy;
            }
            if (getbossstorage >= 4 && shoproom == 1)
            {
                shoproomx = g_pointx;
                shoproomy = g_pointy;
                shoproom--;
            }

        }
        sclroom();//シークレットルーム作成
        g_field[g_nowpositiony, g_nowpositionx] = 1000 * g_nowfloor;//プレイヤーが生成される初期位置のナンバーを入力
        if (shoproomx == bossroomx && shoproomy == bossroomy)
        {
            if (g_field[_startpointy - 2,_startpointx] >= 10)
            {
                g_field[_startpointy - 2,_startpointx] = 20000 + g_nowfloor;//ショップルーム生成
            }
            else if(g_field[_startpointy, _startpointx + 2] >= 10)
            {
                g_field[_startpointy, _startpointx + 2] = 20000 + g_nowfloor;
            }
            else
            {
                g_field[_startpointy, _startpointx - 2] = 20000 + g_nowfloor;
            }
        }
        else
        {
            g_field[shoproomy, shoproomx] = 20000 + g_nowfloor;//ショップルーム生成
        }
        g_field[bossroomy, bossroomx] = 10000 + g_nowfloor;//ボスルーム生成
    }


    //1000番台は初期部屋
    //10000番台はボス部屋
    //20000番台はショップ
    //30000番台はシークレットルーム
    //
    //
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
                        Instantiate(roadvar, new Vector2(this.rectTransform.anchoredPosition.x + (j * 20), this.rectTransform.anchoredPosition.y + (i * -20)), Quaternion.identity, this.transform);
                        break;
                    case 2:
                        Instantiate(roadsid, new Vector2(this.rectTransform.anchoredPosition.x + (j * 20), this.rectTransform.anchoredPosition.y + (i * -20)), Quaternion.identity, this.transform);
                    break;
                    case 3:
                    case 4:
                        break;
                    case 10001:
                    case 10002:
                    case 10003:
                        Instantiate(bossroom, new Vector2(this.rectTransform.anchoredPosition.x + (j * 20), this.rectTransform.anchoredPosition.y + (i * -20)), Quaternion.identity, this.transform);
                        break;
                    case 20001:
                    case 20002:
                    case 20003:
                        Instantiate(shoproom, new Vector2(this.rectTransform.anchoredPosition.x + (j * 20), this.rectTransform.anchoredPosition.y + (i * -20)), Quaternion.identity, this.transform);
                        break;
                    case 30001:
                    case 30002:
                    case 30003:
                        Instantiate(sclroompre, new Vector2(this.rectTransform.anchoredPosition.x + (j * 20), this.rectTransform.anchoredPosition.y + (i * -20)), Quaternion.identity, this.transform);
                        break;
                    default:
                        Instantiate(froom, new Vector2(this.rectTransform.anchoredPosition.x + (j * 20), this.rectTransform.anchoredPosition.y + (i * -20)), Quaternion.identity, this.transform);
                        break;
                }
            }
        }
        NewPlayer = Instantiate(Player, new Vector2(this.rectTransform.anchoredPosition.x + (g_nowpositionx * 20), this.rectTransform.anchoredPosition.y + (g_nowpositiony * -20)), Quaternion.identity, this.transform);//プレイヤープレハブをミニマップ上で生成する
    }

    public void RoomMet(int Dir)//新しい部屋に進む処理
    {
        switch (Dir)//上下左右の方向によって分岐
        {
            case pointup:
                    g_nowpositiony -= 2;                         //プレイヤーの位置を上の部屋にずらす
                    g_playerdir = pointup;                       //プレイヤーの進行方向を上に設定
                    uproadsc.UpSceneload();                    //上の道に設定されているスクリプトのシーン読み込み関数を選択
                break;
            case pointright:                                   //以下ほぼ同文
                    g_nowpositionx += 2;
                    g_playerdir = pointright;
                    rightroadsc.RightSceneload();
                break;
            case pointdown:
                    g_nowpositiony += 2;
                    g_playerdir = pointdown;
                    downroadsc.DownSceneload();
                break;
            case pointleft:
                    g_nowpositionx -= 2;
                    g_playerdir = pointleft;
                    leftroadsc.LeftSceneload();
                break;
            default:
                break;
        }
        Destroy(NewPlayer);//プレイヤーのプレハブを壊す
        NewPlayer = Instantiate(Player, new Vector2(this.rectTransform.anchoredPosition.x + (g_nowpositionx * 20), this.rectTransform.anchoredPosition.y + (g_nowpositiony * -20)), Quaternion.identity, this.transform);//新しく移動後のプレイヤーを生成する
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
        /*
        if (g_pointx == g_nowpositionx && g_pointy == g_nowpositiony)//生成の最終ポイントと今のプレイやーの位置が同じ場合
        {
            Instantiate(stairs, new Vector3(10,10,0), Quaternion.identity, this.transform);//次の階層に行く階段を生成する
        }
        */
    }
    void MapRoom(int dir)
    {
        switch (dir) {
            case 1:
                if (g_field[g_pointy - 2, g_pointx] == 0)       //生成ポイントの上にポイントがいる場合
                {
                    g_field[g_pointy - 1, g_pointx] = 1;                            //道を生成
                    g_field[g_pointy - 2, g_pointx] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);//ランダムに部屋を生成
                    g_pointy -= 2;                                      //生成ポイントを一部屋分上にずらす
                }
                else
                {
                    g_field[g_pointy - 1, g_pointx] = 1;
                    g_pointy -= 2;
                }
                break;
            case 2:
                if (g_field[g_pointy, g_pointx + 2] == 0)       //生成ポイントの右にポイントがいる場合
                {
                    g_field[g_pointy, g_pointx + 1] = 2;                            //道を生成
                    g_field[g_pointy, g_pointx + 2] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);//ランダムに部屋を生成
                    g_pointx += 2;                                      //生成ポイントを一部屋分上にずらす
                }
                else
                {
                    g_field[g_pointy, g_pointx + 1] = 2;
                    g_pointx += 2;
                }
                break;
            case 3:
                if (g_field[g_pointy + 2, g_pointx] == 0)       //生成ポイントの下にポイントがいる場合
                {
                    g_field[g_pointy + 1, g_pointx] = 1;                            //道を生成
                    g_field[g_pointy + 2, g_pointx] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);//ランダムに部屋を生成
                    g_pointy += 2;                                      //生成ポイントを一部屋分上にずらす
                }
                else
                {
                    g_field[g_pointy + 1, g_pointx] = 1;
                    g_pointy += 2;
                }
                break;
            case 4:
                if (g_field[g_pointy, g_pointx - 2] == 0)       //生成ポイントの左にポイントがいる場合
                {
                    g_field[g_pointy, g_pointx - 1] = 2;                            //道を生成
                    g_field[g_pointy , g_pointx - 2] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);//ランダムに部屋を生成
                    g_pointx -= 2;                                      //生成ポイントを一部屋分上にずらす
                }
                else
                {
                    g_field[g_pointy, g_pointx - 1] = 2;
                    g_pointx -= 2;
                }
                break;
        }
    }

    void sclroom()
    {
        int sclroomnum = 1;
        while (sclroomnum != 0)
        {
            switch (UnityEngine.Random.Range(pointup, pointright + 1)) {
                case pointup:
                    if (g_pointy > 1)
                    {
                        if (g_field[g_pointy - 2, g_pointx] >= 10)
                        {
                            g_pointy -= 2;
                        }
                        else
                        {
                            g_field[g_pointy - 1, g_pointx] = 3;                            //道を生成
                            g_field[g_pointy - 2, g_pointx] = 30000 + g_nowfloor;//ランダムに部屋を生成
                            sclroomnum--;
                        }
                    }
                    else
                    {
                        g_pointy = _startpointy;
                        g_pointx = _startpointx;
                    }
                    break;
                case pointright:
                    if (g_pointx < fieldx - 2)
                    {
                        if (g_field[g_pointy, g_pointx + 2] >= 10)
                        {
                            g_pointx += 2;
                        }
                        else
                        {
                            g_field[g_pointy, g_pointx + 1] = 4;                            //道を生成
                            g_field[g_pointy, g_pointx + 2] = 30000 + g_nowfloor;//ランダムに部屋を生成
                            sclroomnum--;
                        }
                    }
                    else
                    {
                        g_pointy = _startpointy;
                        g_pointx = _startpointx;
                    }
                    break;
                case pointdown:
                    if (g_pointy < fieldy - 2)
                    {
                        if (g_field[g_pointy + 2, g_pointx] >= 10)
                        {
                            g_pointy += 2;
                        }
                        else
                        {
                            g_field[g_pointy + 1, g_pointx] = 3;                            //道を生成
                            g_field[g_pointy + 2, g_pointx] = 30000 + g_nowfloor;//ランダムに部屋を生成
                            sclroomnum--;
                        }
                    }
                    else
                    {
                        g_pointy = _startpointy;
                        g_pointx = _startpointx;
                    }
                    break;
                case pointleft:
                    if (g_pointx > 1)
                    {
                        if (g_field[g_pointy, g_pointx - 2] >= 10)
                        {
                            g_pointx -= 2;
                        }
                        else
                        {
                            g_field[g_pointy, g_pointx - 1] = 4;                            //道を生成
                            g_field[g_pointy, g_pointx - 2] = 30000 + g_nowfloor;//ランダムに部屋を生成
                            sclroomnum--;
                        }
                    }
                    else
                    {
                        g_pointy = _startpointy;
                        g_pointx = _startpointx;
                    }
                    break;
            }
        }
    }
    #endregion
}
