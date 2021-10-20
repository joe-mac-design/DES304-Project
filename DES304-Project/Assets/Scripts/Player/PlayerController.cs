using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("-Movement-")]
    [SerializeField] private float _moveSpeed;

    [Header("-Dash Controls-")]
    [SerializeField] private float _dashSpeed;
    [SerializeField] float _dashCooldown = 50;
    [SerializeField] private bool _canDash = true;
    [SerializeField] private bool _isDashing = false;

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

        _playerMovement = _playerMovement.normalized;

        _MousePosition = _camera.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.Space) && _canDash)
        {
            _isDashing = true;
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

        if (_isDashing && _canDash)
        {
            _rigidBody.AddForce(_lookDirection * _dashSpeed * Time.fixedDeltaTime);
            _canDash = false;
            _dashCooldown = 50;
            _isDashing = false;    
        }

        //Vector2 _raycastDirection = transform.TransformDirection(Vector2.up) * _raycastLength;
        RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, transform.up, _raycastLength, _enemyLayer);
        if (hitInfo.collider != null)
        {
            Debug.Log(hitInfo.collider.gameObject.tag);
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green; 
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
