using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PrintMetrics : MonoBehaviour
{

    public static int[] levelScore = new int [3];
    public static int currentLevel = 0;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void WriteString()
    {
        string path = Application.streamingAssetsPath + "/PlayerMetrics.txt";

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        int clicks;
        int movesToMatch;
        int wrongMoves;
        
        clicks = Dot.clickCounter;
        movesToMatch = Dot.moveToMatchCounter;
        wrongMoves = Dot.wrongMoveCounter;

        if (!File.Exists(path))
        {
            File.WriteAllText(path, "Metrics \n \n");
        }

        int totalScore = 0;

        foreach (int score in levelScore)
        {
            totalScore += score;
        }

        string content = "Amount of clicks in total this build: " + clicks + "\n" +
            "Amount of matches in total made: " + movesToMatch + 
            "\n" + "Amount of wrong moves made: " + wrongMoves + "\n" +
            "Total score of level: " + totalScore + "\n" + "Total moves to match: " + (clicks - wrongMoves);

        File.AppendAllText(path, content);
    }
}
