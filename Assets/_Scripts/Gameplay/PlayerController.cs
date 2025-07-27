// PlayerController.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // 可在Inspector中调整移动速度
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 从我们的全局输入类获取移动输入
        moveInput = GameInput.GetMovementAxis();
    }

    private void FixedUpdate()
    {
        // 在FixedUpdate中应用物理移动，效果更平滑
        rb.velocity = moveInput.normalized * moveSpeed;
    }
}