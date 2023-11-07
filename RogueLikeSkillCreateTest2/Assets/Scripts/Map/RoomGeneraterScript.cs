using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomGeneraterScript : MonoBehaviour
{
    public static RoomGeneraterScript Instance;

    const int EnemyNull = 0;//�G�̏�񖢓o�^
    const int EnemyDie = 1;//�G����ł�
    const int EnemyLive = 2;//�G�����Ă�

    bool StartFlag = false;

    GameObject stairs;//�K�i�̃v���n�u

    void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
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
        stairs = (GameObject)Resources.Load("MapResource/Stairs");//�K�i�̃v���n�u���擾
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
