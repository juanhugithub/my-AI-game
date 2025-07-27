// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// ��Ϸ״̬ö�٣�������GameManager�ļ��ڣ���Ϊ����GameManager�������
/// </summary>
public enum GameState
{
    Ready = 0,          // ��ʾ��ʼ�˵�
    StandBy = 1,        // �ȴ���Ҳ���
    InProgress = 2,     // ˮ�������У���Ҳ��ɲ���
    GameOver = 3,       // ��Ϸ������������
    CalculateScore = 4, // ���ռƷֽ׶�
    ItemTargeting = 5, // <-- ����״̬
    Paused = 6, // ����������Ϸ��ͣ״̬���������ò˵�
}
public class GameManager : MonoBehaviour
{
    public static GameManager gameManagerInstance;

    [Header("��Ϸ״̬")]
    public GameState gameState = GameState.Ready;

    [Header("����������")]
    public FruitSpawner fruitSpawner;
    public ScoreManager scoreManager;
    public UIManager uiManager;
    public TopLine topLine;
    public WeatherManager weatherManager;
    public AudioManager audioManager;

    [Header("������")]
    public AudioClip[] earlyTauntClips;
    public AudioClip[] midEncourageClips;
    public AudioClip[] lateDangerClips;
    public AudioClip[] celebrationClips;
    public AudioClip[] newRecordClips;
    public AudioClip[] normalEndClips;

    [Header("����Ч")]
    public AudioClip combineSfx;
    public AudioClip hitSfx;
    public AudioClip itemNotAvailableSfx; // <-- ���������߲�������Ч

    [Header("Σ���������")]
    public float dangerHeightThreshold = 3.5f;

    // --- ����ϵͳ ---
    [Header("����Ԥ����")]
    public GameObject blackHolePrefab;
    public GameObject dissolveEffectPrefab; // <-- ���������ڴ������������ЧԤ����

    [Header("����ϵͳ")]
    private Dictionary<ItemType, int> itemCounts;
    public int initialItemCount = 3;

    private System.Action<Vector3> onItemTargetSelected; // <-- ֮ǰȱʧ�ı�������

    private struct VoiceRequest { public FruitType type; public Vector3 position; }
    private List<VoiceRequest> voiceRequestsThisFrame = new List<VoiceRequest>();

    
    void Awake()
    {
        if (gameManagerInstance == null) { gameManagerInstance = this; } else { Destroy(gameObject); }
    }

    void Start()
    {
        uiManager.ShowStartMenu();
        scoreManager.LoadAndDisplayHighestScore();
        InitializeItems();
    }

    void Update()
    {
        if (gameState == GameState.StandBy)
        {
            if (Input.GetMouseButtonDown(0))
            {
                fruitSpawner.DropFruit();
                OnFruitDropped();
            }
            else
            {
                Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                fruitSpawner.MovePendingFruit(mouseWorldPos);
            }
        }
        else if (gameState == GameState.ItemTargeting)
        {
            if (Input.GetMouseButtonDown(0))
            {
                // ���onItemTargetSelected��ֵ��˵���Ǻڶ�ը��������Ҫѡ��λ�õĵ���
                if (onItemTargetSelected != null)
                {
                    Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    targetPosition.z = 0;
                    onItemTargetSelected.Invoke(targetPosition);
                }
                else // ����������ҩ��������Ҫѡ��ˮ���ĵ���
                {
                    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                    if (hit.collider != null && hit.collider.CompareTag("Fruit"))
                    {
                        Fruit selectedFruit = hit.collider.GetComponent<Fruit>();
                        if (selectedFruit != null)
                        {
                            ExecuteDissolvePotion(selectedFruit.fruitType);
                        }
                    }
                }
            }
        }
    }

    void LateUpdate()
    {
        if (voiceRequestsThisFrame.Count == 0) return;
        var bestRequest = voiceRequestsThisFrame.OrderByDescending(r => r.type).First();
        PlaySituationalVoice(bestRequest.type, bestRequest.position);
        voiceRequestsThisFrame.Clear();
    }

    // --- ����ϵͳ���ķ��� ---
    private void InitializeItems()
    {
        itemCounts = new Dictionary<ItemType, int>();
        foreach (ItemType type in System.Enum.GetValues(typeof(ItemType)))
        {
            itemCounts.Add(type, initialItemCount);
            uiManager.UpdateItemCountDisplay(type, itemCounts[type]);
        }
    }

    public void AddItem(ItemType type, int amount = 1)
    {
        if (itemCounts.ContainsKey(type))
        {
            itemCounts[type] += amount;
            uiManager.UpdateItemCountDisplay(type, itemCounts[type]);
        }
    }

    // --- ���߼���� ---
    public void ActivateDissolvePotion()
    {
        // --- �ؼ��޸������� ---
        if (itemCounts[ItemType.DissolvePotion] <= 0)
        {
            // ������Ч
            if (audioManager != null && itemNotAvailableSfx != null)
            {
                audioManager.PlaySFX(itemNotAvailableSfx);
            }
            // ��ʾ��ʾ�ı�
            if (uiManager != null)
            {
                uiManager.ShowTip("����ҩ���������㣡");
            }
            uiManager.CloseAllItemPanels(); // �ر��������
            return; // ֱ�ӷ��أ���ִ�к����߼�
        }
        if (gameState != GameState.StandBy && gameState != GameState.InProgress) return;

        itemCounts[ItemType.DissolvePotion]--;
        uiManager.UpdateItemCountDisplay(ItemType.DissolvePotion, itemCounts[ItemType.DissolvePotion]);
        uiManager.CloseAllItemPanels();
        // --- ��������ʾ��ʾ ---
        uiManager.ShowTip("��ѡ��һ��ˮ����������");

        gameState = GameState.ItemTargeting;
        onItemTargetSelected = null; // ȷ��ί��Ϊ�գ�����Update��������߼���߼�
        Debug.Log("����ҩ���Ѽ����ѡ��Ŀ��ˮ����");
    }

    private void ExecuteDissolvePotion(FruitType typeToDissolve)
    {
        Fruit[] allFruits = FindObjectsOfType<Fruit>();
        int dissolvedCount = 0;
        float scoreToAdd = 0;
        bool effectPlayed = false; // �޸�����������ʼ��effectPlayed

        foreach (Fruit fruit in allFruits)
        {
            if (fruit.fruitType == typeToDissolve)
            {
                scoreToAdd += fruit.fuirtScore;
                dissolvedCount++;

                // --- �ؼ��޸������� ---
                // ��ÿ����������ˮ��λ�ã�������ָ������Ч
                if (dissolveEffectPrefab != null)
                {
                    Instantiate(dissolveEffectPrefab, fruit.transform.position, Quaternion.identity);
                    effectPlayed = true;
                }
                Destroy(fruit.gameObject);
            }
        }
        if (effectPlayed)
        {
            uiManager.ShowTip($"�ɹ�����!");
        }
        else
        {
            uiManager.ShowTip("����û�и����͵�ˮ����");
        }
        // ������һ��ͨ�õ�ʹ�óɹ���Ч
        if (EffectManager.Instance != null)
        {
            // ����Ļ���벥��һ����Ч
            EffectManager.Instance.PlayCombineEffect(Vector3.zero);
        }

        scoreManager.AddScore(scoreToAdd);
        Debug.Log($"������ {dissolvedCount} ��ˮ����");
        RestoreToStandBy();
    }

    public void ActivateBlackHoleBomb()
    {
        // --- �ؼ��޸������� ---
        if (itemCounts[ItemType.BlackHoleBomb] <= 0)
        {
            if (audioManager != null && itemNotAvailableSfx != null)
            {
                audioManager.PlaySFX(itemNotAvailableSfx);
            }
            if (uiManager != null)
            {
                uiManager.ShowTip("�ڶ�**###**�������㣡");
            }
            uiManager.CloseAllItemPanels();
            return;
        }
        if (gameState != GameState.StandBy && gameState != GameState.InProgress) return;

        itemCounts[ItemType.BlackHoleBomb]--;
        uiManager.UpdateItemCountDisplay(ItemType.BlackHoleBomb, itemCounts[ItemType.BlackHoleBomb]);
        uiManager.CloseAllItemPanels();
        // --- ��������ʾ��ʾ ---
        uiManager.ShowTip("������Ļѡ��ڶ�λ��");
        uiManager.ShowTargetingIndicator(true); // <-- ��������ʾָʾ��

        gameState = GameState.ItemTargeting;
        onItemTargetSelected = DeployBlackHoleBomb;
        Debug.Log("�ڶ�ը���Ѽ����ѡ����λ�á�");
    }

    // --- ������֮ǰȱʧ�ķ��� ---
    private void DeployBlackHoleBomb(Vector3 position)
    {
        if (blackHolePrefab != null)
        {
            Instantiate(blackHolePrefab, position, Quaternion.identity);
        }
        uiManager.ShowTargetingIndicator(false); // <-- ����������ָʾ��
        // --- ������������Ч����ʾ ---
        uiManager.ShowTip("�ڶ�������!");
        

        onItemTargetSelected = null;
        RestoreToStandBy();
    }

    public void ActivateLuckyFruit()
    {
        // --- �ؼ��޸������� ---
        if (itemCounts[ItemType.LuckyFruit] <= 0)
        {
            if (audioManager != null && itemNotAvailableSfx != null)
            {
                audioManager.PlaySFX(itemNotAvailableSfx);
            }
            if (uiManager != null)
            {
                uiManager.ShowTip("���˹�ʵ�������㣡");
            }
            uiManager.CloseAllItemPanels();
            return;
        }

        if (gameState != GameState.StandBy) return;

        uiManager.CloseAllItemPanels();
        uiManager.ShowLuckyFruitPanel(true);
    }

    public void OnLuckyFruitSelected(int fruitIndex)
    {
        // **�������۳��߼��Ƶ����**
        if (itemCounts[ItemType.LuckyFruit] > 0)
        {
            itemCounts[ItemType.LuckyFruit]--;
            uiManager.UpdateItemCountDisplay(ItemType.LuckyFruit, itemCounts[ItemType.LuckyFruit]);

            fruitSpawner.CreateSpecificFruit(fruitIndex);
            uiManager.ShowTip("�¸�ˮ����Ϊ��׼����!");
        }
        uiManager.ShowLuckyFruitPanel(false);
       
    }

    private void RestoreToStandBy()
    {
        // ���ӳٻָ�
        Invoke(nameof(SetStateToStandBy), 0.2f);
    }

    private void SetStateToStandBy()
    {
        gameState = GameState.StandBy;
    }

    // --- ����ԭ�з��� ---
    public void StartGame()
    {
        gameState = GameState.StandBy;
        scoreManager.ResetCurrentScore();
        uiManager.ShowGameUI();

        WeatherType[] weathers = (WeatherType[])System.Enum.GetValues(typeof(WeatherType));
        WeatherType randomWeather = weathers[Random.Range(0, weathers.Length)];
        weatherManager.SetWeather(randomWeather);
        audioManager.PlayBGM(randomWeather);

        fruitSpawner.CreateFruit();
    }

    public void OnFruitDropped()
    {
        if (gameState == GameState.StandBy)
        {
            gameState = GameState.InProgress;
            Invoke(nameof(PrepareNextFruit), 0.5f);
        }
    }

    private void PrepareNextFruit()
    {
        if (gameState == GameState.InProgress || gameState == GameState.StandBy)
        {
            fruitSpawner.CreateFruit();
            gameState = GameState.StandBy;
        }
    }

    public void PlayHitSound()
    {
        audioManager.PlaySFX(hitSfx);
    }

    public void RequestCombine(Fruit fruitA, Fruit fruitB)
    {
        Vector3 centerPos = (fruitA.transform.position + fruitB.transform.position) / 2;
        FruitType combinedType = fruitA.fruitType;
        if (EffectManager.Instance != null) { EffectManager.Instance.PlayCombineEffect(centerPos); }
        voiceRequestsThisFrame.Add(new VoiceRequest { type = combinedType + 1, position = centerPos });
        fruitSpawner.Combine(combinedType, centerPos);
        scoreManager.AddScore(fruitA.fuirtScore);
        audioManager.PlaySFX(combineSfx);
        Destroy(fruitA.gameObject);
        Destroy(fruitB.gameObject);
    }

    /// <summary>
    /// ���޸ġ��˷��������������ġ���TopLine�����ġ����������Ϸ������
    /// </summary>
    public void TriggerFinalGameOver()
    {
        if (gameState == GameState.GameOver) return;
        gameState = GameState.GameOver;
        Debug.Log("��Ϸ���ս�������TopLine������");

        // �����Ž������������յġ�1:1�ĵ÷ֽ���ͳ���ж��
        int finalScore = Mathf.FloorToInt(scoreManager.TotalScore);
        WatermelonBridge.Instance.ReportFinalResult(finalScore);
    }


    private void PlaySituationalVoice(FruitType newFruitType, Vector3 combinePosition)
    {
        if (audioManager == null) return;
        AudioClip clipToPlay = null;
        if (newFruitType == FruitType.Eleven && celebrationClips.Length > 0)
        {
            clipToPlay = celebrationClips[Random.Range(0, celebrationClips.Length)];
        }
        else
        {
            bool isInDangerZone = combinePosition.y >= dangerHeightThreshold;
            if (isInDangerZone && lateDangerClips.Length > 0 && Random.value < 0.3f)
            {
                clipToPlay = lateDangerClips[Random.Range(0, lateDangerClips.Length)];
            }
            if (clipToPlay == null && Random.value < 0.25f)
            {
                if (newFruitType >= FruitType.Five && newFruitType <= FruitType.Nine && midEncourageClips.Length > 0)
                    clipToPlay = midEncourageClips[Random.Range(0, midEncourageClips.Length)];
                else if (newFruitType >= FruitType.Two && newFruitType <= FruitType.Four && earlyTauntClips.Length > 0)
                    clipToPlay = earlyTauntClips[Random.Range(0, earlyTauntClips.Length)];
            }
        }
        audioManager.PlayVoice(clipToPlay);
    }
    
    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    /// <summary>
    /// ֱ�ӽ�����Ϸ�����س���
    /// ����ǰ���ִ�5�۶һ��ɽ�ң���������߷֡�
    /// </summary>
    public void QuitGame()
    {
//        // ���浱ǰ��߷�
//        scoreManager.SaveHighestScore();
//        //������ֲ�Ϊ0������ǰ���ִ�5�۶һ��ɽ�ң�������������
//        long goldToAdd = Mathf.FloorToInt(scoreManager.TotalScore * 0.5f);
//        // ��ӽ�ҵ��������
//        DataManager.Instance.AddGold(goldToAdd);
//        //δ����UI����ʾ
//        Debug.Log($"��Ϸ��������ý��: {goldToAdd}����ǰ�������: {DataManager.Instance.PlayerData.Gold}");

//        // ������������������ϵͳ�ı�׼�������ؽӿ�
//        SceneLoader.Instance.LoadSceneAsync("TownScene", true);
//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#endif
        Debug.LogError("����QuitGame()�����������ˣ����ش�������ťӦ�����ӵ�WatermelonBridge��HandleReturnToTownFromSettings()���������������");
    }
}
