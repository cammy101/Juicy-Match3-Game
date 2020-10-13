using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlankGoal
{
    public int numberNeeded;
    public int numberCollected;

    public Sprite goalSprite;

    public string matchValue;
}

public class GoalManager : MonoBehaviour
{

    public BlankGoal[] levelGoals;

    public List<GoalPanel> currentGoals = new List<GoalPanel>();

    public GameObject goalPrefab;
    public GameObject goalPrefabSmall;
    public GameObject goalIntroPrefav;
    public GameObject goalGameParent;

    public ScoreManager scoreManager;
    public Board board;

    private EndGameManager endGame;

    // Start is called before the first frame update
    void Start()
    {
        SetupIntroGoals();
        endGame = FindObjectOfType<EndGameManager>();
        scoreManager = FindObjectOfType<ScoreManager>();
        board = FindObjectOfType<Board>();
    }


    public void SetupIntroGoals()
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            // New goal panel at goal intro parent position
            GameObject goal = Instantiate(goalPrefab, goalIntroPrefav.transform.position, Quaternion.identity);
            goal.transform.SetParent(goalIntroPrefav.transform);

            // set imahe and text of the goal
            GoalPanel panel = goal.GetComponent<GoalPanel>();
            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;


            // new goal panel at goal game parent
            GameObject gameGoal = Instantiate(goalPrefabSmall, goalGameParent.transform.position, Quaternion.identity);
            gameGoal.transform.SetParent(goalGameParent.transform);

            panel = gameGoal.GetComponent<GoalPanel>();

            currentGoals.Add(panel);

            panel.thisSprite = levelGoals[i].goalSprite;
            panel.thisString = "0/" + levelGoals[i].numberNeeded;
        }
    }

    public void UpdateGoals()
    {
        int goalsCompleted = 0;

        for(int i = 0; i < levelGoals.Length; i++)
        {
            currentGoals[i].thisText.text = "" + levelGoals[i].numberCollected + "/" + levelGoals[i].numberNeeded;

            if (levelGoals[i].numberCollected >= levelGoals[i].numberNeeded)
            {
                goalsCompleted++;
                currentGoals[i].thisText.text = "" + levelGoals[i].numberNeeded + "/" + levelGoals[i].numberNeeded;
            }
        }

        if (goalsCompleted >= levelGoals.Length /*|| scoreManager.score > board.scoreGoals[length-1]*/)
        {
            if (endGame != null)
            {
                endGame.WinGame();
            }
        }
    }

    public void CompareGoal(string goalToCompare)
    {
        for (int i = 0; i < levelGoals.Length; i++)
        {
            if (goalToCompare == levelGoals[i].matchValue)
            {
                levelGoals[i].numberCollected++;
            }
        }
    }
}
