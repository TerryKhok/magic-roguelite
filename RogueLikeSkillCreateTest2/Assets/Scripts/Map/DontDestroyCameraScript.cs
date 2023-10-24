using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


//�~�j�}�b�v�̕ێ�
public class DontDestroyCameraScript : MonoBehaviour {

    // Start is called before the first frame update
    public void Awake()
    {
        if (MapGeneraterScript.Instance.g_playerdir == 0)
        {
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
