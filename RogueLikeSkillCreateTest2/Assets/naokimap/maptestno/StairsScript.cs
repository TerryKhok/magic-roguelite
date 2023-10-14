using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StairsScript : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string triggerstr = (MapGeneraterScript.Instance.g_nowfloor * 1000 + 1000).ToString();
        SceneManager.LoadScene("Stage" + triggerstr);
        MapGeneraterScript.Instance.Init();
        MapGeneraterScript.Instance.Draw();
        //MapGeneraterScript.Instance.NewRoom();
    }
}
