// PlayerController.cs
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // ����Inspector�е����ƶ��ٶ�
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // �����ǵ�ȫ���������ȡ�ƶ�����
        moveInput = GameInput.GetMovementAxis();
    }

    private void FixedUpdate()
    {
        // ��FixedUpdate��Ӧ�������ƶ���Ч����ƽ��
        rb.velocity = moveInput.normalized * moveSpeed;
    }
}