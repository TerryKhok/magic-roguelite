using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyPlayerScript : MonoBehaviour
{
    public static DontDestroyPlayerScript InstancePlayer;

    // Start is called before the first frame update
    public void Awake()
    {
        // �V���O���g���̎���
        if (InstancePlayer == null)
        {
            // ���g���C���X�^���X�Ƃ���
            InstancePlayer = this;
            DontDestroyOnLoad(InstancePlayer);//���̃I�u�W�F�N�g���󂳂Ȃ�
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
