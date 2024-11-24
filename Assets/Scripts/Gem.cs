using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : MonoBehaviour
{

    //[HideInInspector]
    public Vector2 posIndex;
    [HideInInspector] public Board board;

    private Vector2 firstTouchPosition;
    private Vector2 finalTouchPosition;

    private bool mousePressed;
    private float swipeAngle = 0;

    private Gem otherGem;

    public enum GemType { a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,bomb,glass,grass,wood , hidden,horizontal,vertical,combo,prized}
    public GemType type;
    public string letterValue;

    public bool isMatched;

    private Vector2 previousPos;

    public GameObject destroyEffect;

    public int blastSize = 2;

    public bool hasCover=false;

    public bool hasHidden;

    


    private void Awake()
    {
        if (type != GemType.bomb)
        {
            blastSize = 0;
        }
        hasHidden = false;

    }
    void Start()
    {
        
    }

    
    void Update()
    {


        if (Vector2.Distance(transform.position, posIndex) > .01f && !board.isHighlighting)
        {
            if (type != GemType.hidden)
            {
                transform.position = Vector2.Lerp(transform.position, posIndex, board.gemSpeed * Time.deltaTime);

            }
            
        }
        else
        {

            if (!board.isHighlighting)
            {
                if (type != GemType.hidden)
                {
                    
                    transform.position = new Vector3(posIndex.x, posIndex.y, 0f);

                }
                
                
            }


            if (type == GemType.glass)
            {
                board.allGlasses[(int)posIndex.x, (int)posIndex.y] = this;
            }
            else if (type == GemType.grass)
            {
                board.allGrasses[(int)posIndex.x, (int)posIndex.y] = this;
            }
            else if (type == GemType.wood)
            {
                board.allWoods[(int)posIndex.x, (int)posIndex.y] = this;
            }
            else if (type == GemType.hidden)
            {
                board.allHiddens[(int)posIndex.x, (int)posIndex.y] = this;
            }
            else
            {
                board.allGems[(int)posIndex.x, (int)posIndex.y] = this;
            }

        }
        

        if (mousePressed && Input.GetMouseButtonUp(0))
        {
            mousePressed = false;

            if (board.currentState == Board.BoardState.move ) // && board.roundMan.roundTime > 0
            {
                

                finalTouchPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (Vector2.Distance(firstTouchPosition, finalTouchPosition) < 0.01f && ( type == GemType.bomb || type == GemType.vertical || type == GemType.horizontal || type == GemType.combo))
                {
                    LevelNeedsManager.Instance.remainingMoveCount--;
                    StartCoroutine(CheckMoveCo());
                }
                else
                {
                    CalculateAngle();
                }
                
            }
        }
    }

    public void SetupGem(Vector2 pos , Board theBoard)
    {
        posIndex = pos;
        board = theBoard;
    }

    private void CalculateAngle()
    {
        if (type != GemType.glass && hasCover==false && type!=GemType.grass && type!= GemType.wood && type != GemType.hidden && type != GemType.prized)
        {
            swipeAngle = Mathf.Atan2(finalTouchPosition.y - firstTouchPosition.y, finalTouchPosition.x - firstTouchPosition.x);
            swipeAngle = swipeAngle * 180 / Mathf.PI;

            if (Vector3.Distance(firstTouchPosition, finalTouchPosition) > .5f)
            {
                LevelNeedsManager.Instance.remainingMoveCount--;
                MovePieces();
            }
        }
        
    }

    private bool isOtherGemFixed(float x, float y)
    {
        if (board.allWoods[(int)x,(int)y] != null || board.allGems[(int)x, (int)y].type == GemType.prized)
        {
            return true;
        }
        return false;
    }
    private void MovePieces()
    {
        previousPos = posIndex;

        if (swipeAngle < 45 && swipeAngle > -45 && posIndex.x < board.width - 1)
        {
            if (board.allGlasses[(int)posIndex.x + 1 , (int)posIndex.y] == null && !isOtherGemFixed(posIndex.x + 1, posIndex.y)) {

                otherGem = board.allGems[(int)posIndex.x + 1, (int)posIndex.y];
                otherGem.posIndex.x--;
                posIndex.x++;
            }
            
            
        }
        else if (swipeAngle > 45 && swipeAngle <= 135 && posIndex.y < board.height - 1)
        {
            if (board.allGlasses[(int)posIndex.x, (int)posIndex.y+1] == null && !isOtherGemFixed(posIndex.x, posIndex.y+1))
            {
                otherGem = board.allGems[(int)posIndex.x, (int)posIndex.y + 1];
                otherGem.posIndex.y--;
                posIndex.y++;
            }
            
        }
        else if (swipeAngle < -45 && swipeAngle >= -135 && posIndex.y > 0)
        {
            if (board.allGlasses[(int)posIndex.x, (int)posIndex.y-1] == null && !isOtherGemFixed(posIndex.x, posIndex.y-1))
            {
                otherGem = board.allGems[(int)posIndex.x, (int)posIndex.y - 1];
                otherGem.posIndex.y++;
                posIndex.y--;
            }
            
        }
        else if (swipeAngle > 135 || swipeAngle < -135)
        {
            if (posIndex.x > 0) {
                if (board.allGlasses[(int)posIndex.x - 1, (int)posIndex.y] == null && !isOtherGemFixed(posIndex.x - 1, posIndex.y))
                {
                    otherGem = board.allGems[(int)posIndex.x - 1, (int)posIndex.y];
                    otherGem.posIndex.x++;
                    posIndex.x--;
                }
                
            }
            
        }

        board.allGems[(int)posIndex.x, (int)posIndex.y] = this;
        if (otherGem != null) {
            board.allGems[(int)otherGem.posIndex.x, (int)otherGem.posIndex.y] = otherGem;

            
            StartCoroutine(CheckMoveCo());
        }
        else
        {
            Debug.Log("othergem is null");
        }
        

        
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

        if (type != GemType.bomb && type != GemType.vertical && type != GemType.horizontal && type != GemType.combo)
        {

            board.matchFind.FindAllMatches();

            if (otherGem != null)
            {
                if (!isMatched && !otherGem.isMatched)
                {
                    otherGem.posIndex = posIndex;
                    posIndex = previousPos;

                    board.allGems[(int)posIndex.x, (int)posIndex.y] = this;
                    board.allGems[(int)otherGem.posIndex.x, (int)otherGem.posIndex.y] = otherGem;

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
        else if(type == GemType.bomb)
        {
            MarkBeforeExplodeBomb(this);
            board.DestroyMatches();
        }
        else if (type == GemType.vertical)
        {
            MarkVerticalBomb(this);
            board.DestroyMatches();
        }
        else if (type == GemType.horizontal)
        {
            MarkHorizontalBomb(this);
            board.DestroyMatches();
        }
        else if (type == GemType.combo)
        {
            MarkComboBomb(this);
            board.DestroyMatches();
        }
    }
    public void MarkVerticalBomb(Gem verticalBomb)
    {
        List<Gem> willExplode = new List<Gem>();
        int x = (int)this.posIndex.x;

        for(int i = 0; i < board.height; i++)
        {
            if(board.allGems[x, i] != null)
            {
                if(board.allGems[x, i].type == GemType.prized)
                {
                    TriggerPrize(board.allGems[x, i]);
                }
                else
                {
                    if (board.allGems[x, i].type == GemType.bomb)
                    {
                        MarkBeforeExplodeBomb((board.allGems[x, i]));
                    }
                    else if (board.allGems[x, i].type == GemType.horizontal)
                    {
                        MarkHorizontalBomb((board.allGems[x, i]));
                    }
                    else if (board.allGems[x, i].type == GemType.combo)
                    {
                        MarkHorizontalBomb((board.allGems[x, i]));
                    }
                    willExplode.Add(board.allGems[x, i]);
                }
                
            }
            else if((board.allWoods[x, i] != null)){
                willExplode.Add(board.allWoods[x, i]);
            }

            
        }
        board.matchFind.MarkGemsAsMatched(willExplode);
        board.matchFind.CheckForGlasses();
        board.matchFind.CheckForWoods();
        board.matchFind.CheckForGrasses();
        board.matchFind.CheckHiddens();
        board.matchFind.CheckPrized();
    }

    public void MarkHorizontalBomb(Gem horizontalBomb)
    {
        List<Gem> willExplode = new List<Gem>();
        int y = (int)this.posIndex.y;

        for (int i = 0; i < board.width; i++)
        {
            if((board.allGems[i, y] != null)){

                if (board.allGems[i, y].type == GemType.prized)
                {
                    TriggerPrize(board.allGems[i, y]);
                }
                else
                {
                    if (board.allGems[i, y].type == GemType.bomb)
                    {
                        MarkBeforeExplodeBomb((board.allGems[i, y]));
                    }
                    else if (board.allGems[i, y].type == GemType.vertical)
                    {
                        MarkVerticalBomb((board.allGems[i, y]));
                    }
                    else if (board.allGems[i, y].type == GemType.combo)
                    {
                        MarkVerticalBomb((board.allGems[i, y]));
                    }
                    willExplode.Add(board.allGems[i, y]);
                }
                
            }
            else if (board.allWoods[i, y] != null)
            {
                willExplode.Add(board.allWoods[i, y]);
            }
        }
        board.matchFind.MarkGemsAsMatched(willExplode);
        board.matchFind.CheckForGlasses();
        board.matchFind.CheckForWoods();
        board.matchFind.CheckForGrasses();
        board.matchFind.CheckHiddens();
        board.matchFind.CheckPrized();
    }

    public void MarkComboBomb(Gem comboBomb)
    {
        List<Gem> willExplode = new List<Gem>();
        int x = (int)this.posIndex.x;
        int y = (int)this.posIndex.y;

        for (int i = 0; i < board.height; i++)
        {
            if(board.allGems[x, i] != null)
            {
                if (board.allGems[x, i].type == GemType.prized)
                {
                    TriggerPrize(board.allGems[x, i]);
                }
                else
                {
                    if (board.allGems[x, i].type == GemType.bomb)
                    {
                        MarkBeforeExplodeBomb((board.allGems[x, i]));
                    }
                    else if (board.allGems[x, i].type == GemType.horizontal)
                    {
                        MarkHorizontalBomb((board.allGems[x, i]));
                    }
                   
                    willExplode.Add(board.allGems[x, i]);
                }
            }
            else if(board.allWoods[x, i] != null){

                willExplode.Add(board.allWoods[x, i]);
            }
            
        }

        for (int i = 0; i < board.width; i++)
        {
            if (i != x)
            {
                if(board.allGems[i, y] != null)
                {
                    if (board.allGems[i, y].type == GemType.prized)
                    {
                        TriggerPrize(board.allGems[i, y]);
                    }
                    else
                    {
                        if (board.allGems[i, y].type == GemType.bomb)
                        {
                            MarkBeforeExplodeBomb((board.allGems[i, y]));
                        }
                        else if (board.allGems[i, y].type == GemType.vertical)
                        {
                            MarkVerticalBomb((board.allGems[i, y]));
                        }
                        
                        willExplode.Add(board.allGems[i, y]);
                    }
                }
                else if(board.allWoods[i, y] != null)
                {
                    willExplode.Add(board.allWoods[i, y]);
                }
                

            }
            
        }
        board.matchFind.MarkGemsAsMatched(willExplode);
        board.matchFind.CheckForGlasses();
        board.matchFind.CheckForWoods();
        board.matchFind.CheckForGrasses();
        board.matchFind.CheckHiddens();
        board.matchFind.CheckPrized();
    }

    public void MarkBeforeExplodeBomb(Gem bomb)
    {
        List<Gem> gems = new List<Gem>();
        gems.Add(bomb);
        HashSet<Gem> processedBombs = new HashSet<Gem>(); // İşlenmiş bombaları kontrol etmek için
        Queue<Gem> bombQueue = new Queue<Gem>(); // Bombaların tetiklenmesi için bir kuyruk

        bombQueue.Enqueue(this); // İlk bombayı kuyruğa ekle
        processedBombs.Add(this); // İşaretle

        this.isMatched = true;

        while (bombQueue.Count > 0)
        {
            Gem currentBomb = bombQueue.Dequeue();
            int x = (int)currentBomb.posIndex.x;
            int y = (int)currentBomb.posIndex.y;

            // Çevresindeki taşları ekle
            AddNeighboringGems(gems, x, y);

            // Eğer çevresindeki bir taş bomba ise, onu da tetiklenecekler arasına ekle
            foreach (Gem gem in gems)
            {
                if (gem.type == GemType.bomb && !processedBombs.Contains(gem))
                {
                    bombQueue.Enqueue(gem);
                    processedBombs.Add(gem);
                }
            }
        }

        // Çevredeki tüm bombalar ve taşlar eşleşmiş olarak işaretlendi
        board.matchFind.MarkGemsAsMatched(gems);
        board.matchFind.CheckForGlasses();
        board.matchFind.CheckForWoods();
        board.matchFind.CheckForGrasses();
        board.matchFind.CheckHiddens();
        board.matchFind.CheckPrized();

    }
    // Çevredeki taşları ekleyip listeye dahil eden yardımcı fonksiyon
    private void AddNeighboringGems(List<Gem> gems, int x, int y)
    {

        // bir üstündeki
        if (y < board.height - 1)
        {
            gems.Add(AddGemOrWood(x,y+1));
        }

        //bir altındaki
        if (y > 0)
        {
            gems.Add(AddGemOrWood(x, y - 1));
        }

        //solundaki ve solundakinin üstü ve altı
        if (x > 0)
        {
            gems.Add(AddGemOrWood(x-1, y));
            if (y < board.height - 1)
            {
                gems.Add(AddGemOrWood(x-1, y + 1));
            }
            if (y > 0)
            {
                gems.Add(AddGemOrWood(x-1, y - 1));
            }
        }

        //sağındaki ve sağındakinin üstü ve altı
        if (x < board.width - 1)
        {
            gems.Add(AddGemOrWood(x+1, y));
            if (y < board.height - 1)
            {
                gems.Add(AddGemOrWood(x+1, y + 1));
            }
            if (y > 0)
            {
                gems.Add(AddGemOrWood(x+1, y - 1));
            }
        }

        //2 üstü
        if(y+1 < board.height - 1)
        {
            gems.Add(AddGemOrWood(x, y + 2));
        }

        //2 altı
        if (y-1 > 0)
        {
            gems.Add(AddGemOrWood(x, y -2));
        }

        //2 sağı
        if (x + 1 < board.height - 1)
        {
            gems.Add(AddGemOrWood(x+2, y));
        }

        //2 solu
        if (x - 1 > 0)
        {
            gems.Add(AddGemOrWood(x-2, y));
        }
    }

    private Gem AddGemOrWood(int x , int y)
    {
        if (board.allWoods[x,y] != null)
        {
            return board.allWoods[x, y];
        }
        else if(board.allGems[x, y] != null)
        {
            if(board.allGems[x, y].type == GemType.prized)
            {
                TriggerPrize(board.allGems[x, y]);
                return null;
            }
            else
            {
                return board.allGems[x, y];
            }
            
        }
        return null;
    }

    public void TriggerPrize(Gem gem)
    {
        //effect
        LevelNeedsManager.Instance.remainingPrizedCount--;
    }
}
