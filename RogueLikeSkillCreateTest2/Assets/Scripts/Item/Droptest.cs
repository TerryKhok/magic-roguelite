using UnityEngine;

public class Droptest : MonoBehaviour
{
    [SerializeField] GameObject _enemy;
    private DropItemEnemy _drop;
    private Renderer _rend;
    bool _deth = false;
    string _enemyName;

    // Start is called before the first frame update
    void Start()
    {
        _drop = _enemy.GetComponent<DropItemEnemy>();
        _rend = _enemy.GetComponent<Renderer>();
        _enemyName = _drop.getEnemydata().getEnemyName();
    }

   public void Action()
    {
        if(!_deth)
        {
            Debug.Log(_enemyName + "‚ð“|‚µ‚½");
           _rend.material.color = Color.black;
            _drop.ItemDrop();
        }
        else
        {
            _rend.material.color = Color.red;
        }

        _deth = !_deth;
    }
}
