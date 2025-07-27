//black hole script
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [Header("����")]
    public float radius = 3.5f;      // �����뾶
    public float pullForce = 80f;  // ������
    public float duration = 2.0f;  // ����ʱ��
    public float destroyRadius = 0.3f; // ����˰뾶�����ݻ�

    // ����Ϸ��ʼʱ����һ��
    void Start()
    {
        // ����һ��������Ч����ѡ��
        // FindObjectOfType<AudioManager>().PlaySFX(YourSpawnSoundClip);

        // �� duration ָ�����������Զ���������ڶ�����
        Destroy(gameObject, duration);
    }

    // FixedUpdate ���ڴ���������صļ��㣬��Update���ȶ�
    void FixedUpdate()
    {
        // �ҳ��������뾶�ڵ�������ײ��
        Collider2D[] collidersInRadius = Physics2D.OverlapCircleAll(transform.position, radius);

        // �����ҵ���ÿһ����ײ��
        foreach (var collider in collidersInRadius)
        {
            // ����ֻ�ԡ�Fruit����ǩ���������Ȥ
            if (collider.CompareTag("Fruit"))
            {
                // ��ȡˮ���ĸ������
                Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // �����ˮ��ָ��ڶ����ĵķ���
                    Vector2 direction = (Vector2)transform.position - rb.position;

                    // ���ˮ�������ķǳ�������ֱ��������
                    if (direction.magnitude < destroyRadius)
                    {
                        // ��������Բ���һ��������Ч
                        // EffectManager.Instance.PlayDestroyEffect(collider.transform.position);
                        Destroy(collider.gameObject);
                    }
                    else // ���������Χ���Ͷ���ʩ����
                    {
                        // ʹ�� AddForce ��ˮ���������ġ�ForceMode.Impulse Ч����ͻȻ
                        rb.AddForce(direction.normalized * pullForce * Time.deltaTime, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    // (��ѡ) �ڱ༭���л��Ƴ���Χ���������
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radius); // ����������Χ
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, destroyRadius); // ���ƴݻٷ�Χ
    }
}