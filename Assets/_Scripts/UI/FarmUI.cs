// FarmUI.cs
using UnityEngine;
using UnityEngine.UI;

public class FarmUI : MonoBehaviour
{
    [SerializeField] private Button returnToTownButton;

    void Start()
    {
        if (returnToTownButton != null)
        {
            returnToTownButton.onClick.AddListener(OnReturnToTownClicked);
        }
    }

    private void OnReturnToTownClicked()
    {
        // 调用SceneLoader的标准切换方法，从农场回到小镇
        SceneLoader.Instance.Transition("FarmScene", "TownScene");
    }
}