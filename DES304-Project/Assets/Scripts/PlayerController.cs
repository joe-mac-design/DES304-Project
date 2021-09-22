using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("-Movement-")]
    [SerializeField] private float _moveSpeed;

    [Header("-Utility-")]
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private Camera _camera;

    [Header("-Dash Controls-")]
    public float _dashSpeed;
    //private float _dashTime = 0.5f;
    //private float _dashCooldown = 1f;

    Vector2 _playerMovement;
    Vector2 _mousePosition;

    void Start()
    {

    }

    void Update()
    {
        _playerMovement.x = Input.GetAxisRaw("Horizontal");
        _playerMovement.y = Input.GetAxisRaw("Vertical");

        _playerMovement = _playerMovement.normalized;
        //_playerMovement = new Vector2(_playerMovement.x, _playerMovement.y).normalized;

        _mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        _rigidBody.MovePosition(_rigidBody.position + _playerMovement * _moveSpeed * Time.fixedDeltaTime);

        Vector2 _lookDirection = _mousePosition - _rigidBody.position;
        float angle = Mathf.Atan2(_lookDirection.y, _lookDirection.x) * Mathf.Rad2Deg - 90f;
        _rigidBody.rotation = angle;     
    }

}
