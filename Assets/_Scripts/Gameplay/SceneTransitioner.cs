// SceneTransitioner.cs
using UnityEngine;

public class SceneTransitioner : MonoBehaviour
{
    [Tooltip("Ҫ���ص�Ŀ�곡������")]
    public string targetSceneName;
    

    private bool isPlayerInRange = false;
    private void Update()
    {
        if (isPlayerInRange && GameInput.GetInteractActionDown())
        {
            // ��ȡ��ǰ�ű����ڳ���������
            string currentSceneName = gameObject.scene.name;

            // ����SceneLoader���½ӿڣ��ṩ��ȷ�ġ�����������ȥ��ָ��
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