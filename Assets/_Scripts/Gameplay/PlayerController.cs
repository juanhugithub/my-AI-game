// PlayerController.cs
using UnityEngine;
using System.Collections.Generic; // For Dictionary

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // ����Inspector�е����ƶ��ٶ�
    [Header("�������")] // ��������
    [SerializeField] private SpriteRenderer playerSprite; // ��ҵĿɼ������Ρ�
    [SerializeField] private Collider2D playerCollider;  // ��ҵ���ײ��
    private Rigidbody2D rb;
    private Vector2 moveInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        // �����������δ��Inspector��ָ�����Զ���ȡ���
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
        // ���ĳ����л��¼�
        EventManager.Instance.Subscribe<(string, GameEvents.SceneStateType)>(GameEvents.OnSceneStateChanged, OnSceneChanged);
        EventManager.Instance.Subscribe<string>("OnPlayerSpawnPointChange", GoToSpawnPoint);
    }
    private void OnDestroy()
    {
        // ȡ������
        EventManager.Instance.Unsubscribe<(string, GameEvents.SceneStateType)>(GameEvents.OnSceneStateChanged, OnSceneChanged);
        EventManager.Instance.Unsubscribe<string>("OnPlayerSpawnPointChange", GoToSpawnPoint);
    }
    private void Update()
    {
        // ֻ������ҿɼ�ʱ�Ž����ƶ�����
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
        // ��FixedUpdate��Ӧ�������ƶ���Ч����ƽ��
        rb.velocity = moveInput.normalized * moveSpeed;
    }
   
    /// <summary>
    /// ��һ���³����������ʱ���� 
    /// </summary>
    /// <param name="sceneInfo"></param>
    private void OnSceneChanged((string sceneName, GameEvents.SceneStateType state) sceneInfo)
    {
        // ���ؼ�������������� 'state' ����Ϊ 'sceneInfo.state'
        if (sceneInfo.state == GameEvents.SceneStateType.LoadedAdditive)
        {
            // ���ݳ������ƣ���������Ƿ�ɼ�
            if (sceneInfo.sceneName == "TownScene" || sceneInfo.sceneName == "FarmScene")
            {
                // ������̽�������У���ʾ���
                SetPlayerVisibility(true);
                GoToSpawnPoint("Default"); // ���ƶ���������
            }
            else if (sceneInfo.sceneName == "WatermelonScene")
            {
                // �ڶ����淨�����У��������
                SetPlayerVisibility(false);
            }
        }
    }
    /// <summary>
    /// ��������һ��ͳһ�ķ�����������ҵĿɼ��ԺͿɽ�����
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
    // �ƶ���ָ��ID�ĳ�����
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