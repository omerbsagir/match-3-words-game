using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{

    //[HideInInspector]
    public Vector2Int posIndex;
    [HideInInspector] public Board board;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;

    private bool mousePressed;
    private float swipeAngle = 0;

    private Gem otherGem;

    public enum GemType { a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z }
    public GemType type;
    public string letterValue;

    public bool isMatched;

    private Vector2Int previousPos;

    public GameObject destroyEffect;
    

    void Start()
    {
        
    }

    
    void Update()
    {


        if (Vector2.Distance(transform.position, posIndex) > .01f)
        {
            transform.position = Vector2.Lerp(transform.position, posIndex, board.gemSpeed * Time.deltaTime);
        }
        else
        {
            transform.position = new Vector3(posIndex.x, posIndex.y, 0f);
            board.allGems[posIndex.x, posIndex.y] = this;
        }

        if (mousePressed && Input.GetMouseButtonUp(0))

        {
            mousePressed = false;

            if (board.currentState == Board.BoardState.move) // && board.roundMan.roundTime > 0
            {
                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                CalculateAngle();
            }
        }
    }

    public void SetupGem(Vector2Int pos , Board theBoard)
    {
        posIndex = pos;
        board = theBoard;
    }

    private void CalculateAngle()
    {
        swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x);
        swipeAngle = swipeAngle * 180 / Mathf.PI;

        if (Vector3.Distance(firstTouchPosition, finalTouchPosition) > .5f)
        {
            MovePieces();
        }
    }

    private void MovePieces()
    {
        previousPos = posIndex;

        if (swipeAngle < 45 && swipeAngle > -45 && posIndex.x < board.width - 1)
        {
            otherGem = board.allGems[posIndex.x + 1, posIndex.y];
            otherGem.posIndex.x--;
            posIndex.x++;
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && posIndex.y < board.height - 1)
        {
            otherGem = board.allGems[posIndex.x, posIndex.y + 1];
            otherGem.posIndex.y--;
            posIndex.y++;
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && posIndex.y > 0)
        {
            otherGem = board.allGems[posIndex.x, posIndex.y - 1];
            otherGem.posIndex.y++;
            posIndex.y--;
        }
        else if (swipeAngle > 135 || swipeAngle < -135)
        {
            if (posIndex.x > 0) {
                otherGem = board.allGems[posIndex.x - 1, posIndex.y];
                otherGem.posIndex.x++;
                posIndex.x--;
            }
            
        }

        board.allGems[posIndex.x, posIndex.y] = this;
        if (otherGem != null) {
            board.allGems[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;
        }
        

        StartCoroutine(CheckMoveCo());
    }

    private void OnMouseDown()
    {
        if (board.currentState == Board.BoardState.move) //&& board.roundMan.roundTime > 0
        {
            firstTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePressed = true;
        }
    }

    public IEnumerator CheckMoveCo()
    {
        board.currentState = Board.BoardState.wait;

        yield return new WaitForSeconds(.25f);

        board.matchFind.FindAllMatches();

        if (otherGem != null)
        {
            if (!isMatched && !otherGem.isMatched)
            {
                otherGem.posIndex = posIndex;
                posIndex = previousPos;

                board.allGems[posIndex.x, posIndex.y] = this;
                board.allGems[otherGem.posIndex.x, otherGem.posIndex.y] = otherGem;

                yield return new WaitForSeconds(.25f);

                board.currentState = Board.BoardState.move;
            }
            board.DestroyMatches();
        }
        else
        {
            board.currentState = Board.BoardState.move;
        }
    }
}
