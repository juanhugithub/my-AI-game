// PlayerController.cs
using UnityEngine;
using System.Collections.Generic; // For Dictionary

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
        // 从我们的全局输入类获取移动输入
        moveInput = GameInput.GetMovementAxis();
    }

    private void FixedUpdate()
    {
        // 在FixedUpdate中应用物理移动，效果更平滑
        rb.velocity = moveInput.normalized * moveSpeed;
    }
    // 当一个新场景加载完成时调用
    private void OnSceneChanged((string sceneName, GameEvents.SceneStateType state) sceneInfo)
    {
        if (sceneInfo.state == GameEvents.SceneStateType.LoadedAdditive)
        {
            // 简单的逻辑：每次加载新场景，都尝试移动到该场景默认出生点
            GoToSpawnPoint("Default");
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