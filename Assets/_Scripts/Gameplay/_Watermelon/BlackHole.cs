//black hole script
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [Header("属性")]
    public float radius = 3.5f;      // 吸引半径
    public float pullForce = 80f;  // 吸引力
    public float duration = 2.0f;  // 持续时间
    public float destroyRadius = 0.3f; // 进入此半径即被摧毁

    // 在游戏开始时调用一次
    void Start()
    {
        // 播放一个生成音效（可选）
        // FindObjectOfType<AudioManager>().PlaySFX(YourSpawnSoundClip);

        // 在 duration 指定的秒数后，自动销毁这个黑洞对象
        Destroy(gameObject, duration);
    }

    // FixedUpdate 用于处理物理相关的计算，比Update更稳定
    void FixedUpdate()
    {
        // 找出在吸引半径内的所有碰撞体
        Collider2D[] collidersInRadius = Physics2D.OverlapCircleAll(transform.position, radius);

        // 遍历找到的每一个碰撞体
        foreach (var collider in collidersInRadius)
        {
            // 我们只对“Fruit”标签的物体感兴趣
            if (collider.CompareTag("Fruit"))
            {
                // 获取水果的刚体组件
                Rigidbody2D rb = collider.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    // 计算从水果指向黑洞中心的方向
                    Vector2 direction = (Vector2)transform.position - rb.position;

                    // 如果水果离中心非常近，就直接销毁它
                    if (direction.magnitude < destroyRadius)
                    {
                        // 在这里可以播放一个销毁特效
                        // EffectManager.Instance.PlayDestroyEffect(collider.transform.position);
                        Destroy(collider.gameObject);
                    }
                    else // 如果还在外围，就对它施加力
                    {
                        // 使用 AddForce 将水果拉向中心。ForceMode.Impulse 效果更突然
                        rb.AddForce(direction.normalized * pullForce * Time.deltaTime, ForceMode2D.Impulse);
                    }
                }
            }
        }
    }

    // (可选) 在编辑器中绘制出范围，方便调试
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, radius); // 绘制吸引范围
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, destroyRadius); // 绘制摧毁范围
    }
}