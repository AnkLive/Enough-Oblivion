using UnityEngine;
using System.Collections;
public class Player : MonoBehaviour
{
    // Параметры движения
    public float moveSpeed = 10f; 

    // Параметры прыжка
    public float jumpForce = 12f; 
    public float maxJumpHeight = 4f; 
    public bool canDoubleJump = true;
    private bool doubleJumped = false;

    // Параметры Dash
    public float dashForce = 15f; 
    public float dashCooldown = 2f;
    public float dashDuration = 0.2f; 

    private bool isDashing = false;
    private float dashCooldownTimer = 0f;

    // Проверка нахождения на земле
    private bool isGrounded = false;
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    // Rigidbody компонент для управления физикой персонажа
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Проверка нахождения на земле
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Движение влево и вправо
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Прыжок (используем пробел)
        if (isGrounded)
        {
            doubleJumped = false;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            }
            else if (canDoubleJump && !doubleJumped)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                doubleJumped = true;
            }
        }

        // Dash (используем левый Shift)
        if (Input.GetKeyDown(KeyCode.LeftShift) && !isDashing && dashCooldownTimer <= 0)
        {
            isDashing = true;
            StartCoroutine(Dash());
        }

        // Обновление таймера для перезарядки Dash
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        // Ограничение скорости в горизонтальном направлении
        rb.velocity = new Vector2(Mathf.Clamp(rb.velocity.x, -moveSpeed, moveSpeed), rb.velocity.y);
    }

    // Coroutine для Dash
    IEnumerator Dash()
    {
        float dashTime = 0;
        while (dashTime < dashDuration)
        {
            rb.velocity = new Vector2(transform.localScale.x * dashForce, rb.velocity.y);
            dashTime += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
        dashCooldownTimer = dashCooldown;
    }
}