using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _OrientationTransform;

    [Header("Movement Speed")]
    [SerializeField] private KeyCode _walkKey;
    [SerializeField] private float _movementSpeed;

    [Header("sprint settings")]
    [SerializeField] private KeyCode _sprintKey;
    [SerializeField] private float _sprintMultiplayer;
    [SerializeField] private float _sprintDrag;

    [Header("jump settings")]
    [SerializeField] private KeyCode _jumpKey;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpCooldown;
    [SerializeField] private bool _canJump;
    [SerializeField] private float _PlayerHeight;

    [Header("Ground Check Settings")]
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundDrag;
    private Rigidbody _PlayerRigidbody;

    private float _horizontalInput, _verticalInput;

    private Vector3 _movementDirection;

    private bool _isSprinting;

    private void Awake()
    {
        _PlayerRigidbody = GetComponent<Rigidbody>();
        _PlayerRigidbody.freezeRotation = true;
    }

    private void Update()
    {
        SetInputs();
        SetPlayerDrag();
        LimitPlayerSpeed();
    }

    private void FixedUpdate()
    {
        SetPlayerMovement();
    }
    private void LimitPlayerSpeed()
    {
    // 1. O anki (sprint veya normal) maksimum hızı belirle
    float currentMaxSpeed;
        if (_isSprinting)
        {
            currentMaxSpeed = _movementSpeed * _sprintMultiplayer;
        }
        else
        {
            currentMaxSpeed = _movementSpeed;
        }

    // 2. Y eksenini (düşüş/zıplama) hesaba katmadan mevcut yatay hızı al
    // (BURASI SENDE EKSİKTİ)
    Vector3 flatVelocity = new Vector3(_PlayerRigidbody.linearVelocity.x, 0f, _PlayerRigidbody.linearVelocity.z);

    // 3. Eğer yatay hız, izin verilen maksimum hızı aşıyorsa...
        if (flatVelocity.magnitude > currentMaxSpeed)
        {
            // 4. Hızı limitle
            Vector3 limitedVelocity = flatVelocity.normalized * currentMaxSpeed;
        
            // 5. Rigidbody'nin hızını, limitlenmiş yatay hız + mevcut dikey hız olarak ayarla
            // (DOĞRU ATAMA ŞEKLİ BUDUR)
            _PlayerRigidbody.linearVelocity = new Vector3(limitedVelocity.x, _PlayerRigidbody.linearVelocity.y, limitedVelocity.z);
        }
}
    private void SetInputs()
    {

        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(_sprintKey))
        {
            _isSprinting = true;
            Debug.Log("player Sprinting!!!");
        }
        else if (Input.GetKeyDown(_walkKey))
        {
            _isSprinting = false;
            Debug.Log("player Walking!!!");
        }
        else if (Input.GetKey(_jumpKey) && _canJump && IsGrounded())
        {
            _canJump = false;
            SetPlayerJumping();
            Invoke(nameof(ResetJumping), _jumpCooldown);
        }

    }
    private void SetPlayerMovement()
    {
        _movementDirection = _OrientationTransform.forward * _verticalInput
        + _OrientationTransform.right * _horizontalInput;
        if (_isSprinting)
        {
            _PlayerRigidbody.AddForce(_movementDirection.normalized * _movementSpeed * _sprintMultiplayer, ForceMode.Force);
        }
        else
        {
            _PlayerRigidbody.AddForce(_movementDirection.normalized * _movementSpeed, ForceMode.Force);
        }
    }
    private void SetPlayerDrag()
    {
        if (_isSprinting)
        {
            _PlayerRigidbody.linearDamping = _sprintDrag;
        }
        else
        {
            _PlayerRigidbody.linearDamping = _groundDrag;
        }
    }
    private void SetPlayerJumping()
    {
        _PlayerRigidbody.linearVelocity = new Vector3(_PlayerRigidbody.linearVelocity.x, 0f, _PlayerRigidbody.linearVelocity.z);
        _PlayerRigidbody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }
    private void ResetJumping()
    {
        _canJump = true;
    }
    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _PlayerHeight * 0.5f * 0.2f, _groundLayer);
    }
}