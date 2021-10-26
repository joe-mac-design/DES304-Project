﻿using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("-Movement-")]
    [SerializeField] private float _moveSpeed;

    [Header("-Dash Controls-")]
    [SerializeField] private float _dashSpeed = 1500f; // dash speed
    [SerializeField] float _dashTime = 0.1f; // How long the dash is active for
    [SerializeField] float _dashCooldown = 1f; // time between dashes
    public bool _canDash { get; private set; } = true;
    public bool _isDashing { get; private set; } = false;

    [Header("Health")]
    public float _playerHealth;
    [SerializeField] private TMP_Text _healthText;

    [Header("Raycasts")]
    [SerializeField] private LayerMask _enemyLayer;

    [Header("-Objects-")]
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private Camera _camera;

    Vector2 _playerMovement;
    Vector2 _MousePosition;
    Vector2 _lookDirection;
    Vector2 _lastPosition;

    private void Start()
    {
        UpdateHealth();
    }

    void Update()
    {
        _playerMovement.x = Input.GetAxisRaw("Horizontal");
        _playerMovement.y = Input.GetAxisRaw("Vertical");

        _MousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        if (_canDash && Input.GetMouseButtonDown(1))
        {
            _lastPosition = transform.position;
            Dash();
        }
    }

    private void FixedUpdate()
    {
        _rigidBody.AddForce(_playerMovement.normalized * _moveSpeed * Time.deltaTime, ForceMode2D.Force);

        _lookDirection = _MousePosition - _rigidBody.position;
        float angle = Mathf.Atan2(_lookDirection.y, _lookDirection.x) * Mathf.Rad2Deg - 90f;
        _rigidBody.rotation = angle;

        _rigidBody.velocity = Vector2.zero;
    }

    private void Dash()
    {
        _rigidBody.AddForce(_lookDirection * _dashSpeed);

        _isDashing = true;
        _canDash = false;

        Invoke(nameof(StopDashing), _dashTime);
        Invoke(nameof(DashCooldown), _dashCooldown);
        StartCoroutine(PostDashCheck());
    }

    private void StopDashing()
    {
        _isDashing = false;
    }

    private void DashCooldown()
    {
        _canDash = true;
    }

    private IEnumerator PostDashCheck()
    {
        yield return new WaitForSeconds(_dashTime);

        RaycastHit2D hitInfo = Physics2D.Linecast(transform.position, _lastPosition, _enemyLayer);
        if (hitInfo.collider != null)
        {
            Debug.Log(hitInfo.collider.gameObject.tag);
            if (hitInfo.collider.gameObject.CompareTag("Enemy") && _rigidBody.velocity.magnitude < 10)
            {
                Destroy(hitInfo.collider.gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(transform.position, _lastPosition);
    }

    // Health & Damage
    public void TakeDamage(int projectileDamage)
    {
        _playerHealth -= projectileDamage;
        HealthCheck();
    }

    private void HealthCheck()
    {
        if (_playerHealth <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void UpdateHealth()
    {
        _healthText.text = _playerHealth.ToString("0");
    }
}
