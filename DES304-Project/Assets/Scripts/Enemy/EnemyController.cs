using UnityEngine;

public class EnemyController : MonoBehaviour
{

    /*
    EnemyController script provided by Blackthornprod on YouTube: https://youtu.be/_Z1t7MNk0c4 
    Adjusted to fit my game type.
    */

    [Header("-Movement-")]
    public float _enemySpeed; 
    public float _stoppingDistance;
    public float _retreatDistance;

    [Header("-Shooting-")]
    public float _startTimeBetweenShots;
    private float _timeBetweenShots;

    [Header("-Objects-")]
    public GameObject _bullet;
    public Transform _player;

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        _timeBetweenShots = _startTimeBetweenShots;
    }

    void Update()
    {

        if(_player == null)
        {
            return;
        }

        // Enemy following player
        if (Vector2.Distance(transform.position, _player.position) > _stoppingDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, _player.position, _enemySpeed * Time.deltaTime);
        }
        else if (Vector2.Distance(transform.position, _player.position) < _stoppingDistance && Vector2.Distance(transform.position, _player.position) > _retreatDistance)
        {
            transform.position = this.transform.position;
        }
        else if (Vector2.Distance(transform.position, _player.position) < _retreatDistance)
        {
            transform.position = Vector2.MoveTowards(transform.position, _player.position, -_enemySpeed * Time.deltaTime);
        }

        // Spawning Bullet
        if (_timeBetweenShots <= 0)
        {
            Instantiate(_bullet, transform.position, Quaternion.identity);
            _timeBetweenShots = _startTimeBetweenShots;
        }
        else
        {
            _timeBetweenShots -= Time.deltaTime;
        } 
    }
}
