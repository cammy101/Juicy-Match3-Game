using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTitle : MonoBehaviour
{
    public int hitPoints;

    private SpriteRenderer sprite;

    private GoalManager goalManager;

    public void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        goalManager = FindObjectOfType<GoalManager>();
    }

    public void Update()
    {
        if (hitPoints <= 0)
        {
            if (goalManager != null)
            {
                goalManager.CompareGoal(this.gameObject.tag);
                goalManager.UpdateGoals();
            }

            Destroy(this.gameObject);
        }
    }

    public void TakeHit(int hitAmount)
    {
        hitPoints -= hitAmount;
        MakeLighter();
    }

    void MakeLighter()
    {
        Color color = sprite.color;

        float newAlpha = color.a * .5f;

        sprite.color = new Color(color.r, color.g, color.b, newAlpha);
    }
}
