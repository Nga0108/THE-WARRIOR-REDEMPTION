using UnityEngine;
using UnityEngine.InputSystem;

public class TopDownMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator anim;
    private Vector2 moveInput;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        if (rb != null)
        {
            rb.gravityScale = 0;
            rb.freezeRotation = true;
        }
    }

    void Update()
    {
        Vector2 input = Vector2.zero;
        var keyboard = Keyboard.current;

        if (keyboard != null)
        {
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed) input.y = 1;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed) input.y = -1;
            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed) input.x = -1;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed) input.x = 1;
        }

        moveInput = input.normalized;

        // Xử lý lật mặt (Flip)
        if (moveInput.x > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput.x < 0) transform.localScale = new Vector3(-1, 1, 1);

        // Cập nhật Animator
        if (anim != null)
        {
            // magnitude trả về 1 nếu di chuyển, 0 nếu đứng yên
            anim.SetFloat("InputX", moveInput.magnitude);
        }
    }

    // CHỈ GIỮ LẠI MỘT HÀM FIXEDUPDATE DUY NHẤT NÀY
    void FixedUpdate()
    {
        if (rb != null)
        {
            // Trong Unity 6, dùng linearVelocity thay cho velocity
            rb.linearVelocity = moveInput * moveSpeed;
        }
    }
}