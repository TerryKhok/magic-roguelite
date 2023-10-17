using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UpRoadScript : MonoBehaviour
{
    private SpriteRenderer spriterenderer;
    private BoxCollider2D boxcollider2d;

    private int _roomnumberint;
    private string _roomnumberstr;
        void Awake()
    {
        spriterenderer = GetComponent<SpriteRenderer>();   
        boxcollider2d = GetComponent<BoxCollider2D>();
    }

    public void UpRoadGenerate(int uproad)
    {
        if (uproad != 1)
        {
            spriterenderer.enabled = true;
            boxcollider2d.isTrigger = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        _roomnumberint = MapGeneraterScript.Instance.g_field[MapGeneraterScript.Instance.g_nowpositiony + 2, MapGeneraterScript.Instance.g_nowpositionx];
        _roomnumberstr = _roomnumberint.ToString();
        MapGeneraterScript.Instance.RoomMet(1);
    }
    public void UpSceneload()
    {
        SceneManager.LoadScene("Stage" + _roomnumberstr);
    }
}
