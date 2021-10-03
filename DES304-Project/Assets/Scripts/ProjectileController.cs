using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    public float _bulletSpeed;

    private Transform _player;
    private Vector2 _target;

    private void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player").transform;

        _target = new Vector2(_player.position.x, _player.position.y);
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, _target, _bulletSpeed * Time.deltaTime);

        if(transform.position.x == _target.x && transform.position.y == _target.y)
        {
            DestroyProjectile();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            DestroyProjectile();
        }
    }

    void DestroyProjectile()
    {
        Destroy(gameObject);
    }

}
