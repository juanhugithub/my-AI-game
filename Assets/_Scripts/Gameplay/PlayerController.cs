// PlayerController.cs
using UnityEngine;
using System.Collections.Generic; // For Dictionary

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // 可在Inspector中调整移动速度
    [Header("组件引用")] // 【新增】
    [SerializeField] private SpriteRenderer playerSprite; // 玩家的可见“外形”
    [SerializeField] private Collider2D playerCollider;  // 玩家的碰撞体
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // 【新增】如果未在Inspector中指定，自动获取组件
        if (playerSprite == null)
        {
            playerSprite = GetComponentInChildren<SpriteRenderer>();
        }
        if (playerCollider == null)
        {
            playerCollider = GetComponent<Collider2D>();
        }
    }
    private void Start()
    {
        // 订阅场景切换事件
        EventManager.Instance.Subscribe<(string, GameEvents.SceneStateType)>(GameEvents.OnSceneStateChanged, OnSceneChanged);
        EventManager.Instance.Subscribe<string>("OnPlayerSpawnPointChange", GoToSpawnPoint);
    }
    private void OnDestroy()
    {
       
    }
    private void Update()
    {
        // 只有在玩家可见时才接受移动输入
        if (playerSprite.enabled)
        {
            moveInput = GameInput.GetMovementAxis();
        }
        else
        {
            moveInput = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (PlayerStateMachine.Instance == null || PlayerStateMachine.Instance.CurrentState != PlayerState.Gameplay)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        rb.velocity = moveInput.normalized * moveSpeed;
    }

    /// <summary>
    /// 当一个新场景加载完成时调用 
    /// </summary>
    /// <param name="sceneInfo"></param>
    private void OnSceneChanged((string sceneName, GameEvents.SceneStateType state) sceneInfo)
    {
        // 【关键修正】将错误的 'state' 修正为 'sceneInfo.state'
        if (sceneInfo.state == GameEvents.SceneStateType.LoadedAdditive)
        {
            // 【核心修改】根据场景名称，决定玩家是否可见
            if (sceneInfo.sceneName == "TownScene") // 只有在小镇才显示
            {
                SetPlayerVisibility(true);
                GoToSpawnPoint("Default");
            }
            else if (sceneInfo.sceneName == "FarmScene" || sceneInfo.sceneName == "WatermelonScene")
            {
                // 在农场和西瓜等独立玩法场景中，隐藏玩家
                SetPlayerVisibility(false);
            }
        }
    }
    /// <summary>
    /// 【新增】一个统一的方法来控制玩家的可见性和可交互性
    /// </summary>
    private void SetPlayerVisibility(bool isVisible)
    {
        if (playerSprite != null)
        {
            playerSprite.enabled = isVisible;
        }
        if (playerCollider != null)
        {
            playerCollider.enabled = isVisible;
        }
    }
    // 移动到指定ID的出生点
    private void GoToSpawnPoint(string spawnPointID)
    {
        var spawnPoints = FindObjectsOfType<SceneSpawnPoint>();
        foreach (var point in spawnPoints)
        {
            if (point.spawnPointID == spawnPointID)
            {
                transform.position = point.transform.position;
                return;
            }
        }
    }
    /// <summary>
    /// 【新增】当应用程序退出前，这个方法会被调用。
    /// 这是为持久化对象进行最终清理（如取消事件订阅）最安全的地方。
    /// </summary>
    private void OnApplicationQuit()
    {
        // 尽管此时EventManager.Instance几乎可以肯定是有效的，
        // 但保留这个检查是一个非常健壮的编程习惯。
        if (EventManager.Instance != null)
        {
            EventManager.Instance.Unsubscribe<(string, GameEvents.SceneStateType)>(GameEvents.OnSceneStateChanged, OnSceneChanged);
            EventManager.Instance.Unsubscribe<string>("OnPlayerSpawnPointChange", GoToSpawnPoint);
        }
    }
}