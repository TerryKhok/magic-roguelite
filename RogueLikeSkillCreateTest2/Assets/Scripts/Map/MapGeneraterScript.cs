using Mono.Cecil.Pdb;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class MapGeneraterScript : MonoBehaviour //�~�j�}�b�v�𐶐����邽�߂̃X�N���v�g
{
    #region�@�ϐ�
    public static MapGeneraterScript Instance;
    const int fieldy = 12;//�t�B�[���h�̑傫���x
    const int fieldx = 12;//�t�B�[���h�̑傫���w

    const int pointup = 1;//�����
    const int pointright = 2;//�E����
    const int pointdown = 3;//������
    const int pointleft = 4;//�E����

    [SerializeField]
    int _startpointx;
    [SerializeField]
    int _startpointy;

    int _pointdir = 0;//�}�b�v�𐶐��������
    public int g_pointy;//�}�b�v�����̌��݈ʒu�x
    public int g_pointx;//�}�b�v�����̌��݈ʒu�w
    [SerializeField]
    int _room;//�������镔���̍ő吔
    [SerializeField]
    int _roommin;//�������镔���̍ŏ���
    public int g_playerdir;//�������܂������v���C���[�̕���
    public int[,] g_field = new int[fieldy, fieldx];//�t�B�[���h�̏�����

    public int g_nowpositionx = 4;//���̃v���C���[�̃|�W�V����X
    public int g_nowpositiony = 4;//���̃v���C���[�̃|�W�V����Y

    public int g_nowfloor = 0;//���݂̊K�w
    #endregion

    #region �^��`
    GameObject roadvar;//�c�̓��̃v���n�u
    GameObject roadsid;//���̓��̃v���n�u
    GameObject froom;//�����̃v���n�u
    GameObject Player;//�v���C���[�̃v���n�u
    GameObject bossroom;//�{�X�̃v���n�u
    GameObject shoproom;//�V���b�v�̃v���n�u
    GameObject sclroompre;//�B�������̃v���n�u

    GameObject NewPlayer;//image�v���C���[�{��

    RectTransform rectTransform;//Canvas�ˑ��̃g�����X�t�H�[��

    RightRoadScript rightroadsc;
    UpRoadScript uproadsc;
    DownRoadScript downroadsc;
    LeftRoadScript leftroadsc;
    GameObject TObj;//�g���܂킷�p�Q�[��OBJ
    GameObject mainchar;//�v���C���[�̖{��
    #endregion

    #region Unity�ˑ��֐�
    void Awake()
    {
        // �V���O���g���̎���
        if (Instance == null)
        {
            // ���g���C���X�^���X�Ƃ���
            Instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(Instance);//���̃I�u�W�F�N�g���󂳂Ȃ�
        rectTransform = GetComponent<RectTransform>();//���N�g�g�����X�t�H�[���̃R���|�[�l���g���擾
        roadvar = (GameObject)Resources.Load("MapResource/roadvar");//�c�̓��̃v���n�u���擾
        roadsid = (GameObject)Resources.Load("MapResource/roadsid");//���̓��̃v���n�u���擾
        froom = (GameObject)Resources.Load("MapResource/froom");//�����̃v���n�u���擾
        Player = (GameObject)Resources.Load("MapResource/Player");//�v���C���[�̃v���n�u���擾
        bossroom = (GameObject)Resources.Load("MapResource/bossroom");//�{�X�̃v���n�u���擾
        mainchar = TObj = GameObject.Find("player_test");//���C���L�������擾
        shoproom = (GameObject)Resources.Load("MapResource/shoproom");//�{�X�̃v���n�u���擾
        sclroompre = (GameObject)Resources.Load("MapResource/sclroom");//�{�X�̃v���n�u���擾
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    void Start()
    {
        Init();//�����X�V����
        Draw();//�`�揈��
        NewRoom();//�V���������ɓ���������
    }
    #endregion

    #region�@����֐�
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
        if (g_nowpositiony > 1)//���݂��Ȃ��z����Q�Ƃ��Ȃ�����
        {
            uproadsc.UpRoadGenerate(g_field[g_nowpositiony - 1, g_nowpositionx]);//�J�x�����邩�ǂ����̊m�F�������ɂ�������J�x�𐶐��Ȃ�������g���K�[�ɂȂ�
        }
        else
        {
            uproadsc.UpRoadGenerate(0);//���݂��Ȃ��z����Q�Ƃ�����J�x�𐶐�
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
    public void Init()//�����X�V����
    {
        int roadcounter;
        int trueroom;//���ۂɐ������镔���̐�
        int mapbranch;//���ۂɐ������镔���̕���
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
        g_nowpositiony = _startpointy; //�X�V���ɏ���������
        g_pointx = _startpointx;�@�@�@ //
        g_pointy = _startpointy;       ///
        trueroom = UnityEngine.Random.Range(_roommin,_room + 1);//�������镔���̐��̐ݒ�
        mapbranch = UnityEngine.Random.Range(2,5);//���������̕��򐔂�ݒ�
        branchcount = trueroom / mapbranch;
        for (int i = 0; i < fieldy; i++)
        {
            for (int j = 0; j < fieldx; j++)
            {
                g_field[i, j] = 0;//�t�B�[���h����������
            }
        }
        for(int i = 0;i < trueroom;i++)//���[���̐��������[�v
        {
            roadcounter = 0;
            if (i % branchcount == 0 && i > 1)//�K�v���򐔂ɉ����Đ����ʒu��ς���
            {
                g_pointx = _startpointx;
                g_pointy = _startpointy;
            }

            //�Ȃ񂩍l����̖ʓ|�ɂȂ�������͋Z�s���܂�
            //������������ĂȂ��Ƃ�����L��
            if (g_pointy > 1)//���݂��Ȃ��z����Q�Ƃ��Ȃ����߂̃l�X�g
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
            //���̓��ɉ����ė����Ŏ��̕����ɂȂ��铹�𐶐�����������w��
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
                    MapRoom(1);//��̓������I�������Ȃ�
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
                            Debug.Log("101�G���[");
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
                            Debug.Log("1001�G���[");
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
                            Debug.Log("111�G���[");
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
                            Debug.Log("1011�G���[");
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
                            Debug.Log("1101�G���[");
                            break;
                    }
                    break;
                case 10:
                    MapRoom(2);//�E�̓������I�������Ȃ�
                    break;
                case 100:
                    MapRoom(3);//���̓������I�������Ȃ�
                    break;
                case 1000:
                    MapRoom(4);//���̓������I�������Ȃ�
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
                            Debug.Log("110�G���[");
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
                            Debug.Log("1010�G���[");
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
                            Debug.Log("1100�G���[");
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
                            Debug.Log("1110�G���[");
                            break;
                    }
                    break;
            }
            //�{�X�����̐�����̐ݒ�
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
            if (setbossstorage <= getbossstorage)//�{�X���[���̉���������
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
        sclroom();//�V�[�N���b�g���[���쐬
        g_field[g_nowpositiony, g_nowpositionx] = 1000 * g_nowfloor;//�v���C���[����������鏉���ʒu�̃i���o�[�����
        if (shoproomx == bossroomx && shoproomy == bossroomy)
        {
            if (g_field[_startpointy - 2,_startpointx] >= 10)
            {
                g_field[_startpointy - 2,_startpointx] = 20000 + g_nowfloor;//�V���b�v���[������
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
            g_field[shoproomy, shoproomx] = 20000 + g_nowfloor;//�V���b�v���[������
        }
        g_field[bossroomy, bossroomx] = 10000 + g_nowfloor;//�{�X���[������
    }


    //1000�ԑ�͏�������
    //10000�ԑ�̓{�X����
    //20000�ԑ�̓V���b�v
    //30000�ԑ�̓V�[�N���b�g���[��
    //
    //
    public void Draw() //�`�揈��
    {
        if (NewPlayer)//�����}�b�v��Ƀv���C���[��image�����鎞��
        {
            Destroy(NewPlayer);
        }
        for (int i = 0; i < fieldy; i++)
        {
            for (int j = 0; j < fieldx; j++)
            {
                switch (g_field[i, j])//�������ꂽ�t�B�[���h����}�X���Ƃ�image�v���n�u�ŕ`�悵�Ă���
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
        NewPlayer = Instantiate(Player, new Vector2(this.rectTransform.anchoredPosition.x + (g_nowpositionx * 20), this.rectTransform.anchoredPosition.y + (g_nowpositiony * -20)), Quaternion.identity, this.transform);//�v���C���[�v���n�u���~�j�}�b�v��Ő�������
    }

    public void RoomMet(int Dir)//�V���������ɐi�ޏ���
    {
        switch (Dir)//�㉺���E�̕����ɂ���ĕ���
        {
            case pointup:
                    g_nowpositiony -= 2;                         //�v���C���[�̈ʒu����̕����ɂ��炷
                    g_playerdir = pointup;                       //�v���C���[�̐i�s��������ɐݒ�
                    uproadsc.UpSceneload();                    //��̓��ɐݒ肳��Ă���X�N���v�g�̃V�[���ǂݍ��݊֐���I��
                break;
            case pointright:                                   //�ȉ��قړ���
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
        Destroy(NewPlayer);//�v���C���[�̃v���n�u����
        NewPlayer = Instantiate(Player, new Vector2(this.rectTransform.anchoredPosition.x + (g_nowpositionx * 20), this.rectTransform.anchoredPosition.y + (g_nowpositiony * -20)), Quaternion.identity, this.transform);//�V�����ړ���̃v���C���[�𐶐�����
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)//�V�[�������[�h���ꂽ�Ƃ�
    {
        switch (g_playerdir)
        {//�v���C���[���V�[�����܂��������̌����ɂ���Đ����ʒu��ς���
            case 1://������̎���
                TObj = GameObject.Find("DownRoadTrigger");//���̓����擾
                mainchar.transform.position = new Vector3(TObj.transform.position.x, TObj.transform.position.y + 2, TObj.transform.position.z);//�Ή������ʒu�Ƀv���C���[���ړ�
                NewRoom();//�}�b�v�W�F�l���[�g�X�N���v�g�ɂ���V���������ɓ��������̏�����ǉ�
                break;//�ȉ��قړ���
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
        if (g_pointx == g_nowpositionx && g_pointy == g_nowpositiony)//�����̍ŏI�|�C���g�ƍ��̃v���C��[�̈ʒu�������ꍇ
        {
            Instantiate(stairs, new Vector3(10,10,0), Quaternion.identity, this.transform);//���̊K�w�ɍs���K�i�𐶐�����
        }
        */
    }
    void MapRoom(int dir)
    {
        switch (dir) {
            case 1:
                if (g_field[g_pointy - 2, g_pointx] == 0)       //�����|�C���g�̏�Ƀ|�C���g������ꍇ
                {
                    g_field[g_pointy - 1, g_pointx] = 1;                            //���𐶐�
                    g_field[g_pointy - 2, g_pointx] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);//�����_���ɕ����𐶐�
                    g_pointy -= 2;                                      //�����|�C���g���ꕔ������ɂ��炷
                }
                else
                {
                    g_field[g_pointy - 1, g_pointx] = 1;
                    g_pointy -= 2;
                }
                break;
            case 2:
                if (g_field[g_pointy, g_pointx + 2] == 0)       //�����|�C���g�̉E�Ƀ|�C���g������ꍇ
                {
                    g_field[g_pointy, g_pointx + 1] = 2;                            //���𐶐�
                    g_field[g_pointy, g_pointx + 2] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);//�����_���ɕ����𐶐�
                    g_pointx += 2;                                      //�����|�C���g���ꕔ������ɂ��炷
                }
                else
                {
                    g_field[g_pointy, g_pointx + 1] = 2;
                    g_pointx += 2;
                }
                break;
            case 3:
                if (g_field[g_pointy + 2, g_pointx] == 0)       //�����|�C���g�̉��Ƀ|�C���g������ꍇ
                {
                    g_field[g_pointy + 1, g_pointx] = 1;                            //���𐶐�
                    g_field[g_pointy + 2, g_pointx] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);//�����_���ɕ����𐶐�
                    g_pointy += 2;                                      //�����|�C���g���ꕔ������ɂ��炷
                }
                else
                {
                    g_field[g_pointy + 1, g_pointx] = 1;
                    g_pointy += 2;
                }
                break;
            case 4:
                if (g_field[g_pointy, g_pointx - 2] == 0)       //�����|�C���g�̍��Ƀ|�C���g������ꍇ
                {
                    g_field[g_pointy, g_pointx - 1] = 2;                            //���𐶐�
                    g_field[g_pointy , g_pointx - 2] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);//�����_���ɕ����𐶐�
                    g_pointx -= 2;                                      //�����|�C���g���ꕔ������ɂ��炷
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
                            g_field[g_pointy - 1, g_pointx] = 3;                            //���𐶐�
                            g_field[g_pointy - 2, g_pointx] = 30000 + g_nowfloor;//�����_���ɕ����𐶐�
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
                            g_field[g_pointy, g_pointx + 1] = 4;                            //���𐶐�
                            g_field[g_pointy, g_pointx + 2] = 30000 + g_nowfloor;//�����_���ɕ����𐶐�
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
                            g_field[g_pointy + 1, g_pointx] = 3;                            //���𐶐�
                            g_field[g_pointy + 2, g_pointx] = 30000 + g_nowfloor;//�����_���ɕ����𐶐�
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
                            g_field[g_pointy, g_pointx - 1] = 4;                            //���𐶐�
                            g_field[g_pointy, g_pointx - 2] = 30000 + g_nowfloor;//�����_���ɕ����𐶐�
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
