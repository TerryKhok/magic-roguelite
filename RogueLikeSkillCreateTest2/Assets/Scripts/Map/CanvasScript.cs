using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//ミニマップの保持
public class CanvasScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}
