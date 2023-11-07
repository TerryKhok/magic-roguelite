using UnityEngine;

public class Droptest : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    private DropItemEnemy drop;
    private Renderer rend;
    bool deth = false;

    // Start is called before the first frame update
    void Start()
    {
        drop = enemy.GetComponent<DropItemEnemy>();
        rend = enemy.GetComponent<Renderer>();
    }

   public void Action()
    {
        if(!deth)
        {
            Debug.Log(name + "‚ð“|‚µ‚½");
           rend.material.color = Color.black;
            drop.ItemDrop();
        }
        else
        {
            rend.material.color = Color.red;
        }

        deth = !deth;
    }
}
