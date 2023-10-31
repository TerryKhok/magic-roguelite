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

    int _pointdir = 0;//�}�b�v�𐶐��������
    public int g_pointy = 4;//�}�b�v�����̌��݈ʒu�x
    public int g_pointx = 4;//�}�b�v�����̌��݈ʒu�w
    [SerializeField]
    int _room = 15;//�������镔���̍ő吔
    [SerializeField]
    int _roommin = 8;//�������镔���̍ŏ���

    int _trueroom = 0;

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
    public void Awake()
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
    }
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();//���N�g�g�����X�t�H�[���̃R���|�[�l���g���擾
        roadvar = (GameObject)Resources.Load("MapResource/roadvar");//�c�̓��̃v���n�u���擾
        roadsid = (GameObject)Resources.Load("MapResource/roadsid");//���̓��̃v���n�u���擾
        froom = (GameObject)Resources.Load("MapResource/froom");//�����̃v���n�u���擾
        Player = (GameObject)Resources.Load("MapResource/Player");//�v���C���[�̃v���n�u���擾
        mainchar = TObj = GameObject.Find("PlayerMoveR (1)");//���C���L�������擾
        SceneManager.sceneLoaded += OnSceneLoaded;
        Init();//�X�V����
        Draw();//�`�揈��
        NewRoom();
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
        uproadsc.UpRoadGenerate(g_field[g_nowpositiony + 1, g_nowpositionx]);
        rightroadsc.RightRoadGenerate(g_field[g_nowpositiony, g_nowpositionx + 1]);
        downroadsc.DownRoadGenerate(g_field[g_nowpositiony - 1, g_nowpositionx]);
        leftroadsc.LeftRoadGenerate(g_field[g_nowpositiony, g_nowpositionx - 1]);
    }
    public void Init()//�X�V����
    {
        #region ���������A���S���Y��
        g_nowfloor += 1;    ///
        g_nowpositionx = 4; //
        g_nowpositiony = 4; //�X�V���ɏ���������
        g_pointx = 4;�@�@�@ //
        g_pointy = 4;       ///
        _trueroom = 0;
        for (int i = 0; i < fieldy; i++)
        {
            for (int j = 0; j < fieldx; j++)
            {
                g_field[i, j] = 0;//�t�B�[���h����������
            }
        }
        for (int i = 0; i < _room; i++)
        {
            if(i % (_room / 2) == 0)                                                                                   //�����̐������񕪂̈ꂨ�������
            {
                g_pointx = 4;                                                                                          //���������ʒu������������
                g_pointy = 4;
            }
            _pointdir = UnityEngine.Random.Range(pointup, pointleft + 1);�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@  //�}�b�v�̐����|�C���g���ړ��������������
            switch (_pointdir)
            {
                case pointup:�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@  //�}�b�v�ړ��|�C���g����̎�
                    if (g_pointy > 2)�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@//�}�b�v�̐����|�C���g��������ɏ���ɒB���Ă��Ȃ���
                    {
                        if (g_field[g_pointy - 1, g_pointx] == 0 && g_field[g_pointy - 2,g_pointx] == 0 )�@�@�@�@�@�@�@//�����|�C���g�̏�Ƀ|�C���g������ꍇ
                        {
                            g_field[g_pointy - 1, g_pointx] = 1;�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@ //���𐶐�
                            g_field[g_pointy - 2, g_pointx] = UnityEngine.Random.Range(g_nowfloor * 10, 10 + g_nowfloor * 10);//�����_���ɕ����𐶐�
                            g_pointy -= 2;�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@//�����|�C���g���ꕔ������ɂ��炷
                            _trueroom += 1;
                        }
                        else if (g_field[g_pointy - 1, g_pointx] == 0)
                        {
                            g_field[g_pointy - 1, g_pointx] = 1;�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@                //���𐶐�����
                            g_pointy -= 2;
                            i--;�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@                        //��������Ȃ������Ƃ����[���̐����L�[�v����
                        }
                        else                                                                 
                        {
                            g_pointy -= 2;                                                                          //���������̕����ɂ͐i��
                            //i--;�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@                        //��������Ȃ������Ƃ����[���̐����L�[�v����
                        }
                    }
  //                  else
  //                  {
  //                      i--;�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@                          //�}�b�v�����̎��ɕ����̐���ς����Ƀ��[�v�ɖ߂�
 //                   }
                    break;
                case pointright:�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@�@                          //�ȉ��قړ���
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
        if(_trueroom < _roommin)//�������[�v�����o�����̂ŗ�O�����@���Ȃ��ŁI�I�I
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
        g_field[g_nowpositiony, g_nowpositionx] = 1000 * g_nowfloor;//�v���C���[����������鏉���ʒu�̃i���o�[�����
    }

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
        NewPlayer = Instantiate(Player, new Vector2(this.rectTransform.anchoredPosition.x + (g_nowpositionx * 20), this.rectTransform.anchoredPosition.y + (g_nowpositiony * 20)), Quaternion.identity, this.transform);//�v���C���[�v���n�u���~�j�}�b�v��Ő�������
    }

    public void RoomMet(int Dir)//�V���������ɐi�ޏ���
    {
        switch (Dir)//�㉺���E�̕����ɂ���ĕ���
        {
            case pointup:
                if (g_field[g_nowpositiony + 1, g_nowpositionx] >= 1)//������ɓ����������ꍇ
                {
                    g_nowpositiony += 2;                         //�v���C���[�̈ʒu����̕����ɂ��炷
                    g_playerdir = pointup;                       //�v���C���[�̐i�s��������ɐݒ�
                    uproadsc.UpSceneload();                    //��̓��ɐݒ肳��Ă���X�N���v�g�̃V�[���ǂݍ��݊֐���I��
                }
                break;
            case pointright:                                   //�ȉ��قړ���
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
        Destroy(NewPlayer);//�v���C���[�̃v���n�u����
        NewPlayer = Instantiate(Player, new Vector2(this.rectTransform.anchoredPosition.x + (g_nowpositionx * 20), this.rectTransform.anchoredPosition.y + (g_nowpositiony * 20)), Quaternion.identity, this.transform);//�V�����ړ���̃v���C���[�𐶐�����
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
        if (g_pointx == g_nowpositionx && g_pointy == g_nowpositiony)//�����̍ŏI�|�C���g�ƍ��̃v���C��[�̈ʒu�������ꍇ
        {
            //Instantiate(stairs, new Vector3(10,10,0), Quaternion.identity, this.transform);//���̊K�w�ɍs���K�i�𐶐�����
        }
    }
    #endregion
}
