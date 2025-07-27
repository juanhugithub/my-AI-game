// GameManager.cs
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 游戏状态枚举，定义在GameManager文件内，因为它与GameManager紧密相关
/// </summary>
public enum GameState
{
    Ready = 0,          // 显示开始菜单
    StandBy = 1,        // 等待玩家操作
    InProgress = 2,     // 水果下落中，玩家不可操作
    GameOver = 3,       // 游戏结束条件触发
    CalculateScore = 4, // 最终计分阶段
    ItemTargeting = 5, // <-- 新增状态
    Paused = 6, // 【新增】游戏暂停状态，用于设置菜单
}
public class GameManager : MonoBehaviour
{
    public static GameManager gameManagerInstance;

    [Header("游戏状态")]
    public GameState gameState = GameState.Ready;

    [Header("管理器引用")]
    public FruitSpawner fruitSpawner;
    public ScoreManager scoreManager;
    public Watermelon_UIManager uiManager;
    public TopLine topLine;
    public WeatherManager weatherManager;

    [Header("危险区域参数")]
    public float dangerHeightThreshold = 3.5f;

    // --- 道具系统 ---
    [Header("道具预制体")]
    public GameObject blackHolePrefab;
    public GameObject dissolveEffectPrefab; // <-- 新增：用于存放您的消融特效预制体

    [Header("道具系统")]
    private Dictionary<ItemType, int> itemCounts;
    public int initialItemCount = 3;

    private System.Action<Vector3> onItemTargetSelected; // <-- 之前缺失的变量声明
    private GameObject offendingFruit; // 用于“继续游戏”功能
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
            // 【已重构】使用新的全局输入
            if (GameInput.GetDropAction())
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
            // 【已重构】使用新的全局输入
            if (GameInput.GetDropAction())
            {
                if (onItemTargetSelected != null)
                {
                    Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    targetPosition.z = 0;
                    onItemTargetSelected.Invoke(targetPosition);
                }
                else
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


    // --- 道具系统核心方法 ---
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

    // --- 道具激活方法 ---
    public void ActivateDissolvePotion()
    {
        if (itemCounts[ItemType.DissolvePotion] <= 0)
        {
            AudioManager.Instance.PlaySound("itemNotAvailableSfx");
            uiManager.ShowTip("消融药剂数量不足！");
            uiManager.CloseAllItemPanels();
            return;
        }
        if (gameState != GameState.StandBy && gameState != GameState.InProgress) return;

        itemCounts[ItemType.DissolvePotion]--;
        uiManager.UpdateItemCountDisplay(ItemType.DissolvePotion, itemCounts[ItemType.DissolvePotion]);
        uiManager.CloseAllItemPanels();
        // --- 新增：显示提示 ---
        uiManager.ShowTip("请选择一种水果进行消除");

        gameState = GameState.ItemTargeting;
        onItemTargetSelected = null; // 确保委托为空，这样Update里会走射线检测逻辑
        Debug.Log("消融药剂已激活，请选择目标水果。");
    }

    private void ExecuteDissolvePotion(FruitType typeToDissolve)
    {
        Fruit[] allFruits = FindObjectsOfType<Fruit>();
        int dissolvedCount = 0;
        float scoreToAdd = 0;
        bool effectPlayed = false; // 修复：声明并初始化effectPlayed

        foreach (Fruit fruit in allFruits)
        {
            if (fruit.fruitType == typeToDissolve)
            {
                scoreToAdd += fruit.fuirtScore;
                dissolvedCount++;

                // --- 关键修改在这里 ---
                // 在每个被消除的水果位置，播放您指定的特效
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
            uiManager.ShowTip($"成功消除!");
        }
        else
        {
            uiManager.ShowTip("场上没有该类型的水果。");
        }
        // 假设有一个通用的使用成功特效
        if (EffectManager.Instance != null)
        {
            // 在屏幕中央播放一个特效
            EffectManager.Instance.PlayCombineEffect(Vector3.zero);
        }

        scoreManager.AddScore(scoreToAdd);
        Debug.Log($"消除了 {dissolvedCount} 个水果。");
        RestoreToStandBy();
    }

    public void ActivateBlackHoleBomb()
    {
        if (itemCounts[ItemType.BlackHoleBomb] <= 0)
        {
            AudioManager.Instance.PlaySound("itemNotAvailableSfx");
            uiManager.ShowTip("黑洞炸弹数量不足！");
            uiManager.CloseAllItemPanels();
            return;
        }
        if (gameState != GameState.StandBy && gameState != GameState.InProgress) return;

        itemCounts[ItemType.BlackHoleBomb]--;
        uiManager.UpdateItemCountDisplay(ItemType.BlackHoleBomb, itemCounts[ItemType.BlackHoleBomb]);
        uiManager.CloseAllItemPanels();
        // --- 新增：显示提示 ---
        uiManager.ShowTip("请点击屏幕选择黑洞位置");
        uiManager.ShowTargetingIndicator(true); // <-- 新增：显示指示器

        gameState = GameState.ItemTargeting;
        onItemTargetSelected = DeployBlackHoleBomb;
        Debug.Log("黑洞炸弹已激活，请选择部署位置。");
    }

    // --- 这里是之前缺失的方法 ---
    private void DeployBlackHoleBomb(Vector3 position)
    {
        if (blackHolePrefab != null)
        {
            Instantiate(blackHolePrefab, position, Quaternion.identity);
        }
        uiManager.ShowTargetingIndicator(false); // <-- 新增：隐藏指示器
        // --- 新增：播放特效和提示 ---
        uiManager.ShowTip("黑洞已生成!");
        

        onItemTargetSelected = null;
        RestoreToStandBy();
    }

    public void ActivateLuckyFruit()
    {
        if (itemCounts[ItemType.LuckyFruit] <= 0)
        {
            AudioManager.Instance.PlaySound("itemNotAvailableSfx");
            uiManager.ShowTip("幸运果实数量不足！");
            uiManager.CloseAllItemPanels();
            return;
        }
        if (gameState != GameState.StandBy) return;

        uiManager.CloseAllItemPanels();
        uiManager.ShowLuckyFruitPanel(true);
    }

    public void OnLuckyFruitSelected(int fruitIndex)
    {
        // **把数量扣除逻辑移到这里！**
        if (itemCounts[ItemType.LuckyFruit] > 0)
        {
            itemCounts[ItemType.LuckyFruit]--;
            uiManager.UpdateItemCountDisplay(ItemType.LuckyFruit, itemCounts[ItemType.LuckyFruit]);

            fruitSpawner.CreateSpecificFruit(fruitIndex);
            uiManager.ShowTip("下个水果已为你准备好!");
        }
        uiManager.ShowLuckyFruitPanel(false);
       
    }

    private void RestoreToStandBy()
    {
        // 简单延迟恢复
        Invoke(nameof(SetStateToStandBy), 0.2f);
    }

    private void SetStateToStandBy()
    {
        gameState = GameState.StandBy;
    }

    // --- 其他原有方法 ---
    public void StartGame()
    {
        gameState = GameState.StandBy;
        scoreManager.ResetCurrentScore();
        uiManager.ShowGameUI();

        WeatherType[] weathers = (WeatherType[])System.Enum.GetValues(typeof(WeatherType));
        WeatherType randomWeather = weathers[Random.Range(0, weathers.Length)];
        weatherManager.SetWeather(randomWeather);
        //audioManager.PlayBGM(randomWeather);

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
        AudioManager.Instance.PlaySound("hitSfx");
    }

    public void RequestCombine(Fruit fruitA, Fruit fruitB)
    {
        Vector3 centerPos = (fruitA.transform.position + fruitB.transform.position) / 2;
        FruitType combinedType = fruitA.fruitType;
        if (EffectManager.Instance != null) { EffectManager.Instance.PlayCombineEffect(centerPos); }
        voiceRequestsThisFrame.Add(new VoiceRequest { type = combinedType + 1, position = centerPos });
        fruitSpawner.Combine(combinedType, centerPos);
        scoreManager.AddScore(fruitA.fuirtScore);
        AudioManager.Instance.PlaySound("combineSfx");
        Destroy(fruitA.gameObject);
        Destroy(fruitB.gameObject);
    }

    /// <summary>
    /// 【修改】此方法现在是真正的、由TopLine触发的、不可逆的游戏结束。
    /// </summary>
    public void TriggerFinalGameOver()
    {
        if (gameState == GameState.GameOver) return;
        gameState = GameState.GameOver;
        Debug.Log("游戏最终结束！由TopLine触发。");

        // 调用桥接器来处理最终的、1:1的得分结算和场景卸载
        int finalScore = Mathf.FloorToInt(scoreManager.TotalScore);
        WatermelonBridge.Instance.ReportFinalResult(finalScore);
    }


    
    public void ReturnToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    /// <summary>
    /// 直接结束游戏并返回城镇。
    /// 将当前积分打5折兑换成金币，并保存最高分。
    /// </summary>
    public void QuitGame()
    {
//        // 保存当前最高分
//        scoreManager.SaveHighestScore();
//        //如果积分不为0，将当前积分打5折兑换成金币，保留整数部分
//        long goldToAdd = Mathf.FloorToInt(scoreManager.TotalScore * 0.5f);
//        // 添加金币到玩家数据
//        DataManager.Instance.AddGold(goldToAdd);
//        //未来在UI上显示
//        Debug.Log($"游戏结束，获得金币: {goldToAdd}。当前金币总数: {DataManager.Instance.PlayerData.Gold}");

//        // 【场景串联】调用主系统的标准场景加载接口
//        SceneLoader.Instance.LoadSceneAsync("TownScene", true);
//#if UNITY_EDITOR
//        UnityEditor.EditorApplication.isPlaying = false;
//#endif
        Debug.LogError("错误：QuitGame()方法被调用了！“回大厅”按钮应该链接到WatermelonBridge的HandleReturnToTownFromSettings()方法，而不是这里！");
    }
}
