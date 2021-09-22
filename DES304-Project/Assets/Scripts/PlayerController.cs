using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [Header("-Movement-")]
    public float _moveSpeed;

    [Header("-Utility-")]
    public Rigidbody2D _rigidBody;
    public Camera _camera;

    [Header("-Dash Controls-")]
    public float _dashSpeed;
    [SerializeField] float _dashCooldown = 50;
    private bool _canDash = true;

    Vector2 _playerMovement;
    Vector2 _mousePosition;

    void Update()
    {
        _playerMovement.x = Input.GetAxisRaw("Horizontal");
        _playerMovement.y = Input.GetAxisRaw("Vertical");

        _playerMovement = _playerMovement.normalized;

        _mousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        _rigidBody.MovePosition(_rigidBody.position + _playerMovement * _moveSpeed * Time.fixedDeltaTime);

        Vector2 _lookDirection = _mousePosition - _rigidBody.position;
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

        if (Input.GetKeyDown(KeyCode.Space) && _canDash)
        {
            _rigidBody.AddForce(_lookDirection * _dashSpeed * Time.fixedDeltaTime);
            _canDash = false;
            _dashCooldown = 50;
        }

    }

}
