// Fruit.cs
using UnityEngine;

public class Fruit : MonoBehaviour
{
    [Header("Ë®¹ûÊôÐÔ")]
    public FruitType fruitType;
    public int fuirtScore;

    [HideInInspector]
    public bool isCombined = false;
    private bool hasCollided = false;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasCollided && rb.velocity.magnitude > 0.1f)
        {
            hasCollided = true;
            if (GameManager.gameManagerInstance != null) { GameManager.gameManagerInstance.PlayHitSound(); }
        }

        if (collision.gameObject.CompareTag("Fruit"))
        {
            Fruit otherFruit = collision.gameObject.GetComponent<Fruit>();
            if (otherFruit != null && otherFruit.fruitType == this.fruitType && !this.isCombined && !otherFruit.isCombined)
            {
                if (transform.position.y > otherFruit.transform.position.y)
                {
                    this.isCombined = true;
                    otherFruit.isCombined = true;
                    if (GameManager.gameManagerInstance != null) { GameManager.gameManagerInstance.RequestCombine(this, otherFruit); }
                }
            }
        }
    }
}
