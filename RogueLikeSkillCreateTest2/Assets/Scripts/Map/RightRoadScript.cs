using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RightRoadScript : MonoBehaviour
{
    private SpriteRenderer spriterenderer;
    private BoxCollider2D boxcollider2d;

    private int roomnumberint;
    private string roomnumberstr;
    void Awake()
    {
        spriterenderer = GetComponent<SpriteRenderer>();
        boxcollider2d = GetComponent<BoxCollider2D>();
    }

    public void RightRoadGenerate(int rightroad)
    {
        if (rightroad != 2)
        {
            spriterenderer.enabled = true;
            boxcollider2d.isTrigger = false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            roomnumberint = MapGeneraterScript.Instance.g_field[MapGeneraterScript.Instance.g_nowpositiony, MapGeneraterScript.Instance.g_nowpositionx + 2];
            roomnumberstr = roomnumberint.ToString();
            MapGeneraterScript.Instance.RoomMet(2);
        }
    }
    public void RightSceneload()
    {
        SceneManager.LoadScene("Stage" + roomnumberstr);
    }
}
