using UnityEngine;
using TMPro;
using System;

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
    [SerializeField] private float _raycastLength;
    [SerializeField] private LayerMask _enemyLayer;

    [Header("-Objects-")]
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private Camera _camera;

    Vector2 _playerMovement;
    Vector2 _MousePosition;

    private void Start()
    {
        UpdateHealth();
    }

    void Update()
    {
        _playerMovement.x = Input.GetAxisRaw("Horizontal");
        _playerMovement.y = Input.GetAxisRaw("Vertical");

        //_playerMovement = _playerMovement.normalized;

        _MousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        if (_canDash && Input.GetKeyDown(KeyCode.Space))
        {
            //_isDashing = true;
            //Debug.Log(_isDashing);
            Dash();
        }
    }

    private void FixedUpdate()
    {
        //_rigidBody.MovePosition(_rigidBody.position + _playerMovement * _moveSpeed * Time.fixedDeltaTime);
        _rigidBody.AddForce(_playerMovement.normalized * _moveSpeed * Time.deltaTime, ForceMode2D.Force);

        Vector2 _lookDirection = _MousePosition - _rigidBody.position;
        float angle = Mathf.Atan2(_lookDirection.y, _lookDirection.x) * Mathf.Rad2Deg - 90f;
        _rigidBody.rotation = angle;

        //if (_dashCooldown == 0)
        //{
        //    _canDash = true;
        //}
        //else
        //{
        //    _dashCooldown--;
        //}

        _rigidBody.velocity = Vector2.zero;

        //if (_canDash &&_isDashing)
        //{
        //    _rigidBody.AddForce(_lookDirection * _dashSpeed);
        //    _dashCooldown = 25;
        //    _canDash = false;
        //    _isDashing = false;
        //}

        //Vector2 _raycastDirection = transform.TransformDirection(Vector2.up) * _raycastLength;
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, _raycastLength, _enemyLayer);
        if (hitInfo.collider != null)
        {
            Debug.Log(hitInfo.collider.gameObject.tag);
            if (hitInfo.collider.gameObject.CompareTag("Enemy") && _isDashing)
            {
                Destroy(hitInfo.collider.gameObject);
            }
        }
    }

    private void Dash()
    {
        Vector2 _lookDirection = _MousePosition - _rigidBody.position;
        _rigidBody.AddForce(_lookDirection * _dashSpeed);

        _isDashing = true;
        _canDash = false;

        Invoke(nameof(StopDashing), _dashTime);
        Invoke(nameof(DashCooldown), _dashCooldown);

    }

    private void StopDashing()
    {
        _isDashing = false;
    }

    private void DashCooldown()
    {
        _canDash = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta; 
        Vector2 _raycastDirection = transform.TransformDirection(Vector2.up) * _raycastLength;
        Gizmos.DrawRay(transform.position, _raycastDirection);
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
