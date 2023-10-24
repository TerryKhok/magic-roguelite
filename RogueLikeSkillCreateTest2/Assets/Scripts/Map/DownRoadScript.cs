using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DownRoadScript : MonoBehaviour
{
    private SpriteRenderer spriterenderer;
    private BoxCollider2D boxcollider2d;

    private int _roomnumberint;//�����ԍ�
    private string _roomnumberstr;//�����ԍ��i������j
    void Awake()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
        boxcollider2d = GetComponent<BoxCollider2D>();
    }

    public void DownRoadGenerate(int downroad)//���𐶐����邩���Ȃ���
    {
        if (downroad != 1)
        {
            spriterenderer.enabled = true;
            boxcollider2d.isTrigger = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _roomnumberint = MapGeneraterScript.Instance.g_field[MapGeneraterScript.Instance.g_nowpositiony - 2, MapGeneraterScript.Instance.g_nowpositionx];//�����ԍ��l��
            _roomnumberstr = _roomnumberint.ToString();//������ɕύX
            MapGeneraterScript.Instance.RoomMet(3);//�}�b�v�W�F�l���[�g�Ɉ�����Ԃ�
        }
    }
    public void DownSceneload()
    {
        SceneManager.LoadScene("Stage" + _roomnumberstr);//�V�[�������[�h����
    }
}
