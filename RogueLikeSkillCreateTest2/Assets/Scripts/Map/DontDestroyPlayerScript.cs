using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyPlayerScript : MonoBehaviour
{
    public static DontDestroyPlayerScript InstancePlayer;

    // Start is called before the first frame update
    public void Awake()
    {
        // シングルトンの呪文
        if (InstancePlayer == null)
        {
            // 自身をインスタンスとする
            InstancePlayer = this;
            DontDestroyOnLoad(InstancePlayer);//このオブジェクトを壊さない
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}
