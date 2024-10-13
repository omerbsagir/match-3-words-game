using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    private List<string> wordDatabase = new List<string>();
    private WordDatabase wd;

    public enum BoardState { wait, move }
    public BoardState currentState = BoardState.move;

    private int letterCountFM;






    private void Awake()
    {
        wd = FindObjectOfType<WordDatabase>();
        matchFind = FindObjectOfType<MatchFinder>();
        letterSelector = FindObjectOfType<LetterSelector>();
        
    }

    void Start()
    {
        wordDatabase = wd.wordList;
        letterCountFM = matchFind.letterCountForMatch;
        allGems = new Gem[width, height];
        Setup();    
    }

    private void Setup()
    {
        for(int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector2 pos = new Vector2(x, y);
                GameObject bgTile = Instantiate(tilePrefab, pos, Quaternion.identity);
                bgTile.transform.parent = transform;
                bgTile.name = "BG Tile - " + x + ", " + y;

                int gemToUse = letterSelector.GetRandomLetter();

                int iterations = 0;
                Vector2Int startPos = new Vector2Int(x, y);
                while ((MatchesAtSame(startPos, gems[gemToUse]) || MatchesAtWord(startPos, gems[gemToUse]) ) && iterations < 100)
                {
                    
                    gemToUse = letterSelector.GetRandomLetter();
                    iterations++;

                    if (iterations == 99)
                    {
                        Debug.Log("sıçıyor");
                    }
                }
                
                SpawnGem(new Vector2Int(x,y),gems[gemToUse]);
               
            }
        }
    }

    void SpawnGem(Vector2Int pos,Gem gemToSpawn)
    {
        Gem gem = Instantiate(gemToSpawn, new Vector3(pos.x, pos.y+height, 0f), Quaternion.identity);
        gem.transform.parent = transform;
        gem.name = "Gem - " + pos.x + ", " + pos.y;
        allGems[pos.x, pos.y] = gem;

        gem.SetupGem(pos, this);
    }

    bool MatchesAtSame(Vector2Int posToCheck, Gem gemToCheck)
    {
        if (posToCheck.x > 0)
        {
            if (allGems[posToCheck.x - 1, posToCheck.y].type == gemToCheck.type) //&& allGems[posToCheck.x - 2, posToCheck.y].type == gemToCheck.type
            {
                return true;
            }
        }

        if (posToCheck.y > 0)
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

    
    string GetWordFromPositionsHorizontal(Vector2Int startPos, Vector2Int endPos, Gem gemToCheck)
    {
        string word = gemToCheck.letterValue;

        // Yatay bir kelime oluşturma
        for (int x = startPos.x-1; x >= endPos.x; x--)
        {
            word += allGems[x, startPos.y].letterValue;
        }

        return word;
    }
    string GetWordFromPositionsVertical(Vector2Int startPos, Vector2Int endPos, Gem gemToCheck)
    {
        string word = gemToCheck.letterValue;

        for (int y = startPos.y-1; y >= endPos.y; y--)
        {
            word += allGems[startPos.x, y].letterValue;
        }

        return word;
    }


    bool IsValidWord(string word)
    {

        return wordDatabase.Contains(word.ToLower());  
    }


    // Anlamlı kelimeleri yok et
    private void DestroyMatchedLetterAt(Vector2Int pos)
    {
        if (allGems[pos.x, pos.y] != null)
        {
            if (allGems[pos.x, pos.y].isMatched)
            {
                //Instantiate(allGems[pos.x, pos.y].destroyEffect, new Vector2(pos.x, pos.y), Quaternion.identity);

                Destroy(allGems[pos.x, pos.y].gameObject);
                allGems[pos.x, pos.y] = null;
            }
        }
    }

    public void DestroyMatches()
    {
        StartCoroutine(HandleMatchesCoroutine());
    }

    private IEnumerator HandleMatchesCoroutine()
    {
        // 1. Eşleşen taşları yeşile boyama
        for (int i = 0; i < matchFind.currentMatches.Count; i++)
        {
            if (matchFind.currentMatches[i] != null)
            {
                allGems[matchFind.currentMatches[i].posIndex.x, matchFind.currentMatches[i].posIndex.y].GetComponent<SpriteRenderer>().color = Color.green;
            }
        }

        // 2. 3 saniye boyunca bekleme (animasyon)
        yield return new WaitForSeconds(3f);

        // 3. Eşleşen taşları yok etme
        for (int i = 0; i < matchFind.currentMatches.Count; i++)
        {
            if (matchFind.currentMatches[i] != null)
            {
                DestroyMatchedLetterAt(matchFind.currentMatches[i].posIndex);
            }
        }

        // 4. Satırları azaltma (yok edilen taşlardan sonra düşen taşlar)
        StartCoroutine(DecreaseRowCo());
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
                if (allGems[x, y] == null)
                {
                    nullCounter++;
                }
                else if (nullCounter > 0)
                {
                    allGems[x, y].posIndex.y -= nullCounter;
                    allGems[x, y - nullCounter] = allGems[x, y];
                    allGems[x, y] = null;
                }
            }
            nullCounter = 0;
        }

        StartCoroutine(FillBoardCo());
    }

    // Tahtayı doldur
    private IEnumerator FillBoardCo()
    {
        yield return new WaitForSeconds(.5f);
        RefillBoard();
        yield return new WaitForSeconds(.5f);
        matchFind.FindAllMatches();
        if (matchFind.currentMatches.Count > 0)
        {
            yield return new WaitForSeconds(.5f);
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
                if (allGems[x, y] == null)
                {
                    int letterToUse = Random.Range(0, gems.Length);
                    SpawnGem(new Vector2Int(x, y), gems[letterToUse]);
                }
            }
        }
        CheckMisplacedGems();
    }

    private void CheckMisplacedGems()
    {
        List<Gem> foundGems = new List<Gem>();

        foundGems.AddRange(FindObjectsOfType<Gem>());

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (foundGems.Contains(allGems[x, y]))
                {
                    foundGems.Remove(allGems[x, y]);
                }
            }
        }

        foreach (Gem g in foundGems)
        {
            Destroy(g.gameObject);
        }
    }

}
