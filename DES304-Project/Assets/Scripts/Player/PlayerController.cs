﻿using UnityEngine;
using TMPro;
using System;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("-Movement-")]
    [SerializeField] private float _moveSpeed;

    [Header("-Dash Controls-")]
    //[SerializeField] private float _dashSpeed = 1500f; // dash speed
    [SerializeField] private float _maxDistance = 1f;
    [SerializeField] float _dashTime = 0.1f; // How long the dash is active for
    [SerializeField] float _dashCooldown = 1f; // time between dashes
    [SerializeField] int _maxDashes = 3;
    //public float _megaDashSpeed;
    public bool _canDash { get; private set; } = true;
    public bool _isDashing { get; private set; } = false;

    [Header("Health")]
    public float _playerHealth;
    [SerializeField] private TMP_Text _healthText;

    [Header("Raycasts")]
    [SerializeField] private LayerMask _enemyLayer;
    [SerializeField] private LayerMask _wallLayer;

    [Header("-Objects-")]
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private Camera _camera;
    public GameObject _retryPanel;

    Vector2 _playerMovement;
    Vector2 _MousePosition;
    Vector2 _lookDirection;
    Vector2 _lastPosition;

    Queue _dashQueue = new Queue();

    private void Start()
    {
        UpdateHealth();
    }

    void Update()
    {
        _playerMovement.x = Input.GetAxisRaw("Horizontal");
        _playerMovement.y = Input.GetAxisRaw("Vertical");

        _MousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        if (_canDash && Input.GetKeyDown(KeyCode.Space))
        {
            _lastPosition = transform.position;
            Dash();
        }

        if (Input.GetMouseButtonDown(0) && _dashQueue.Count < _maxDashes)
        {
            ClickPosition();
        }

        if (Input.GetButtonUp("Fire2"))
        {
            if (_dashQueue.Count == 0) Dash();
            else
            {
                StartCoroutine(ClickPositionDash());
            }
        }
    }

    private void FixedUpdate()
    {
        _rigidBody.AddForce(_playerMovement.normalized * _moveSpeed * Time.deltaTime, ForceMode2D.Force);

        _lookDirection = _MousePosition - _rigidBody.position;
        if (_lookDirection.magnitude > 0.1f)
        {
            float angle = Mathf.Atan2(_lookDirection.y, _lookDirection.x) * Mathf.Rad2Deg - 90f;
            _rigidBody.rotation = angle;
        }
        

        _rigidBody.velocity = Vector2.zero;
    }

    private void ClickPosition()
    {
        _dashQueue.Enqueue(_MousePosition);
        Vector3 _otherPosition = transform.position;
        foreach (Vector2 _dashClick in _dashQueue)
        {
            Debug.DrawLine(_dashClick, _otherPosition, Color.white, 5f);
            _otherPosition = _dashClick;
            Debug.Log(_dashClick);
        }
    }

    private IEnumerator ClickPositionDash()
    {
        _dashQueue.Peek();

        float _distance = Vector2.Distance(transform.position, (Vector2)_dashQueue.Peek());

        _lookDirection = (Vector2)_dashQueue.Peek() - _rigidBody.position;
        float angle = Mathf.Atan2(_lookDirection.y, _lookDirection.x) * Mathf.Rad2Deg - 90f;
        _rigidBody.rotation = angle;

        _lastPosition = transform.position;

        //_rigidBody.AddForce(_lookDirection * (_distance * _megaDashSpeed), ForceMode2D.Force); // replace this 
        _rigidBody.MovePosition((Vector2)_dashQueue.Peek()); // moves to exact line position
        StartCoroutine(PostDashCheck());
        _dashQueue.Dequeue();

        yield return new WaitForSeconds(0.5f);

        if (_dashQueue.Count != 0)
        {
            StartCoroutine(ClickPositionDash());
        }
    }

    private void Dash()
    {
        Vector2 myDash = _MousePosition - (Vector2)transform.position; // Takes vector of the dash

        if (myDash.magnitude > _maxDistance) // if the dash is too long, make it shorter
        {
            myDash = (Vector2)transform.position + myDash.normalized * _maxDistance;
        }

        RaycastHit2D hitInfo = Physics2D.Linecast(transform.position, (Vector2)transform.position + myDash, _wallLayer);
        if (hitInfo.collider != null) // if the dash hits a wall, stop it right before wall
        {
            myDash = hitInfo.point - (Vector2)transform.position;
            Debug.Log(hitInfo.point);
            Debug.Log(hitInfo.rigidbody.name);
        }

        // Move player in dash direction
        gameObject.transform.position = Vector2.MoveTowards(transform.position, (Vector2)transform.position + myDash, 200f);

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

        RaycastHit2D hitInfo = Physics2D.Linecast(_lastPosition, transform.position, _enemyLayer);
        if (hitInfo.collider != null)
        {
            Debug.Log(hitInfo.collider.gameObject.tag);
            if (hitInfo.collider.gameObject.CompareTag("Enemy") && _rigidBody.velocity.magnitude < 10)
            {
                Destroy(hitInfo.collider.gameObject);
                StartCoroutine(PostDashCheck());
            }
        }
    }

    // Gizmos
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
            _retryPanel.SetActive(true);
        }
    }

    public void UpdateHealth()
    {
        _healthText.text = _playerHealth.ToString("0");
    }
}
