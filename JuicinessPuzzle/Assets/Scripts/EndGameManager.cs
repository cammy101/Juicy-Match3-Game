using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public enum GameType
{
    Moves,
    Time
}

[System.Serializable]
public class EndGameRequirments
{
    public GameType gameType;

    public int counterValue;
}

public class EndGameManager : MonoBehaviour
{
    public GameObject movesLabel;
    public GameObject timeLabel;
    public GameObject youWinPanel;
    public GameObject tryAgainPanel;

    public ScoreManager scoreManager;

    public GameObject winParticale;

    public TextMeshProUGUI counter;

    public EndGameRequirments requirements;
    private Board board;

    public int currentCounterValue;

    public AudioClip winSound;
    private AudioSource winSource;

    public AudioSource audioSource;

    private float timerSeconds;

    // Start is called before the first frame update
    void Start()
    {
        SetupGame();

        board = FindObjectOfType<Board>();
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (requirements.gameType == GameType.Time && currentCounterValue > 0)
        {
            timerSeconds -= Time.deltaTime;

            if (timerSeconds <= 0)
            {
                DecreaseCounterValue();
                timerSeconds = 1;
            }
        }
    }

    void SetupGame()
    {
        currentCounterValue = requirements.counterValue;

        if (requirements.gameType == GameType.Moves)
        {
            movesLabel.SetActive(true);
            timeLabel.SetActive(false);
        }
        else
        {
            timerSeconds = 1;
            movesLabel.SetActive(false);
            timeLabel.SetActive(true);
        }

        counter.text = "" + currentCounterValue;
    }

    public void DecreaseCounterValue()
    {
        if (board.currentState != GameState.pause)
        {
            currentCounterValue--;
            counter.text = "" + currentCounterValue;

            if (currentCounterValue <= 0)
            {
                LoseGame();
            }
        }
    }

    public void WinGame()
    {
        if(board.AllJuice == true || board.someJuice)
        {
            audioSource.enabled = true;

            if (!audioSource.isPlaying)
            {
                audioSource.clip = winSound;
                audioSource.Play();
            }
        }

        if (board.AllJuice == true)
        {
            winParticale.SetActive(true);
        }

        youWinPanel.SetActive(true);
        PrintMetrics.levelScore[PrintMetrics.currentLevel] = scoreManager.score;

        if (PrintMetrics.currentLevel >= 2)
        {
            PrintMetrics.WriteString();
        }
        else 
        {
            PrintMetrics.currentLevel++;
        }
        board.currentState = GameState.win;
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }

    public void LoseGame()
    {
        tryAgainPanel.SetActive(true);
        board.currentState = GameState.lose;
        //Debug.Log("You Lose");
        currentCounterValue = 0;
        counter.text = "" + currentCounterValue;
        FadePanelController fade = FindObjectOfType<FadePanelController>();
        fade.GameOver();
    }
}
