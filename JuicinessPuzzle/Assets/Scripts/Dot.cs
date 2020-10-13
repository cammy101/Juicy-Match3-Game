using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Dot : MonoBehaviour
{
    //star fumction time to 0, currenttuimetomove + delte.time
    // function in completeting move, add time to list, float to string, format to certain decimal, when turning to string use string + = time "-"

    [Header ("Board Variables")]
    public int column;
    public int row;
    public int previousColumn;
    public int previousRow;
    public int targetX;
    public int targetY;

    public static int clickCounter = 0;
    public static int wrongMoveCounter = 0;
    public static int moveToMatchCounter = 0;

    public bool isMatched = false;
    public GameObject otherDot;

    private Board board;
    private FindMatches findMatches;
    private HintManager hintManager;
    private ScreenShake shake;
    private EndGameManager endGameManager;

    public Color mainColor;
    public Color clickedColor;

    private Vector2 firstClickPosition = Vector2.zero;
    private Vector2 finalClickPosition = Vector2.zero;
    private Vector2 tempPosition;

    [Header("Swipe Variables")]
    public float swipeAngle = 0;
    public float swipeResit = .5f;

    [Header("Power-Up Variables")]
    public bool isColourBomb;
    public bool isColumnBomb;
    public bool isRowBomb;
    public bool isAdjacentBomb;

    public GameObject rowArrow;
    public GameObject columnArrow;
    public GameObject colourBomb;
    public GameObject adjacentMarker;
    public GameObject colourEffect;

    public ParticleSystem hover;

    public Transform childObj;
    public Transform columnChildEffect;
    public Transform rowChildEffect;
    public Transform adjChildEffect;

    public Explodable explodable;

    [Header("Sounds")]
    public AudioClip click;
    private AudioSource audioSource;

    public AudioClip wrong;
    private AudioSource wrongSource;


    // Start is called before the first frame update
    void Start()
    {
        isColumnBomb = false;
        isRowBomb = false;
        isColourBomb = false;
        isAdjacentBomb = false;

        //board = FindObjectOfType<Board>();

        board = GameObject.FindWithTag("Board").GetComponent<Board>();

        findMatches = FindObjectOfType<FindMatches>();
        hintManager = FindObjectOfType<HintManager>();
        shake = FindObjectOfType<ScreenShake>();
        endGameManager = FindObjectOfType<EndGameManager>();

        explodable = GetComponent<Explodable>();

        audioSource = gameObject.GetComponent<AudioSource>();

        childObj = transform.Find("FollowMouse");

        if (board.AllJuice == true)
        {
            childObj.gameObject.SetActive(true);
        }
    }

    // Testing & Debug
    private void OnMouseOver()
    {
        if(Input.GetMouseButtonDown(1))
        {
            isColourBomb = true;
            GameObject marker = Instantiate(colourBomb, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
        }

        if (board.AllJuice == true || board.someJuice == true)
        {
            hover.gameObject.SetActive(true);
        }
        else
        {
            hover.gameObject.SetActive(false);
        }
    }

    private void OnMouseExit()
    {
        hover.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        targetX = column;
        targetY = row;

        // Move towards target
        if (Mathf.Abs(targetX - transform.position.x) > .1)
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .5f);

            if(board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
                findMatches.FindAllMathces();
            }
        }
        // Directly sets the position
        else
        {
            tempPosition = new Vector2(targetX, transform.position.y);
            transform.position = tempPosition;
        }

        if (Mathf.Abs(targetY - transform.position.y) > .1)
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = Vector2.Lerp(transform.position, tempPosition, .5f);
            
            if (board.allDots[column, row] != this.gameObject)
            {
                board.allDots[column, row] = this.gameObject;
                findMatches.FindAllMathces();
            }
        }
        // Directly sets the position
        else
        {
            tempPosition = new Vector2(transform.position.x, targetY);
            transform.position = tempPosition;
        }
    }

    public IEnumerator CheckMoveCo()
    { 
        if (isColourBomb)
        {
            //This piece is colour explosion, other piece colour to destroy
            findMatches.MatchPiecesOfColour(otherDot.tag);
            isMatched = true;
            moveToMatchCounter++;
        }
        else if (otherDot.GetComponent<Dot>().isColourBomb)
        {
            // The other piece is a color bomb, this piece has colour to destroy
            findMatches.MatchPiecesOfColour(this.gameObject.tag);
            otherDot.GetComponent<Dot>().isMatched = true;
            moveToMatchCounter++;
        }

        yield return new WaitForSeconds(.5f);

        if (otherDot != null)
        {
            if (!isMatched && !otherDot.GetComponent<Dot>().isMatched)
            {
                otherDot.GetComponent<Dot>().row = row;
                otherDot.GetComponent<Dot>().column = column;
                row = previousRow;
                column = previousColumn;


                audioSource.enabled = true;

                if (!audioSource.isPlaying)
                {
                    audioSource.clip = wrong;
                    audioSource.Play();
                }

                if (board.AllJuice == true || board.someJuice == true)
                {
                    shake.TriggerShake();
                }

                wrongMoveCounter++;

                yield return new WaitForSeconds(.5f);
                board.currentDot = null;
                board.currentState = GameState.move;
            }
            else
            {
                if (endGameManager != null)
                {
                    if (endGameManager.requirements.gameType == GameType.Moves)
                    {
                        endGameManager.DecreaseCounterValue();
                    }
                }

                board.DestroyMatches();
            }
        }
    }

    private void OnMouseDown()
    {

        //Destroy Hint
        if(hintManager != null)
        {
            hintManager.DestroyHint();
        }

        if(board.currentState == GameState.move)
        {
            firstClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        mySprite.color = clickedColor;

        clickCounter++;


        audioSource.enabled = true;

        if (!audioSource.isPlaying)
        {
            audioSource.clip = click;
            audioSource.Play();
        }
    }

    private void OnMouseUp()
    {
        if (board.currentState == GameState.move)
        {
            finalClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CalculateAngle();
        }

        SpriteRenderer mySprite = GetComponent<SpriteRenderer>();
        mySprite.color = mainColor;
    }

    void CalculateAngle()
    {
        if (Mathf.Abs(finalClickPosition.y - firstClickPosition.y) > swipeResit || Mathf.Abs(finalClickPosition.x - firstClickPosition.x) > swipeResit)
        {
            board.currentState = GameState.wait;
            swipeAngle = Mathf.Atan2(finalClickPosition.y - firstClickPosition.y, finalClickPosition.x - firstClickPosition.x) * 180 / Mathf.PI;
            MovePieces();
            board.currentDot = this;
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void MovePiecesRework(Vector2 direction)
    {
        otherDot = board.allDots[column + (int)direction.x, row + (int)direction.y];
        previousRow = row;
        previousColumn = column;
        if (otherDot != null)
        {
            otherDot.GetComponent<Dot>().column += -1 * (int)direction.x;
            otherDot.GetComponent<Dot>().row += -1 * (int)direction.y;
            column += (int)direction.x;
            row += (int)direction.y;
            StartCoroutine(CheckMoveCo());
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void MovePieces()
    {
        // Right Swipe
        if (swipeAngle > -45 && swipeAngle <=  45 && column < board.width - 1)
        {
            MovePiecesRework(Vector2.right);
        }
        // Up Swipe
        else if (swipeAngle > 45 && swipeAngle <= 135 && row < board.height - 1)
        {
            MovePiecesRework(Vector2.up);
        }
        // Left Swipe
        else if ((swipeAngle > 135 || swipeAngle <= -135) && column > 0)
        {
            MovePiecesRework(Vector2.left);
        }
        // Down Swipe
        else if ((swipeAngle < -45 && swipeAngle >= -135) && row > 0)
        {
            MovePiecesRework(Vector2.down);
        }
        else
        {
            board.currentState = GameState.move;
        }
    }

    void FindMatches()
    {
        if(column > 0 && column < board.width - 1)
        {
            GameObject leftDot1 = board.allDots[column - 1, row];
            GameObject rightDot1 = board.allDots[column + 1, row];

            if(leftDot1 != null && rightDot1 != null)
            {
                if (leftDot1.tag == this.gameObject.tag && rightDot1.tag == this.gameObject.tag)
                {
                    leftDot1.GetComponent<Dot>().isMatched = true;
                    rightDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                    moveToMatchCounter++;
                }
            }
        }

        if (row > 0 && row < board.height - 1)
        {
            GameObject upDot1 = board.allDots[column, row + 1];
            GameObject downDot1 = board.allDots[column, row - 1];

            if(upDot1 != null && downDot1 != null)
            {
                if (upDot1.tag == this.gameObject.tag && downDot1.tag == this.gameObject.tag)
                {
                    upDot1.GetComponent<Dot>().isMatched = true;
                    downDot1.GetComponent<Dot>().isMatched = true;
                    isMatched = true;
                    moveToMatchCounter++;
                }
            }
        }
    }

    public void MakeRowExplosion()
    {
        if(!isColumnBomb && !isColourBomb && !isAdjacentBomb)
        {
            isRowBomb = true;
            GameObject arrow = Instantiate(rowArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
            rowChildEffect = arrow.transform.Find("RowParticale");

            if (board.AllJuice == true || board.someJuice == true)
            {
                rowChildEffect.gameObject.SetActive(true);
            }
        }
    }

    public void MakeColumnExplosion()
    {
        if (!isRowBomb && !isColourBomb && !isAdjacentBomb)
        {
            isColumnBomb = true;
            GameObject arrow = Instantiate(columnArrow, transform.position, Quaternion.identity);
            arrow.transform.parent = this.transform;
            columnChildEffect = arrow.transform.Find("ColumnArrow/ColumnParticale");

            if (board.AllJuice == true || board.someJuice)
            {
                columnChildEffect.gameObject.SetActive(true);
            }
        }
    }

    public void MakeColourBomb()
    {
        if (!isColumnBomb && !isRowBomb && !isAdjacentBomb)
        {

            isColourBomb = true;
            GameObject colour = Instantiate(colourBomb, transform.position, Quaternion.identity);
            colour.transform.parent = this.transform;
            this.gameObject.tag = "Color";
        }
    }

    public void MakeAdjacentBomb()
    {
        if (!isColumnBomb && !isRowBomb && !isColourBomb)
        {
            isAdjacentBomb = true;
            GameObject marker = Instantiate(adjacentMarker, transform.position, Quaternion.identity);
            marker.transform.parent = this.transform;
            adjChildEffect = marker.transform.Find("AdjacentParticale");
            
            if (board.AllJuice == true || board.someJuice == true)
            {
                adjChildEffect.gameObject.SetActive(true);
            }
        }
    }
}
