using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    wait,
    move,
    win,
    lose,
    pause
}

public enum TileKind
{
    Breakable,
    Blank,
    Normal
}

[System.Serializable]
public class MatchType
{
    public int type;
    public string color;
}

[System.Serializable]
public class TileType
{
    public int x;
    public int y;

    public TileKind tileKind;
}

[RequireComponent(typeof(Explodable))]
public class Board : MonoBehaviour
{
    public GameState currentState = GameState.move;

    public int width;
    public int height;
    public int offSet;
    public int basePieceValue = 20;
    public int[] scoreGoals;

    private int streakValue = 1;

    public bool AllJuice = true;
    public bool someJuice = true;

    private bool[,] blankSpaces;

    private BackgroundTitle[,] breakableTiles;
 
    public GameObject[,] allDots;
    public GameObject[] dots;
    public GameObject tilePrefab;
    public GameObject destroyEffect;
    public GameObject breakableTilePrefab;

    [Header("Match Variables")]
    public MatchType matchType;
    private FindMatches findMatches;
    public Dot currentDot;
    public TileType[] boardLayout;
    private ScoreManager scoreManager;
    private GoalManager goalManager;

    public float refillDelay = 0.5f;

    public AudioClip connectNormal;
    public AudioSource audioSource;


    // Start is called before the first frame update
    void Start()
    {
        findMatches = FindObjectOfType<FindMatches>();
        scoreManager = FindObjectOfType<ScoreManager>();
        goalManager = FindObjectOfType<GoalManager>();

        blankSpaces = new bool[width, height];
        allDots = new GameObject[width, height];

        breakableTiles = new BackgroundTitle[width, height];

        SetUp();

        currentState = GameState.pause;
    }

    public void GenerateBlanks()
    {
        for (int i = 0; i < boardLayout.Length; i++)
        {
            if(boardLayout[i].tileKind == TileKind.Blank)
            {
                blankSpaces[boardLayout[i].x, boardLayout[i].y] = true;
            }
        }
    }

    public void GenerateBreakables()
    {
        //Look at all tiles in layout
        for (int i = 0; i < boardLayout.Length; i++)
        {
            //if any tiles are "break tile"
            if (boardLayout[i].tileKind == TileKind.Breakable)
            {
                Vector2 tempPos = new Vector2(boardLayout[i].x, boardLayout[i].y);
                //Create "break tile" at that position
                GameObject tile = Instantiate(breakableTilePrefab, tempPos, Quaternion.identity);

                breakableTiles[boardLayout[i].x, boardLayout[i].y] = tile.GetComponent<BackgroundTitle>();
            }
        }
    }

    public void SetUp()
    {
        GenerateBlanks();
        GenerateBreakables();
        for (int i = 0; i < width; i ++)
        {
            for (int j = 0; j < height; j++)
            {
                if (!blankSpaces[i, j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    GameObject backgroundTile = Instantiate(tilePrefab, tempPosition, Quaternion.identity) as GameObject;
                    backgroundTile.transform.parent = this.transform;
                    backgroundTile.name = "( " + i + ", " + j + " )";

                    int dotToUse = Random.Range(0, dots.Length);

                    int maxIterations = 0;

                    while (MatchesAt(i, j, dots[dotToUse]) && maxIterations < 100)
                    {
                        dotToUse = Random.Range(0, dots.Length);
                        maxIterations++;
                    }
                    maxIterations = 0;

                    GameObject dot = Instantiate(dots[dotToUse], tempPosition, Quaternion.identity);
                    dot.GetComponent<Dot>().row = j;
                    dot.GetComponent<Dot>().column = i;
                    dot.transform.parent = this.transform;
                    dot.name = "( " + i + ", " + j + " )";
                    allDots[i, j] = dot;
                }
            }

        }
    }

    private bool MatchesAt(int coloumn, int row, GameObject piece)
    {
        if (coloumn > 1 && row > 1)
        {
            if(allDots[coloumn -1, row] !=null && allDots[coloumn -2, row] !=null)
            {
                if (allDots[coloumn - 1, row].tag == piece.tag && allDots[coloumn - 2, row].tag == piece.tag)
                {
                    return true;
                }
            }

            if(allDots[coloumn, row -1] !=null && allDots[coloumn, row -2] !=null)
            {
                if (allDots[coloumn, row -1].tag == piece.tag && allDots[coloumn, row - 2].tag == piece.tag)
                {
                    return true;
                }
            }

        }

        else if (coloumn <= 1 || row <= 1)
        {
            if (row > 1)
            {
                if(allDots[coloumn, row -1] !=null && allDots[coloumn, row -2] !=null)
                {
                    if (allDots[coloumn, row - 1].tag == piece.tag && allDots[coloumn, row - 2].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (coloumn > 1)
            {
                if (allDots[coloumn -1, row] !=null && allDots[coloumn -2, row] !=null)
                {
                    if (allDots[coloumn - 1, row].tag == piece.tag && allDots[coloumn - 2, row].tag == piece.tag)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private MatchType ColumnOrRow()
    {
        //make copy of current matches
        List<GameObject> matchCopy = findMatches.currentMatches as List<GameObject>;

        matchType.type = 0;
        matchType.color = "";

        //cycle through all match copy decide if a bomb is needed
        for (int i = 0; i < matchCopy.Count; i++)
        {
            //store dot
            Dot thisDot = matchCopy[i].GetComponent<Dot>();

            string color = matchCopy[i].tag;

            int column = thisDot.column;
            int row = thisDot.row;
            int columnMatch = 0;
            int rowMatch = 0;

            //go through rest of dots and compare
            for (int j = 0; j < matchCopy.Count; j++)
            {
                //store next dot
                Dot nextDot = matchCopy[j].GetComponent<Dot>();

                if (nextDot == thisDot)
                {
                    continue;
                }

                if (nextDot.column == thisDot.column && nextDot.tag == color)
                {
                    columnMatch++;
                }

                if (nextDot.row == thisDot.row && nextDot.tag == color)
                {
                    rowMatch++;
                }
            }
            //return 3 if column or row, return 2 if adjacent, return 1 if colour
            if (columnMatch == 4 || rowMatch == 4)
            {
                matchType.type = 1;
                matchType.color = color;
                return matchType;
            }
            else if (columnMatch == 2 && rowMatch ==2)
            {
                matchType.type = 2;
                matchType.color = color;
                return matchType;
            }
            else if (columnMatch == 3 || rowMatch == 3)
            {
                matchType.type = 3;
                matchType.color = color;
                return matchType;
            }
        }

        matchType.type = 0;
        matchType.color = "";
        return matchType;
        #region comment
        /*
        // Checks to see whether or not to rewad players with adjacent explosion or colour explosion
        int numberHorizontal = 0;
        int numberVertical = 0;
        Dot firstPiece = findMatches.currentMatches[0].GetComponent<Dot>();
        // Go through everything in current matches, if same row as first row add to hori, if first colum at vert
        if (firstPiece != null)
        {
            foreach (GameObject currentPiece in findMatches.currentMatches)
            {
                Dot dot = currentPiece.GetComponent<Dot>();

                if (dot.row == firstPiece.row)
                {
                    numberHorizontal++;
                }

                if (dot.column == firstPiece.column)
                {
                    numberVertical++;
                }
            }
        }

        return (numberVertical == 5 || numberHorizontal == 5);
        */
        #endregion
    }

    private void CheckToMakeBombs()
    {
        if(findMatches.currentMatches.Count > 3)
        {
            MatchType typeOfMatch = ColumnOrRow();

            if (typeOfMatch.type == 1)
            {
                //Make Colour Explosion
                if (currentDot != null && currentDot.isMatched && currentDot.tag == typeOfMatch.color)
                {
                    currentDot.isMatched = false;
                    currentDot.MakeColourBomb();
                }
                else
                {
                    if (currentDot.otherDot != null)
                    {
                        Dot otherDot = currentDot.otherDot.GetComponent<Dot>();

                        if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                        {
                            otherDot.isMatched = false;
                            otherDot.MakeColourBomb();
                        }
                    }
                }
            }
            else if (typeOfMatch.type == 2)
            {
                //Make Adjacent Explosion
                //Debug.Log("Create Adjacent Explosion");

                if (currentDot != null && currentDot.isMatched && currentDot.tag == typeOfMatch.color)
                {
                    currentDot.isMatched = false;
                    currentDot.MakeAdjacentBomb();
                }
                else if (currentDot.otherDot != null)
                {
                    Dot otherDot = currentDot.otherDot.GetComponent<Dot>();

                    if (otherDot.isMatched && otherDot.tag == typeOfMatch.color)
                    {
                        otherDot.isMatched = false;
                        otherDot.MakeAdjacentBomb();
                    }
                }
            }
            else if (typeOfMatch.type == 3)
            {
                findMatches.CheckExplosion(typeOfMatch);
            }
        }

        #region comment
        /*
        if (findMatches.currentMatches.Count == 4 || findMatches.currentMatches.Count == 7)
        {
            findMatches.CheckExplosion();
        }

        if (findMatches.currentMatches.Count == 5 || findMatches.currentMatches.Count == 8)
        {
            if (ColumnOrRow())
            {
                //Make Colour Explosion
                //Debug.Log("Create Colour Explosion");
                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isColourBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeColourBomb();
                        }
                    }
                }
                else
                {
                    if (currentDot.otherDot != null)
                    {
                        Dot otherDot = currentDot.otherDot.GetComponent<Dot>();

                        if (otherDot.isMatched)
                        {
                            if (!otherDot.isColourBomb)
                            {
                                otherDot.isMatched = false;
                                otherDot.MakeColourBomb();
                            }
                        }
                    }
                }

            }
            else
            {
                //Make Adjacent Explosion
                //Debug.Log("Create Adjacent Explosion");

                if (currentDot != null)
                {
                    if (currentDot.isMatched)
                    {
                        if (!currentDot.isAdjacentBomb)
                        {
                            currentDot.isMatched = false;
                            currentDot.MakeAdjacentBomb();
                        }
                    }
                }
                else
                {
                    if (currentDot.otherDot != null)
                    {
                        Dot otherDot = currentDot.otherDot.GetComponent<Dot>();

                        if (otherDot.isMatched)
                        {
                            if (!otherDot.isAdjacentBomb)
                            {
                                otherDot.isMatched = false;
                                otherDot.MakeAdjacentBomb();
                            }
                        }
                    }
                }
            }
        } 
        */
        #endregion
    }

    private void DestroyMatchesAt(int column, int row)
    {
        if(allDots[column, row].GetComponent<Dot>().isMatched)
        {
            if(breakableTiles[column, row] != null)
            {
                //if it does need to break take one hit
                breakableTiles[column, row].TakeHit(1);

                if (breakableTiles[column, row].hitPoints <= 0)
                {
                    breakableTiles[column, row] = null;
                }
            }

            if (goalManager != null)
            {
                goalManager.CompareGoal(allDots[column, row].tag.ToString());
                goalManager.UpdateGoals();
            }

            if (AllJuice == true || someJuice == true)
            {
                GameObject particale = Instantiate(destroyEffect, allDots[column, row].transform.position, Quaternion.identity);

                Destroy(particale, .5f);
            }

            if (AllJuice == true)
            {
                allDots[column, row].GetComponent<Dot>().explodable.generateFragments();
                ExplosionForce ef = GameObject.FindObjectOfType<ExplosionForce>();
                ef.doExplosion(transform.position);
            }

            //StartCoroutine(DestroysAfter(column, row, allDots[column, row]));

            if (AllJuice == true || someJuice == true)
            {
                audioSource.enabled = true;

                if (!audioSource.isPlaying)
                {
                    audioSource.clip = connectNormal;
                    audioSource.Play();
                }
            }
     
            Destroy(allDots[column, row]);
            //allDots[column, row].GetComponent<Renderer>().enabled = false;
            scoreManager.IncreaseScore(basePieceValue * streakValue);
            allDots[column, row] = null;
        }
    }

    //public IEnumerator DestroyFragmentsAfter(int column, int row, GameObject dot)
    //{
    //    yield return new WaitForSeconds(.1f);

    //    for (int i = dot.GetComponent<Explodable>().fragments.Count - 1; i >= 0; i--)
    //    {
    //        Destroy(dot.GetComponent<Explodable>().fragments[i]);
    //    }

    //    Destroy(dot);
    //}

    public void DestroyMatches()
    {
        //How many elements are in matched pieces list
        if (findMatches.currentMatches.Count >= 4)
        {
            CheckToMakeBombs();
        }

        findMatches.currentMatches.Clear();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(LowerRowReworkCo());
    }

    private IEnumerator LowerRowReworkCo()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if current spot is not blank and is empty
                if(!blankSpaces[i,j] && allDots[i,j] == null)
                {
                    //loop from space above to top of column
                    for(int k = j + 1; k < height; k++)
                    {
                        //if dot is found
                        if(allDots[i,k] != null)
                        {
                            //move dot to empty space
                            allDots[i, k].GetComponent<Dot>().row = j;
                            //set that spot to null
                            allDots[i, k] = null;
                            //break loop
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(refillDelay * 0.5f);
        StartCoroutine(FillBoardCo());
    }

    private IEnumerator LowerRowCo()
    {
        int nullCount = 0;

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] == null)
                {
                    nullCount++;
                }
                else if (nullCount > 0)
                {           
                    allDots[i, j].GetComponent<Dot>().row -= nullCount;
                    allDots[i, j] = null;
                }
            }

            nullCount = 0;
        }

        yield return new WaitForSeconds(refillDelay * 0.4f);

        StartCoroutine(FillBoardCo());
    }

    private void RefillBoard()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i, j] == null && !blankSpaces[i,j])
                {
                    Vector2 tempPosition = new Vector2(i, j + offSet);
                    int dotTosue = Random.Range(0, dots.Length);

                    int maxIterations = 0;

                   
                    while (MatchesAt(i, j, dots[dotTosue]) && maxIterations < 100)
                    {
                        maxIterations++;
                        dotTosue = Random.Range(0, dots.Length);
                    }

                    maxIterations = 0;

                    GameObject piece = Instantiate(dots[dotTosue], tempPosition, Quaternion.identity);
                    allDots[i, j] = piece;
                    piece.GetComponent<Dot>().row = j;
                    piece.GetComponent<Dot>().column = i;
                }
            }
        }
    }

    private bool MatchesonBoard()
    {
        findMatches.FindAllMathces();

        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i, j] != null)
                {
                    if(allDots[i, j].GetComponent<Dot>().isMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(refillDelay);

        RefillBoard();

        yield return new WaitForSeconds(refillDelay);

        while (MatchesonBoard())
        {
            //yield return new WaitForSeconds(refillDelay);
            streakValue++;
            DestroyMatches();
            yield break;
            //yield return new WaitForSeconds(2 * refillDelay);
        }
        //findMatches.currentMatches.Clear();
        currentDot = null;

        if(IsDeadLock())
        {
            StartCoroutine(ShuffleBoard());
        }

        yield return new WaitForSeconds(refillDelay);

        if(currentState != GameState.pause)
        {
            currentState = GameState.move;
        }
        streakValue = 1;
    }

    //helper function for amending deadlock situations
    private void SwitchPieces(int column, int row, Vector2 direction)
    {
        //take second piece and save it
        GameObject holder = allDots[column + (int)direction.x, row + (int)direction.y] as GameObject;
        //switch first dot to be second position
        allDots[column + (int)direction.x, row + (int)direction.y] = allDots[column, row];
        //set first dot to be the second dot
        allDots[column, row] = holder;
    }

    //Checks the enitre board for matches
    private bool CheckForMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    //Make sure that one and two to the right are in board
                    if (i < width - 2)
                    {
                        //Check if dots to right and two to the righ exist
                        if (allDots[i + 1, j] != null && allDots[i + 2, j] != null)
                        {
                            if (allDots[i + 1, j].tag == allDots[i, j].tag && allDots[i + 2, j].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }

                    //Make sure 2 above is in board
                    if (j < height - 2)
                    {
                        //Check if dots above exist
                        if (allDots[i, j + 1] != null && allDots[i, j + 2] != null)
                        {
                            if (allDots[i, j + 1].tag == allDots[i, j].tag && allDots[i, j + 2].tag == allDots[i, j].tag)
                            {
                                return true;
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    public bool SwitchAndCheck(int column, int row, Vector2 direction)
    {
        SwitchPieces(column, row, direction);

        if(CheckForMatches())
        {
            SwitchPieces(column, row, direction);
            return true;
        }

        SwitchPieces(column, row, direction);
        return false;
    }

    private bool IsDeadLock()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if(allDots[i, j] != null)
                {
                    if (i < width - 1)
                    {
                        if(SwitchAndCheck(i, j, Vector2.right))
                        {
                            return false;
                        }
                    }

                    if (j < height - 1)
                    {
                        if(SwitchAndCheck(i, j, Vector2.up))
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    private IEnumerator ShuffleBoard()
    {
        yield return new WaitForSeconds(0.5f);
        //Create a list of game objects
        List<GameObject> newBoard = new List<GameObject>();
        //Add every piece to this list
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                if (allDots[i, j] != null)
                {
                    newBoard.Add(allDots[i, j]);
                }
            }
        }
        yield return new WaitForSeconds(0.5f);
        //for every spot on the board. . . 
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                //if this spot shouldn't be blank
                if (!blankSpaces[i, j])
                {
                    //Pick a random number
                    int pieceToUse = Random.Range(0, newBoard.Count);

                    //Assign the column to the piece
                    int maxIterations = 0;

                    while (MatchesAt(i, j, newBoard[pieceToUse]) && maxIterations < 100)
                    {
                        pieceToUse = Random.Range(0, newBoard.Count);
                        maxIterations++;
                    }
                    //Make a container for the piece
                    Dot piece = newBoard[pieceToUse].GetComponent<Dot>();
                    maxIterations = 0;
                    piece.column = i;
                    //Assign the row to the piece
                    piece.row = j;
                    //Fill in the dots array with this new piece
                    allDots[i, j] = newBoard[pieceToUse];
                    //Remove it from the list
                    newBoard.Remove(newBoard[pieceToUse]);
                }
            }
        }

        //Check if still deadlock
        if (IsDeadLock())
        {
            StartCoroutine(ShuffleBoard());
        }
    }
}
