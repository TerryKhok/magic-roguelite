using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeftRoadScript : MonoBehaviour
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

    public void LeftRoadGenerate(int leftroad)
    {
        if (leftroad != 2)
        {
            spriterenderer.enabled = true;
            boxcollider2d.isTrigger = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            _roomnumberint = MapGeneraterScript.Instance.g_field[MapGeneraterScript.Instance.g_nowpositiony, MapGeneraterScript.Instance.g_nowpositionx - 2];
            _roomnumberstr = _roomnumberint.ToString();
            MapGeneraterScript.Instance.RoomMet(4);
        }
    }
    public void LeftSceneload()
    {
        SceneManager.LoadScene("Stage" + _roomnumberstr);
    }
}
