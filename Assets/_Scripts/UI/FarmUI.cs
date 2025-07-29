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
        // ����SceneLoader�ı�׼�л���������ũ���ص�С��
        SceneLoader.Instance.Transition("FarmScene", "TownScene");
    }
}