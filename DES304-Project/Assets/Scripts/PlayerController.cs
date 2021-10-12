using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("-Movement-")]
    public float _moveSpeed;

    [Header("-Dash Controls-")]
    public float _dashSpeed;
    [SerializeField] float _dashCooldown = 50;
    private bool _canDash = true;
    private bool _doDash = false;

    [Header("-Objects-")]
    public Rigidbody2D _rigidBody;
    public Camera _camera;

    Vector2 _playerMovement;
    Vector2 _MousePosition;

    void Update()
    {
        _playerMovement.x = Input.GetAxisRaw("Horizontal");
        _playerMovement.y = Input.GetAxisRaw("Vertical");

        _playerMovement = _playerMovement.normalized;

        _MousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        if(Input.GetKeyDown(KeyCode.Space) && _canDash)
        {
            _doDash = true;
        }

    }

    private void FixedUpdate()
    {
        _rigidBody.MovePosition(_rigidBody.position + _playerMovement * _moveSpeed * Time.fixedDeltaTime);

        Vector2 _lookDirection = _MousePosition - _rigidBody.position;
        float angle = Mathf.Atan2(_lookDirection.y, _lookDirection.x) * Mathf.Rad2Deg - 90f;
        _rigidBody.rotation = angle;

        if (_dashCooldown == 0)
        {
            _canDash = true;
        } else
        {
            _dashCooldown--;
        }

        _rigidBody.velocity = Vector2.zero;

        if (_doDash && _canDash)
        {
            _rigidBody.AddForce(_lookDirection * _dashSpeed * Time.fixedDeltaTime);
            _canDash = false;
            _dashCooldown = 50;
            _doDash = false;    
        }

    }
}
