// TopLine.cs
using UnityEngine;

public class TopLine : MonoBehaviour
{
    [Header("游戏结束计时")]
    public float gameOverTimeThreshold = 2.0f;
    private float timer = 0f;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (GameManager.gameManagerInstance.gameState == GameState.CalculateScore && collider.CompareTag("Fruit"))
        {
            GameManager.gameManagerInstance.scoreManager.AddScore(collider.GetComponent<Fruit>().fuirtScore);
            Destroy(collider.gameObject);
        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        // 【修改】增加了对 GameState.Paused 的检查
        if (GameManager.gameManagerInstance.gameState != GameState.Paused &&
            (GameManager.gameManagerInstance.gameState == GameState.StandBy || GameManager.gameManagerInstance.gameState == GameState.InProgress) &&
            collider.CompareTag("Fruit"))
        {
            timer += Time.deltaTime;
            if (timer >= gameOverTimeThreshold)
            {
                Debug.Log("Timer threshold reached! Calling TriggerFinalGameOver...");
                GameManager.gameManagerInstance.TriggerFinalGameOver();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.CompareTag("Fruit"))
        {
            timer = 0f;
        }
    }

    public void StartDescending()
    {
        StartCoroutine(DescendRoutine());
    }

    private System.Collections.IEnumerator DescendRoutine()
    {
        float targetY = -6.0f;
        float duration = 3.0f;
        float elapsedTime = 0f;
        Vector3 startPosition = transform.position;
        while (elapsedTime < duration)
        {
            float newY = Mathf.Lerp(startPosition.y, targetY, elapsedTime / duration);
            transform.position = new Vector3(startPosition.x, newY, startPosition.z);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = new Vector3(startPosition.x, targetY, startPosition.z);
    }
}
