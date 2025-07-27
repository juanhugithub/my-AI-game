// SceneTransitioner.cs
using UnityEngine;

public class SceneTransitioner : MonoBehaviour
{
    [Tooltip("要加载的目标场景名称")]
    public string targetSceneName;
    

    private bool isPlayerInRange = false;
    private void Update()
    {
        if (isPlayerInRange && GameInput.GetInteractActionDown())
        {
            // 获取当前脚本所在场景的名称
            string currentSceneName = gameObject.scene.name;

            // 调用SceneLoader的新接口，提供明确的“从哪来到哪去”指令
            SceneLoader.Instance.Transition(currentSceneName, targetSceneName);
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerInRange = false;
    }
}