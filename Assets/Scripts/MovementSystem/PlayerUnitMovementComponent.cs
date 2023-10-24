using UnityEngine;
using System.Collections;
using NaughtyAttributes;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerUnitMovementComponent : MonoCache, IInitialize<PlayerUnitMovementComponent>, IActivate<PlayerUnitMovementComponent>
{
    #region Field

    private Controller _controller;
    // Ссылка на компонент Rigidbody2D объекта.
    private Rigidbody2D _objectRigidbody;
    
    // Переменная для хранения ввода движения.
    private float _moveInput;

    // Переменные для управления двойным прыжком и моментом выполнения дэша.
    private bool _doubleJumped;
    private bool _canDash = true;
    private bool _isDashing;
    
    // Переменная для определения, находится ли персонаж на земле.
    private bool _isGrounded;
    
    #endregion
    
    #region Inspector Field
    
    [Header("Настройки движения")]
    // Может ли персонаж двигаться.
    [SerializeField] private bool canMove;
    
    // Скорость движения персонажа.
    [SerializeField, Range(0, 100), ShowIf(nameof(canMove))] 
    private float moveSpeed;
    
    [Header("Настройки прыжка")]
    // Может ли персонаж прыгать.
    [SerializeField] private bool canJump;
    
    // Сила прыжка.
    [SerializeField, Range(0, 100), ShowIf(nameof(canJump))] 
    private float jumpForce;
    
    // Максимальная высота прыжка.
    [SerializeField, Range(0, 100), ShowIf(nameof(canJump))] 
    private float maxJumpHeight;
    
    // Может ли персонаж выполнять двойной прыжок.
    [SerializeField, ShowIf(nameof(canJump))] 
    private bool canDoubleJump;
    
    [Header("Настройки дэша")]
    // Может ли персонаж выполнять дэш.
    [SerializeField] private bool canDashing;
    
    // Сила дэша.
    [SerializeField, Range(0, 100), ShowIf(nameof(canDashing))] 
    private float dashForce;
    
    // Время восстановления дэша после выполнения.
    [SerializeField, Range(0, 100), ShowIf(nameof(canDashing))] 
    private float dashCooldown;
    
    // Длительность дэша.
    [SerializeField, Range(0, 100), ShowIf(nameof(canDashing))] 
    private float dashDuration;
    
    [Header("Настройки определения земли")]
    // Расстояние для проверки, находится ли персонаж на земле.
    [SerializeField, Range(0, 100)] 
    private float groundCheckDistance;
    
    // Слой, представляющий землю.
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
        // Получаем ссылку на компонент Rigidbody2D при старте.
        _objectRigidbody = GetComponent<Rigidbody2D>();
        
    }
    
    public override void FixedRun()
    {
        // Получаем ввод движения от игрока.
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
        // Устанавливаем скорость персонажа на основе ввода движения.
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
                // Выполняем прыжок, если персонаж находится на земле.
                _objectRigidbody.velocity = new Vector2(_objectRigidbody.velocity.x, jumpForce);
                Debug.Log("Работает");
            }
            else if (canDoubleJump && !_doubleJumped)
            {
                // Если разрешен двойной прыжок и он не был выполнен ранее, выполняем его.
                _objectRigidbody.velocity = new Vector2(_objectRigidbody.velocity.x, jumpForce);
                _doubleJumped = true;
                Debug.Log("Двойной прыжок Работает");
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
                // Если дэш разрешен, выполняем его.
                StartCoroutine(Dash());
            }
        }
    }

    private IEnumerator Dash()
    {
        _canDash = false;
        _isDashing = true;
        
        // Применяем силу дэша в зависимости от направления движения.
        _objectRigidbody.velocity = new Vector2(_moveInput * dashForce, 0);
        
        // Ждем указанное время длительности дэша.
        yield return new WaitForSeconds(dashDuration);
        _isDashing = false;
        // Ждем указанное время восстановления дэша после выполнения.
        yield return new WaitForSeconds(dashCooldown);
        
        _canDash = true;
        Debug.Log("Дэш Работает");
    }
    
    #endregion

    #region Check ground
    
    private void CheckGroundedStatus()
    {
        // Проверяем, находится ли персонаж на земле, с помощью луча вниз.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundMask);
        if (hit.collider != null)
        {
            // Если луч пересек землю, считаем персонажа на земле.
            _isGrounded = true;
            return;
        }
        _isGrounded = false;
    } 

    // Отрисовка отладочных линий для визуализации определения земли.
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
