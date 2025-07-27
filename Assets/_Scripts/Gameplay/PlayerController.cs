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
        // 取消订阅
        EventManager.Instance.Unsubscribe<(string, GameEvents.SceneStateType)>(GameEvents.OnSceneStateChanged, OnSceneChanged);
        EventManager.Instance.Unsubscribe<string>("OnPlayerSpawnPointChange", GoToSpawnPoint);
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
        // 在FixedUpdate中应用物理移动，效果更平滑
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
            // 根据场景名称，决定玩家是否可见
            if (sceneInfo.sceneName == "TownScene" || sceneInfo.sceneName == "FarmScene")
            {
                // 在世界探索场景中，显示玩家
                SetPlayerVisibility(true);
                GoToSpawnPoint("Default"); // 并移动到出生点
            }
            else if (sceneInfo.sceneName == "WatermelonScene")
            {
                // 在独立玩法场景中，隐藏玩家
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
}