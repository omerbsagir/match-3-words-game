using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text.RegularExpressions;
using static UnityEditor.PlayerSettings;
using static Gem;
using TMPro;

public class Board : MonoBehaviour
{
    public int width, height;
    public GameObject tilePrefab;

    public Gem[] gems;
    public Gem[,] allGems;

    public float gemSpeed;

    [HideInInspector]
    public MatchFinder matchFind;
    [HideInInspector]
    public LetterSelector letterSelector;
    [HideInInspector]
    public LevelManager levelManager;

    private List<string> wordDatabase = new List<string>();
    private WordDatabase wd;

    public enum BoardState { wait, move }
    public BoardState currentState = BoardState.move;

    public Gem bomb;
    public Gem verticalBomb;
    public Gem horizontalBomb;
    public Gem comboBomb;
    public float bombChance = 2f;

    public int letterCountFM;

    private float idleTime = 0f;
    public float maxIdleTime = 3f;

    private bool isDestroying=false;
    private bool isThereAnyBomb = false;

    List<Gem> movedBombs = new List<Gem>();

    public Gem glass;
    public float glassChance = 2f;
    public Gem[,] allGlasses;

    private string potentialMoveDir=null;

    public bool isHighlighting = false;

    public Gem grass;
    public Gem[,] allGrasses;

    public Gem wood;
    public Gem[,] allWoods;

    public Gem[,] layoutWoods;
    public Gem[,] layoutGlasses;
    public Gem[,] layoutGems;
    public Gem[,] layoutGrasses;
    public Gem[,] layoutHiddens;

    public bool[,] layoutIsPoisoned;

    public Gem hidden;
    public Gem[,] allHiddens;

    

    private void Awake()
    {
        wd = FindObjectOfType<WordDatabase>();
        matchFind = FindObjectOfType<MatchFinder>();
        letterSelector = FindObjectOfType<LetterSelector>();
        levelManager = FindObjectOfType<LevelManager>();

        width = levelManager.GetLevelDimensions()[0];
        height = levelManager.GetLevelDimensions()[1];

        layoutWoods = levelManager.GetLevelLayoutWood();
        layoutGlasses = levelManager.GetLevelLayoutGlass();
        layoutGems = levelManager.GetLevelLayoutGems();
        layoutGrasses = levelManager.GetLevelLayoutGrass();
        layoutHiddens = levelManager.GetLevelLayoutHidden();
        layoutIsPoisoned = levelManager.GetLevelLayoutPoison();
        
    }

    void Start()
    {
        
        wordDatabase = wd.wordList;
        letterCountFM = matchFind.letterCountForMatch;
        allGems = new Gem[width, height];
        allGlasses = new Gem[width, height];
        allGrasses = new Gem[width, height];
        allWoods = new Gem[width, height];
        allHiddens = new Gem[width, height];

        Setup();

        
    }

    private void Update()
    {
        if (UserMovedGem())
        {
            if (!isDestroying)
            {
                DeHighlightGems();
            }
            
            idleTime = 0f; // Kullanıcı bir hamle yaptıysa zamanlayıcıyı sıfırla
        }
        else
        {
            idleTime += Time.deltaTime; // Kullanıcı hareketsizse süreyi artır
            if (idleTime >= maxIdleTime)
            {
                ShowPotentialMatch(); // Belirlenen süreyi geçince potansiyel eşleşmeyi göster
                
                if (idleTime >= maxIdleTime*2)
                {
                    isHighlighting = false;
                    idleTime = 0f;
                }
                
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            ShuffleBoard();
        }

        

    }

    
    private void LayoutSetupLast(int x, int y)
    {

        Vector2Int pos = new Vector2Int(x, y);

        if (layoutGrasses[x, y] != null)
        {
            SpawnGrass(pos);

        }
        if (layoutGlasses[x, y] != null)
        {
            SpawnGlass(pos);
        }
        

    }

    private void SetupHiddens()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (layoutHiddens[x, y] != null)
                {
                    SpawnHidden(new Vector2Int(x,y));

                }
            }
        }
    }
    private void Setup()
    {
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                allGems[x, y] = null;
                allGlasses[x, y] = null;
                allGrasses[x, y] = null;
                allWoods[x, y] = null;
                allHiddens[x, y] = null;

                Vector2 pos = new Vector2(x, y);
                GameObject bgTile = Instantiate(tilePrefab, pos, Quaternion.identity);
                bgTile.transform.parent = transform;
                bgTile.name = "BG Tile - " + x + ", " + y;


                Vector2Int startPos = new Vector2Int(x, y);


                if (layoutWoods[x, y] != null)
                {
                    SpawnWood(startPos);
                }
                else
                {
                    if (layoutGems[x,y] != null)
                    {
                        
                        if (layoutGems[x, y].type == Gem.GemType.bomb)
                        {
                            SpawnGem(startPos,bomb);
                        }
                        else if (layoutGems[x, y].type == Gem.GemType.vertical)
                        {
                            SpawnGem(startPos, verticalBomb);
                        }
                        else if (layoutGems[x, y].type == Gem.GemType.horizontal)
                        {
                            SpawnGem(startPos, horizontalBomb);
                        }
                        else if (layoutGems[x, y].type == Gem.GemType.combo)
                        {
                            SpawnGem(startPos, comboBomb);
                        }
                        else
                        {
                            SpawnGem(startPos, layoutGems[x, y]);
                            
                        }
                        

                    }
                    else
                    {
                        int gemToUse = letterSelector.GetRandomLetter();

                        int iterations = 0;

                        while ((MatchesAtSame(startPos, gems[gemToUse]) || MatchesAtWord(startPos, gems[gemToUse])) && iterations < 100)
                        {

                            gemToUse = letterSelector.GetRandomLetter();
                            iterations++;

                            if (iterations == 99)
                            {
                                Debug.Log("sıçıyor");
                            }
                        }


                        SpawnGem(startPos, gems[gemToUse]);
                    }
                    
                    LayoutSetupLast(x, y);


                }

                if (layoutIsPoisoned[x, y])
                {
                    allGems[x, y].PoisonTheGem();
                }

            }
        }
        SetupHiddens();

        LevelNeedsManager.Instance.SetLevelNeeds(LevelManager.Instance.level);
        
    }
    

    void SpawnGem(Vector2Int pos,Gem gemToSpawn)
    {
        

        Gem gem = Instantiate(gemToSpawn, new Vector3(pos.x, pos.y+height, 0f), Quaternion.identity);
        gem.transform.parent = transform;
        gem.name = "Gem - " + pos.x + ", " + pos.y;
        allGems[pos.x, pos.y] = gem;

        gem.SetupGem(pos, this);


    }
    
    void SpawnGlass(Vector2Int pos)
    {
        if(allGems[pos.x, pos.y].type != Gem.GemType.bomb)
        {
            Gem gem = Instantiate(glass, new Vector3(pos.x, pos.y + height, 0f), Quaternion.identity);
            gem.transform.parent = transform;
            gem.name = "Glass - " + pos.x + ", " + pos.y;
            allGems[pos.x, pos.y].hasCover = true;
            allGlasses[pos.x, pos.y] = gem;

            gem.SetupGem(pos, this);

        }
        
    }
    void SpawnGrass(Vector2Int pos)
    {
        if (allGems[pos.x, pos.y].type != Gem.GemType.bomb && allGlasses[pos.x,pos.y]==null)
        {
            Gem gem = Instantiate(grass, new Vector3(pos.x, pos.y + height, 0f), Quaternion.identity);
            gem.transform.parent = transform;
            gem.name = "Grass - " + pos.x + ", " + pos.y;
            allGems[pos.x, pos.y].hasHidden = true;
            allGrasses[pos.x, pos.y] = gem;


            gem.SetupGem(pos, this);

        }

    }
    void SpawnHidden(Vector2Int pos)
    {

        Vector2 baseGrass = new Vector2(pos.x,pos.y);
        Vector2 upGrass = new Vector2(pos.x, pos.y+1);
        Vector2 rightGrass = new Vector2(pos.x+1, pos.y);
        Vector2 crossGrass = new Vector2(pos.x+1, pos.y+1);


        float posX = ( baseGrass.x + upGrass.x + rightGrass.x + crossGrass.x ) / 4;
        float posY = (baseGrass.y + upGrass.y + rightGrass.y + crossGrass.y) / 4;

        Vector2 realPos = new Vector2(posX,posY);

        Gem gem = Instantiate(hidden, new Vector3(posX, posY, 0f), Quaternion.identity);
        gem.transform.parent = transform;
        gem.name = "Hidden - " + pos.x + ", " + pos.y;

        
        allGrasses[(int)baseGrass.x, (int)baseGrass.y].hasHidden = true;
        allGrasses[(int)upGrass.x, (int)upGrass.y].hasHidden = true;
        allGrasses[(int)rightGrass.x, (int)rightGrass.y].hasHidden = true;
        allGrasses[(int)crossGrass.x, (int)crossGrass.y].hasHidden = true;

        allHiddens[pos.x, pos.y] = gem;

       
        gem.SetupGem(pos, this);

    }
    void SpawnWood(Vector2Int pos)
    {
        Gem gem = Instantiate(wood, new Vector3(pos.x, pos.y + height, 0f), Quaternion.identity);
        gem.transform.parent = transform;
        gem.name = "Wood - " + pos.x + ", " + pos.y;
        allWoods[pos.x, pos.y] = gem;

        gem.SetupGem(pos, this);

    }


    bool MatchesAtSame(Vector2Int posToCheck, Gem gemToCheck)
    {
        if (posToCheck.x > 0 && allGems[posToCheck.x - 1, posToCheck.y] != null)
        {
            if (allGems[posToCheck.x - 1, posToCheck.y].type == gemToCheck.type) //&& allGems[posToCheck.x - 2, posToCheck.y].type == gemToCheck.type
            {
                return true;
            }
        }

        if (posToCheck.y > 0 && allGems[posToCheck.x, posToCheck.y - 1] != null)
        {
            if (allGems[posToCheck.x, posToCheck.y - 1].type == gemToCheck.type) //&& allGems[posToCheck.x, posToCheck.y - 2].type == gemToCheck.type
            {
                return true;
            }
        }

        return false;
    }
    bool MatchesAtWord(Vector2Int startPos, Gem gemToCheck)
    {

        Vector2Int endPosH = new Vector2Int();
        Vector2Int endPosV = new Vector2Int();

        string wordH = "";
        string wordV = "";
        string wordRH = "";
        string wordRV = "";

        if (startPos.x > letterCountFM-1)
        {
            int x = startPos.x - letterCountFM;
            endPosH = new Vector2Int(x, startPos.y);

            if (endPosH != null)
            {
                wordH = GetWordFromPositionsHorizontal(startPos, endPosH,gemToCheck);
 
                wordRH = new string(wordH.Reverse().ToArray());
            }
        }

        if (startPos.y > letterCountFM - 1)
        {
            int y = startPos.y - letterCountFM;
            endPosV = new Vector2Int(startPos.x, y);

            if (endPosH != null)
            {
                wordV = GetWordFromPositionsVertical(startPos, endPosV,gemToCheck);

                wordRV = new string(wordV.Reverse().ToArray());
            }
        }



        if (IsValidWord(wordH) || IsValidWord(wordV) || IsValidWord(wordRH) || IsValidWord(wordRV))
        {
            return true;
        }
        return false;
    }

    public bool IsValidWord(string word)
    {
        return wordDatabase.Contains(word.ToLower()); // Küçük harfe çevirip kontrol et
    }

    string GetWordFromPositionsHorizontal(Vector2Int startPos, Vector2Int endPos, Gem gemToCheck)
    {
        string word = "";
        word += gemToCheck.letterValue;

        // Yatay bir kelime oluşturma
        for (int x = startPos.x-1; x >= endPos.x; x--)
        {
            if (allWoods[x,startPos.y] == null && allGems[x, startPos.y].type != GemType.prized)
            {
                word += allGems[x, startPos.y].letterValue;
            }
            
        }

        return word;
    }
    string GetWordFromPositionsVertical(Vector2Int startPos, Vector2Int endPos, Gem gemToCheck)
    {
        string word = "";
        word += gemToCheck.letterValue;

        for (int y = startPos.y-1; y >= endPos.y; y--)
        {

            if (allWoods[startPos.x, y] == null && allGems[startPos.x, y].type != GemType.prized)
            {
                
                word += allGems[startPos.x, y].letterValue;
            }

        }

        return word;
    }

    public void DestroyMatches()
    {
        StartCoroutine(HandleMatchesCoroutine());
    }

    private IEnumerator HandleMatchesCoroutine()
    {
        isDestroying = true;


        for (int i = 0; i < matchFind.currentMatches.Count; i++)
        {
            if (matchFind.currentMatches[i] != null && matchFind.currentMatches[i].type != Gem.GemType.wood && matchFind.currentMatches[i].type != Gem.GemType.hidden && matchFind.currentMatches[i].type != Gem.GemType.grass && matchFind.currentMatches[i].type != Gem.GemType.glass)
            {
                allGems[(int)matchFind.currentMatches[i].posIndex.x, (int)matchFind.currentMatches[i].posIndex.y].GetComponent<SpriteRenderer>().color = Color.green;
            }
        }


        yield return new WaitForSeconds(2f);


        for (int i = 0; i < matchFind.currentMatches.Count; i++)
        {
            if (matchFind.currentMatches[i] != null)
            {
                DestroyMatchedLetterAt(matchFind.currentMatches[i].posIndex);
            }
        }

        isDestroying = false;

        
        StartCoroutine(DecreaseRowCo());
    }

    private void DestroyMatchedLetterAt(Vector2 pos)
    {

        if (allGems[(int)pos.x, (int)pos.y] != null && (allGems[(int)pos.x, (int)pos.y].type != GemType.prized))
        {
            if (allGems[(int)pos.x, (int)pos.y].isMatched)
            {

                if (allGlasses[(int)pos.x, (int)pos.y] != null)
                {
                    Destroy(allGlasses[(int)pos.x, (int)pos.y].gameObject);
                    allGlasses[(int)pos.x, (int)pos.y] = null;
                    LevelNeedsManager.Instance.remainingGlassCount--;

                }
               

                Destroy(allGems[(int)pos.x, (int)pos.y].gameObject);
                allGems[(int)pos.x, (int)pos.y] = null;
                LevelNeedsManager.Instance.remainingLetterCount--;

            }
            

        }
        else if (allWoods[(int)pos.x, (int)pos.y] != null)
        {
            Destroy(allWoods[(int)pos.x, (int)pos.y].gameObject);
            allWoods[(int)pos.x, (int)pos.y] = null;
            LevelNeedsManager.Instance.remainingWoodCount--;
        }
        if (allGrasses[(int)pos.x, (int)pos.y] != null)
        {
            if (allGrasses[(int)pos.x, (int)pos.y].isMatched)
            {
                Destroy(allGrasses[(int)pos.x, (int)pos.y].gameObject);
                allGrasses[(int)pos.x, (int)pos.y] = null;
                LevelNeedsManager.Instance.remainingGrassCount--;

            }

        }
        
        if (allHiddens[(int)pos.x, (int)pos.y] != null)
        {
            if (CheckHiddenNear((int)pos.x, (int)pos.y))
            {
                Destroy(allHiddens[(int)pos.x, (int)pos.y].gameObject);
                allHiddens[(int)pos.x, (int)pos.y] = null;
                LevelNeedsManager.Instance.remainingHiddenCount--;
            }

        }
  

    }

    



    // Yeni harflerin yer değiştirmesi
    private IEnumerator DecreaseRowCo()
    {
        yield return new WaitForSeconds(.2f);

        int nullCounter = 0;
        

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null && allWoods[x,y] == null)
                {
                    nullCounter++;
                }
                else
                {

                    if (nullCounter > 0)
                    {

                        if(allGlasses[x, y] != null || allWoods[x,y] != null)
                        {
                            nullCounter = 0;
                        }

                        else
                        {
                            if (allGems[x, y].type == Gem.GemType.bomb)
                            {
                                movedBombs.Add(allGems[x, y]);

                            }

                            if (allGrasses[x, y - nullCounter] != null)
                            {
                                //allGems[x, y].hasHidden = true;
                                matchFind.currentMatches.Add(allGrasses[x, y - nullCounter]);
                            }
                            else
                            {
                                allGems[x, y].hasHidden = false;
                            }

                            allGems[x, y].posIndex.y -= nullCounter;
                            allGems[x, y - nullCounter] = allGems[x, y];
                            allGems[x, y] = null;

                            
                        }

                        

                    }



                }
            }
            nullCounter = 0;
        }
        
        StartCoroutine(FillBoardCo());
    }

    // Tahtayı doldur
    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(.25f);
        RefillBoard();
        yield return new WaitForSeconds(.25f);
        
        matchFind.FindAllMatches();
        if (movedBombs.Count > 0)
        {
            foreach (Gem g in movedBombs)
            {
                g.MarkBeforeExplodeBomb(g);
            }
            movedBombs.Clear();
        }
        if (matchFind.currentMatches.Count > 0)
        {
            yield return new WaitForSeconds(.25f);
            DestroyMatches();
        }
        else
        {
            yield return new WaitForSeconds(.5f);
            currentState = BoardState.move;
        }
    }

    private void RefillBoard()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] == null && allWoods[x,y] == null)
                {

                    Vector2Int startPos = new Vector2Int(x, y);

                    if (Random.Range(0f, 100f) < bombChance)
                    {
                        SpawnGem(startPos,bomb);
                    }
                    else
                    {
                        int letterToUse = letterSelector.GetRandomLetter();

                        int iterations = 0;

                        while ((MatchesAtSame(startPos, gems[letterToUse])) && iterations < 20)
                        {
                            letterToUse = letterSelector.GetRandomLetter();
                            iterations++;

                            if (iterations == 19)
                            {
                                Debug.Log("sıçıyor refill");
                            }
                        }

                        SpawnGem(startPos, gems[letterToUse]);
                    }
                    
                }
            }
        }
       

        
    }


    private bool UserMovedGem()
    {
        if (currentState == BoardState.wait)
        {
            return true;
        }
        else
        {
            return false;
        }

    }


    private void ShowPotentialMatch()
    {
        List<Gem> potGems = FindPotentialMatches();
        if (potGems != null)
        { 
            HighlightGems(potGems,potentialMoveDir);
            
        }
        else
        {
            CheckBomb();
            if (!isThereAnyBomb)
            {
                Debug.Log("Hamle Kalmadı Tahta Shuffle Ediliyor");
                ShuffleBoard();
            }
            else
            {
                Debug.Log("Hamle Kalmadı Ama Bomba Var");
            }
            

            isThereAnyBomb = false;
        }
        
    }

    private List<Gem> FindPotentialMatches()
    {

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Gem currentGem = allGems[x, y];
                if (currentGem != null && currentGem.type!=Gem.GemType.bomb && currentGem.type != Gem.GemType.vertical && currentGem.type != Gem.GemType.horizontal && currentGem.type != Gem.GemType.combo && allGlasses[x,y]==null)
                {
                    
                    if (CanSwapAndMatch(x, y, x + 1, y) != null && allGlasses[x + 1, y] == null && allWoods[x + 1, y] == null) 
                    {
                        potentialMoveDir = "right";
                        return returnGemsAccDir(CanSwapAndMatch(x, y, x + 1, y), x + 1, y, allGems[x,y]);
                        
                    }
                    else if (CanSwapAndMatch(x, y, x, y + 1) != null && allGlasses[x, y + 1] == null && allWoods[x, y+1] == null) 
                    {
                        potentialMoveDir = "above";
                        return returnGemsAccDir(CanSwapAndMatch(x, y, x, y+1), x, y+1, allGems[x, y]);
                        
                    }
                    else if (CanSwapAndMatch(x, y, x-1, y) != null && allGlasses[x - 1, y] == null && allWoods[x - 1, y] == null)
                    {
                        potentialMoveDir = "left";
                        return returnGemsAccDir(CanSwapAndMatch(x, y, x - 1, y), x -1, y, allGems[x, y]);
                        
                    }
                    else if (CanSwapAndMatch(x, y, x, y-1) != null && allGlasses[x, y - 1] == null && allWoods[x, y-1] == null) 
                    {
                        potentialMoveDir = "under";
                        return returnGemsAccDir(CanSwapAndMatch(x, y, x, y-1), x, y-1, allGems[x, y]);
                        
                    }
                }
            }
        }

        potentialMoveDir = null;

        return null;
    }

    private List<Gem> returnGemsAccDir(string direction,int x2,int y2,Gem gemToAdd)
    {
        List<Gem> returnGems = new List<Gem>();
        switch (direction)
        {
            case "left":
                returnGems = matchFind.GetHorizontalGemsLeft(x2, y2);
                returnGems.RemoveAt(0);
                returnGems.Add(gemToAdd);
                return returnGems;
       
            case "right":
                returnGems = matchFind.GetHorizontalGemsRight(x2, y2);
                returnGems.RemoveAt(0);
                returnGems.Add(gemToAdd);
                return returnGems;

            case "above":
                returnGems = matchFind.GetVerticalGemsAbove(x2, y2);
                returnGems.RemoveAt(0);
                returnGems.Add(gemToAdd);
                return returnGems;

            case "under":
                returnGems = matchFind.GetVerticalGemsUnder(x2, y2);
                returnGems.RemoveAt(0);
                returnGems.Add(gemToAdd);
                return returnGems;

            default:
                return null;
        }
    }
    private string CanSwapAndMatch(int x1, int y1, int x2, int y2) // 0 0 -> 1 0
    {
        if (x2 >= width || y2 >= height || x2 < 0 || y2 < 0) return null;

        Gem temp = allGems[x1, y1];
        allGems[x1, y1] = allGems[x2, y2];
        allGems[x2, y2] = temp;

        List<Gem> potLeftMatch = matchFind.GetHorizontalGemsLeft(x2, y2); //null
        List<Gem> potRightMatch = matchFind.GetHorizontalGemsRight(x2, y2); // 1 2 3 4
        List<Gem> potAboveMatch = matchFind.GetVerticalGemsAbove(x2, y2); // 1 2 3 4
        List<Gem> potUnderMatch = matchFind.GetVerticalGemsUnder(x2, y2); //

        string wordL = matchFind.GetWordFromGems(potLeftMatch);
        string wordR = matchFind.GetWordFromGems(potRightMatch);
        string wordA = matchFind.GetWordFromGems(potAboveMatch);
        string wordU = matchFind.GetWordFromGems(potUnderMatch);

        string rWordL = new string(wordL.Reverse().ToArray());
        string rWordR = new string(wordR.Reverse().ToArray());
        string rWordA = new string(wordA.Reverse().ToArray());
        string rWordU = new string(wordU.Reverse().ToArray());



        // Taşları eski yerlerine geri koy
        temp = allGems[x1, y1];
        allGems[x1, y1] = allGems[x2, y2];
        allGems[x2, y2] = temp;


        if (IsValidWord(wordL) || IsValidWord(rWordL))
        {
            return "left";
        }
        else if (IsValidWord(wordR) || IsValidWord(rWordR))
        {

            return "right";
        }
        else if (IsValidWord(wordA) || IsValidWord(rWordA))
        {

            return "above";
        }
        else if (IsValidWord(wordU) || IsValidWord(rWordU))
        {

            return "under";
        }
        else
        {
            return null;
        }

    }

    void HighlightGems(List<Gem> gems, string potMoveDirection)
    {

        SetupGemsPosition();

        Gem firstGem = gems[gems.Count - 1];
        float x1 = firstGem.posIndex.x;
        float y1 = firstGem.posIndex.y;
        float x2 = firstGem.posIndex.x;
        float y2 = firstGem.posIndex.y;

        switch (potMoveDirection)
        {
            case "right":
                x2 += 0.12f;
                break;
            case "left":
                x2 -= 0.12f;
                break;
            case "above":
                y2 += 0.12f;
                break;
            case "under":
                y2 -= 0.12f;
                break;
            default:
                break;
        }

        // PingPong fonksiyonunu kullanarak gemi iki nokta arasında yumuşak bir şekilde hareket ettir
        float t = Mathf.PingPong(Time.time * 2.0f, 1.0f); // Zamanla iki nokta arasında gidip gelen bir t değeri üretir

        isHighlighting = true;

        firstGem.transform.position = Vector2.Lerp(new Vector2(x1, y1), new Vector2(x2, y2), t);



    }


    void DeHighlightGems()
    {
        isHighlighting = false;

    }

    public void ShuffleBoard()
    {
        if (currentState != BoardState.wait)
        {
            currentState = BoardState.wait;

            List<Gem> gemsFromBoard = new List<Gem>();

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (allGems[x,y]!=null && allGlasses[x, y] == null && allGrasses[x, y] == null && allGems[x,y].type != GemType.prized)
                    {
                        gemsFromBoard.Add(allGems[x, y]);
                        allGems[x, y] = null;
                    }
                    
                }
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (allGems[x, y] == null)
                    {
                        int gemToUse = Random.Range(0, gemsFromBoard.Count);

                        int iterations = 0;
                        while (MatchesAtSame(new Vector2Int(x, y), gemsFromBoard[gemToUse]) && iterations < 100 && gemsFromBoard.Count > 1)
                        {
                            gemToUse = Random.Range(0, gemsFromBoard.Count);
                            iterations++;
                        }

                        gemsFromBoard[gemToUse].SetupGem(new Vector2Int(x, y), this);
                        allGems[x, y] = gemsFromBoard[gemToUse];
                        gemsFromBoard.RemoveAt(gemToUse);
                    }

                }
            }

            StartCoroutine(FillBoardCo());
        }
    }

    private void CheckBomb()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] != null && ( allGems[x,y].type == Gem.GemType.bomb || allGems[x, y].type == GemType.vertical || allGems[x, y].type == GemType.horizontal || allGems[x, y].type == GemType.combo))
                {
                    isThereAnyBomb = true;
                }
            }
        }
    }

    private void SetupGemsPosition()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x,y] != null)
                {
                    allGems[x, y].SetupGem(new Vector2Int(x, y), this);
                }
            }
        }
    }

    private bool CheckHiddenNear(int x , int y)
    {

        List<Vector2Int> grasses = new List<Vector2Int>();

        grasses.Add(new Vector2Int(x, y));
        grasses.Add(new Vector2Int(x, y + 1));
        grasses.Add(new Vector2Int(x + 1, y));
        grasses.Add(new Vector2Int(x + 1, y + 1));

        for (int i = 0; i < 4; i++)
        {
            if (allGrasses[grasses[i].x, grasses[i].y] != null && !allGrasses[grasses[i].x, grasses[i].y].isMatched)
            {
                return false;
            }
           
        }
        return true;
    }

    public void CheckThePoison()
    {
        List<Gem> poisonedGems = new List<Gem>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (allGems[x, y] != null && allGems[x,y].isPoisoned)
                {
                    poisonedGems.Add(allGems[x, y]);
                }
            }
        }
        foreach(Gem g in poisonedGems)
        {
            int x = (int)g.posIndex.x;
            int y = (int)g.posIndex.y;

            if (x+1<width && allGems[x + 1, y]!=null && !allGems[x + 1, y].hasCover && !allGems[x + 1, y].hasHidden)
            {
                allGems[x + 1, y].PoisonTheGem();
            }
            else if(x - 1 >= 0 && allGems[x-1, y] != null && !allGems[x - 1, y].hasCover && !allGems[x - 1, y].hasHidden)
            {
                allGems[x - 1, y].PoisonTheGem();
            }
            else if (y + 1 < height && allGems[x, y+1] != null && !allGems[x, y+1].hasCover && !allGems[x , y+1].hasHidden)
            {
                allGems[x, y+1].PoisonTheGem();
            }
            else if (y- 1 >= 0 && allGems[x , y-1] != null && !allGems[x, y-1].hasCover && !allGems[x, y-1].hasHidden)
            {
                allGems[x, y-1].PoisonTheGem();
            }
            else
            {
                Debug.Log("Zehirlenmeye uygun gem yok");
            }

        }
    }
}
