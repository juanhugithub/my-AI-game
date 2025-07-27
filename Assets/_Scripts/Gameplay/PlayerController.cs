// PlayerController.cs
using UnityEngine;
using System.Collections.Generic; // For Dictionary

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
        // �����ǵ�ȫ���������ȡ�ƶ�����
        moveInput = GameInput.GetMovementAxis();
    }

    private void FixedUpdate()
    {
        // ��FixedUpdate��Ӧ�������ƶ���Ч����ƽ��
        rb.velocity = moveInput.normalized * moveSpeed;
    }
    // ��һ���³����������ʱ����
    private void OnSceneChanged((string sceneName, GameEvents.SceneStateType state) sceneInfo)
    {
        if (sceneInfo.state == GameEvents.SceneStateType.LoadedAdditive)
        {
            // �򵥵��߼���ÿ�μ����³������������ƶ����ó���Ĭ�ϳ�����
            GoToSpawnPoint("Default");
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