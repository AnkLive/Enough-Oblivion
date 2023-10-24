using UnityEngine;
using System.Collections;
using NaughtyAttributes;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerUnitMovementComponent : MonoCache, IInitialize<PlayerUnitMovementComponent>, IActivate<PlayerUnitMovementComponent>
{
    #region Field

    private Controller _controller;
    // ������ �� ��������� Rigidbody2D �������.
    private Rigidbody2D _objectRigidbody;
    
    // ���������� ��� �������� ����� ��������.
    private float _moveInput;

    // ���������� ��� ���������� ������� ������� � �������� ���������� ����.
    private bool _doubleJumped;
    private bool _canDash = true;
    private bool _isDashing;
    
    // ���������� ��� �����������, ��������� �� �������� �� �����.
    private bool _isGrounded;
    
    #endregion
    
    #region Inspector Field
    
    [Header("��������� ��������")]
    // ����� �� �������� ���������.
    [SerializeField] private bool canMove;
    
    // �������� �������� ���������.
    [SerializeField, Range(0, 100), ShowIf(nameof(canMove))] 
    private float moveSpeed;
    
    [Header("��������� ������")]
    // ����� �� �������� �������.
    [SerializeField] private bool canJump;
    
    // ���� ������.
    [SerializeField, Range(0, 100), ShowIf(nameof(canJump))] 
    private float jumpForce;
    
    // ������������ ������ ������.
    [SerializeField, Range(0, 100), ShowIf(nameof(canJump))] 
    private float maxJumpHeight;
    
    // ����� �� �������� ��������� ������� ������.
    [SerializeField, ShowIf(nameof(canJump))] 
    private bool canDoubleJump;
    
    [Header("��������� ����")]
    // ����� �� �������� ��������� ���.
    [SerializeField] private bool canDashing;
    
    // ���� ����.
    [SerializeField, Range(0, 100), ShowIf(nameof(canDashing))] 
    private float dashForce;
    
    // ����� �������������� ���� ����� ����������.
    [SerializeField, Range(0, 100), ShowIf(nameof(canDashing))] 
    private float dashCooldown;
    
    // ������������ ����.
    [SerializeField, Range(0, 100), ShowIf(nameof(canDashing))] 
    private float dashDuration;
    
    [Header("��������� ����������� �����")]
    // ���������� ��� ��������, ��������� �� �������� �� �����.
    [SerializeField, Range(0, 100)] 
    private float groundCheckDistance;
    
    // ����, �������������� �����.
    [SerializeField] 
    private LayerMask groundMask;
    
    #endregion

    #region Methods

    #region Main

    public void Activate()
    {
        _controller.Main.Jump.performed += _ => PerformJump();
        _controller.Main.Dash.performed += _ => PerformDash();
        _controller.Enable();
        AddFixedUpdate();
    }

    public void Deactivate()
    {
        _controller.Disable();
        RemoveFixedUpdate();
    }
    
    public void Initialize()
    {
        _controller = new Controller();
        // �������� ������ �� ��������� Rigidbody2D ��� ������.
        _objectRigidbody = GetComponent<Rigidbody2D>();
        
    }
    
    public override void FixedRun()
    {
        // �������� ���� �������� �� ������.
        UpdateMovementInput();
        CheckGroundedStatus();
        
        if (_isDashing) return;
        
        if (canMove)
        {
            Move();
        }
    }

    #endregion

    #region Movement
    
    private void Move()
    {
        // ������������� �������� ��������� �� ������ ����� ��������.
        _objectRigidbody.velocity = new Vector2(_moveInput * moveSpeed, _objectRigidbody.velocity.y);
    }
    
    private void UpdateMovementInput()
    {
        _moveInput = _controller.Main.Move.ReadValue<float>();
    }

    #endregion

    #region Jump
    
    private void PerformJump()
    {
        if (_isGrounded)
        {
            _doubleJumped = false;
        }
        
        if (canJump)
        {
            if (_isGrounded)
            {
                // ��������� ������, ���� �������� ��������� �� �����.
                _objectRigidbody.velocity = new Vector2(_objectRigidbody.velocity.x, jumpForce);
                Debug.Log("��������");
            }
            else if (canDoubleJump && !_doubleJumped)
            {
                // ���� �������� ������� ������ � �� �� ��� �������� �����, ��������� ���.
                _objectRigidbody.velocity = new Vector2(_objectRigidbody.velocity.x, jumpForce);
                _doubleJumped = true;
                Debug.Log("������� ������ ��������");
            }
        }
    }

    #endregion

    #region Dash
    
    private void PerformDash()
    {
        if (canDashing)
        {
            if (_canDash)
            {
                // ���� ��� ��������, ��������� ���.
                StartCoroutine(Dash());
            }
        }
    }

    private IEnumerator Dash()
    {
        _canDash = false;
        _isDashing = true;
        
        // ��������� ���� ���� � ����������� �� ����������� ��������.
        _objectRigidbody.velocity = new Vector2(_moveInput * dashForce, 0);
        
        // ���� ��������� ����� ������������ ����.
        yield return new WaitForSeconds(dashDuration);
        _isDashing = false;
        // ���� ��������� ����� �������������� ���� ����� ����������.
        yield return new WaitForSeconds(dashCooldown);
        
        _canDash = true;
        Debug.Log("��� ��������");
    }
    
    #endregion

    #region Check ground
    
    private void CheckGroundedStatus()
    {
        // ���������, ��������� �� �������� �� �����, � ������� ���� ����.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundMask);
        if (hit.collider != null)
        {
            // ���� ��� ������� �����, ������� ��������� �� �����.
            _isGrounded = true;
            return;
        }
        _isGrounded = false;
    } 

    // ��������� ���������� ����� ��� ������������ ����������� �����.
    private void OnDrawGizmos()
    {
        var position = transform.position;
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Vector3 start = position;
        Vector3 end = position + Vector3.down * groundCheckDistance;
        Gizmos.DrawLine(start, end);
    }
    
    #endregion
    
    #endregion
}
