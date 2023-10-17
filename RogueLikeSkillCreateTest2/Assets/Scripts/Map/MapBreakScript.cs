using System.Collections;
using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapBreakScript : MonoBehaviour
{
    private int oldfloor;
    void Start()
    {
        //SceneManager.activeSceneChanged += OnActiveSceneChanged;
        SceneManager.sceneLoaded += OnSceneLoaded;
        oldfloor = MapGeneraterScript.Instance.g_nowfloor;
    }
    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, LoadSceneMode mode)
    {
        if (oldfloor < MapGeneraterScript.Instance.g_nowfloor)
        {
             Destroy(this.gameObject);
        }
    }
}
