using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneratorScript : MonoBehaviour
{
    [SerializeField] GameObject[] MapCell;  //�}�b�v�Z���̃v���n�u�ꗗ
    [SerializeField] Vector2 MapCellSize;   //�}�b�v�Z���̃T�C�Y
    [SerializeField] Vector2 FieldSize;     //�}�b�v�Z����~���l�߂鐔
    // Start is called before the first frame update
    void Start()
    {
        int r;  //�}�b�v�Z���I��p�����̒�`�i���Ő����j
        Transform tf = GetComponent<Transform>();    //�I�u�W�F�N�g�̍��W�i���̌ア���ς��g��
        gameObject.transform.position = Vector2.zero;   //�}�b�v�W�F�l���[�^�̈ʒu��(0,0,0)��
        for (int i = 0; i < FieldSize.y; i++) {  //Y�����̃��[�v
            for (int j = 0; j < FieldSize.x; j++)//X�����̃��[�v
            {
                r = Random.Range(0, MapCell.Length);    //�}�b�v�Z���I��p�̗�������
                Instantiate(MapCell[r], GetComponent<Transform>(), true);                    //�v���n�u���X�g���烉���_���Ȃ��̂����̏ꏊ�ɐ���
                tf.position += new Vector3(MapCellSize.x, 0, 0);                      //�}�b�v�Z���̃T�C�Y�ɑΉ����������ړ�
            }
            tf.position += new Vector3(MapCellSize.x * FieldSize.x * -1, MapCellSize.y, 0);                         //�}�b�v�Z���̃T�C�Y�ɑΉ��������c�ړ�
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
