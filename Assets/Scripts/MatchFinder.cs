using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using static UnityEditor.ShaderData;

public class MatchFinder : MonoBehaviour
{
    private Board board;
    public List<Gem> currentMatches = new List<Gem>();
    private List<string> wordDatabase = new List<string>();
    public int letterCountForMatch;

    private WordDatabase wd;

    public List<Word> currentWords;
    public List<Vector2Int> middleGems;

    private void Awake()
    {
        wd = FindObjectOfType<WordDatabase>();
        letterCountForMatch--;
        board = FindObjectOfType<Board>();
        currentWords = new List<Word>();
    }

    private void Start()
    {
        wordDatabase = wd.wordList;
        
    }

    public void FindAllMatches()
    {
        currentMatches.Clear();

        // Tahtadaki tüm taşları kontrol et
        for (int x = 0; x < board.width; x++)
        {
            for (int y = 0; y < board.height; y++)
            {
                Gem currentGem = board.allGems[x, y];
                if (currentGem != null)
                {
                    // Yatay kelime kontrolü
                    List<Gem> horizontalGemsRight = GetHorizontalGemsRight(x, y);
                    List<Gem> horizontalGemsLeft = GetHorizontalGemsLeft(x, y);

                    string horizontalWordR = GetWordFromGems(horizontalGemsRight);
                    string horizontalWordL = GetWordFromGems(horizontalGemsLeft);
                    

                    if (IsValidWord(horizontalWordR))
                    {
                        currentWords.Add(new Word(horizontalGemsRight));
                        middleGems.Add(new Word(horizontalGemsRight).MiddleGem());
                        MarkGemsAsMatched(horizontalGemsRight);
                    }
                    if (IsValidWord(horizontalWordL))
                    {
                        currentWords.Add(new Word(horizontalGemsLeft));
                        middleGems.Add(new Word(horizontalGemsLeft).MiddleGem());
                        MarkGemsAsMatched(horizontalGemsLeft);
                    }

                    // Dikey kelime kontrolü
                    List<Gem> verticalGemsAbove = GetVerticalGemsAbove(x, y);
                    List<Gem> verticalGemsUnder = GetVerticalGemsUnder(x, y);

                    string verticalWordA = GetWordFromGems(verticalGemsAbove);
                    string verticalWordU = GetWordFromGems(verticalGemsUnder);
                

                    if (IsValidWord(verticalWordA))
                    {
                        currentWords.Add(new Word(verticalGemsAbove));
                        middleGems.Add(new Word(verticalGemsAbove).MiddleGem());
                        MarkGemsAsMatched(verticalGemsAbove);
                    }
                    if (IsValidWord(verticalWordU))
                    {
                        currentWords.Add(new Word(verticalGemsUnder));
                        middleGems.Add(new Word(verticalGemsUnder).MiddleGem());
                        MarkGemsAsMatched(verticalGemsUnder);
                    }
                }
            }
        }

        if (currentMatches.Count > 0)
        {
            currentMatches = currentMatches.Distinct().ToList();
        }

        CheckForGlasses();
        CheckForWoods();
        CheckForGrasses();
        CheckHiddens();
        CheckPrized();

    }

    
    public List<Gem> GetHorizontalGemsRight(int startX, int y)
    {

        if ((startX + letterCountForMatch) < board.width)
        {
            List<Gem> gems = new List<Gem>();
            for (int x = startX; x <= startX+ letterCountForMatch && board.allGems[x, y] != null; x++)
            {
                gems.Add(board.allGems[x, y]);
            }
            return gems;
        }
        return null;
        
    }
    public List<Gem> GetHorizontalGemsLeft(int startX, int y)
    {

        if ((startX - letterCountForMatch) >= 0)
        {
            List<Gem> gems = new List<Gem>();
            for (int x = startX; x >= startX- letterCountForMatch && board.allGems[x, y] != null; x--)
            {
                gems.Add(board.allGems[x, y]);
            }
            return gems;
        }
        return null;
    }

    public List<Gem> GetVerticalGemsAbove(int x, int startY)
    {
        if ((startY + letterCountForMatch) < board.height)
        {
            List<Gem> gems = new List<Gem>();
            for (int y = startY; y <= startY+ letterCountForMatch && board.allGems[x, y] != null; y++)
            {
                gems.Add(board.allGems[x, y]);
            }
            return gems;
        }
        return null;
    }
    public List<Gem> GetVerticalGemsUnder(int x, int startY)
    {
        if ((startY - letterCountForMatch) >= 0)
        {
            List<Gem> gems = new List<Gem>();
            for (int y = startY; y >= startY - letterCountForMatch && board.allGems[x, y] != null; y--)
            {
                gems.Add(board.allGems[x, y]);
            }
            return gems;
        }
        return null;
    }


    public string GetWordFromGems(List<Gem> gems)
    {
        string word = "";

        if(gems != null)
        {
            foreach (Gem gem in gems)
            {
                word += gem.letterValue; // 'letter' taşın temsil ettiği harf
            }
        }

        return word;
    }

    public bool IsValidWord(string word)
    {
        return wordDatabase.Contains(word.ToLower()); // Küçük harfe çevirip kontrol et
    }

    public void MarkGemsAsMatched(List<Gem> gems)
    {
        foreach (Gem gem in gems)
        {
            gem.isMatched = true;
            currentMatches.Add(gem);
        }
    }

    public void CheckForGlasses()
    {
        List<Gem> glassGems = new List<Gem>();

        for (int i = 0; i < currentMatches.Count; i++)
        {
            Gem gem = currentMatches[i];
            int x = (int)gem.posIndex.x;
            int y = (int)gem.posIndex.y;


            // Sol tarafa bak (x - 1)
            if (x > 0 && board.allGlasses[x - 1, y] != null && board.allGems[x - 1, y] != null)
            {
                if (!currentMatches.Contains(board.allGems[x - 1, y]))
                {
                    board.allGems[x - 1, y].isMatched = true;
                    glassGems.Add(board.allGems[x - 1, y]);
                }
            }

            // Sağ tarafa bak (x + 1)
            if (x < board.width - 1 && board.allGlasses[x + 1, y] != null && board.allGems[x + 1, y] != null)
            {
                if (!currentMatches.Contains(board.allGems[x + 1, y]))
                {
                    board.allGems[x + 1, y].isMatched = true;
                    glassGems.Add(board.allGems[x + 1, y]);
                }
            }

            // Yukarı bak (y - 1)
            if (y > 0 && board.allGlasses[x, y - 1] != null && board.allGems[x, y - 1] != null)
            {
                if (!currentMatches.Contains(board.allGems[x, y - 1]))
                {
                    board.allGems[x, y-1].isMatched = true;
                    glassGems.Add(board.allGems[x, y - 1]);
                }
            }

            // Aşağı bak (y + 1)
            if (y < board.height - 1 && board.allGlasses[x, y + 1] != null && board.allGems[x, y + 1] != null)
            {
                if (!currentMatches.Contains(board.allGems[x, y + 1]))
                {
                    board.allGems[x, y+1].isMatched = true;
                    glassGems.Add(board.allGems[x, y + 1]);
                }
            }
        }

        foreach(Gem g in glassGems)
        {
            currentMatches.Add(g);
        }
    }
    public void CheckForWoods()
    {
        List<Gem> woodGems = new List<Gem>();

        for (int i = 0; i < currentMatches.Count; i++)
        {
            Gem gem = currentMatches[i];
            int x = (int)gem.posIndex.x;
            int y = (int)gem.posIndex.y;


            // Sol tarafa bak (x - 1)
            if (x > 0 && board.allWoods[x - 1, y] != null)
            {
                if (!currentMatches.Contains(board.allWoods[x - 1, y]))
                {
                    board.allWoods[x - 1, y].isMatched = true;
                    woodGems.Add(board.allWoods[x - 1, y]);
                }
            }

            // Sağ tarafa bak (x + 1)
            if (x < board.width - 1 && board.allWoods[x + 1, y] != null)
            {
                if (!currentMatches.Contains(board.allWoods[x + 1, y]))
                {
                    board.allWoods[x + 1, y].isMatched = true;
                    woodGems.Add(board.allWoods[x + 1, y]);
                }
            }

            // Yukarı bak (y - 1)
            if (y > 0 && board.allWoods[x, y - 1] != null)
            {
                if (!currentMatches.Contains(board.allWoods[x, y - 1]))
                {
                    board.allWoods[x, y - 1].isMatched = true;
                    woodGems.Add(board.allWoods[x, y - 1]);
                }
            }

            // Aşağı bak (y + 1)
            if (y < board.height - 1 && board.allWoods[x, y + 1] != null)
            {
                if (!currentMatches.Contains(board.allWoods[x, y + 1]))
                {
                    board.allWoods[x, y + 1].isMatched = true;
                    woodGems.Add(board.allWoods[x, y + 1]);
                }
            }
        }

        foreach (Gem g in woodGems)
        {
            currentMatches.Add(g);
        }
    }
    public void CheckForGrasses()
    {
        List<Gem> grass = new List<Gem>();

        for (int i = 0; i < currentMatches.Count; i++)
        {

            int x = (int)currentMatches[i].posIndex.x;
            int y = (int)currentMatches[i].posIndex.y;

            int x2 = x+1;
            int y2 = y;

            int x3 = x-1;
            int y3 = y;

            int x4 = x;
            int y4 = y+1;

            int x5 = x;
            int y5 = y-1;

            if (board.allGrasses[x,y] != null)
            {
                board.allGrasses[x, y].isMatched = true;
                grass.Add(board.allGrasses[x, y]);
            }
            if (x2<board.width && board.allGrasses[x2, y2] != null)
            {
                board.allGrasses[x2, y2].isMatched = true;
                grass.Add(board.allGrasses[x2, y2]);
            }
            if (x3>=0 && board.allGrasses[x3, y3] != null)
            {
                board.allGrasses[x3, y3].isMatched = true;
                grass.Add(board.allGrasses[x3, y3]);
            }
            if (y4 < board.height && board.allGrasses[x4, y4] != null)
            {
                board.allGrasses[x4, y4].isMatched = true;
                grass.Add(board.allGrasses[x4, y4]);
            }
            if (y5 >= 0 && board.allGrasses[x5, y5] != null)
            {
                board.allGrasses[x5, y5].isMatched = true;
                grass.Add(board.allGrasses[x5, y5]);
            }

            
        }

        grass.Distinct();

        foreach (Gem g in grass)
        {
            currentMatches.Add(g);
        }

    }
    public void CheckHiddens()
    {
        bool isClear = true;
        List<Vector2Int> grasses = new List<Vector2Int>();

        for (int x = 0; x < board.width; x++)
        {

            for (int y = 0; y < board.height; y++)
            {

                if (board.allHiddens[x, y] != null)
                {

                    isClear = true;

                    grasses.Add(new Vector2Int(x, y));
                    grasses.Add(new Vector2Int(x, y + 1));
                    grasses.Add(new Vector2Int(x + 1, y));
                    grasses.Add(new Vector2Int(x + 1, y + 1));

                    
                    for (int i = 0; i < 4; i++)
                    {
                        if (board.allGrasses[grasses[i].x, grasses[i].y] != null && !board.allGrasses[grasses[i].x, grasses[i].y].isMatched)
                        {
                            isClear = false;
                            break;
                        }
                    }

                    if (isClear)
                    {
                        Debug.Log("Silinmeli");
                        board.allHiddens[x, y].isMatched = true;
                        currentMatches.Add(board.allHiddens[x, y]);
                        
                    }

                    grasses.Clear();


                }
            }
        }
    }
    public void CheckPrized()
    {
        for (int i = 0; i < currentMatches.Count; i++)
        {

            int x = (int)currentMatches[i].posIndex.x;
            int y = (int)currentMatches[i].posIndex.y;

            int x2 = x + 1;
            int y2 = y;

            int x3 = x - 1;
            int y3 = y;

            int x4 = x;
            int y4 = y + 1;

            int x5 = x;
            int y5 = y - 1;

            
            if (x2 < board.width && board.allGems[x2, y2] != null && board.allGems[x2, y2].type == Gem.GemType.prized)
            {
                board.allGems[x2, y2].TriggerPrize(board.allGems[x2, y2]);
            }
            if (x3 >= 0 && board.allGems[x3, y3] != null && board.allGems[x3, y3].type == Gem.GemType.prized)
            {
                board.allGems[x3, y3].TriggerPrize(board.allGems[x3, y3]);
            }
            if (y4 < board.height && board.allGems[x4, y4] != null && board.allGems[x4, y4].type == Gem.GemType.prized)
            {
                board.allGems[x4, y4].TriggerPrize(board.allGems[x4, y4]);
            }
            if (y5 >= 0 && board.allGems[x5, y5] != null && board.allGems[x5, y5].type == Gem.GemType.prized)
            {
                board.allGems[x5, y5].TriggerPrize(board.allGems[x5, y5]);

            }
        }

    }

    /*public void CheckForBombs()
    {
        for (int i = 0; i < currentMatches.Count; i++)
        {
            Gem gem = currentMatches[i];
            int x = gem.posIndex.x;
            int y = gem.posIndex.y;

            if (gem.posIndex.x > 0 && board.allGems[x - 1, y] != null && board.allGems[x - 1, y].type == Gem.GemType.bomb)
            {
                MarkBombArea(new Vector2Int(x - 1, y), board.allGems[x - 1, y]);
            }

            if (gem.posIndex.x < board.width - 1 && board.allGems[x + 1, y] != null && board.allGems[x + 1, y].type == Gem.GemType.bomb)
            {
                MarkBombArea(new Vector2Int(x + 1, y), board.allGems[x + 1, y]);
            }

            if (gem.posIndex.y > 0 && board.allGems[x, y - 1] != null && board.allGems[x, y - 1].type == Gem.GemType.bomb)
            {
                MarkBombArea(new Vector2Int(x, y - 1), board.allGems[x, y - 1]);
            }

            if (gem.posIndex.y < board.height - 1 && board.allGems[x, y + 1] != null && board.allGems[x, y + 1].type == Gem.GemType.bomb)
            {
                MarkBombArea(new Vector2Int(x, y + 1), board.allGems[x, y + 1]);
            }
        }
    }

    public void MarkBombArea(Vector2Int bombPos, Gem theBomb)
    {
        for (int x = bombPos.x - theBomb.blastSize; x <= bombPos.x + theBomb.blastSize; x++)
        {
            for (int y = bombPos.y - theBomb.blastSize; y <= bombPos.y + theBomb.blastSize; y++)
            {
                if (x >= 0 && x < board.width && y >= 0 && y < board.height)
                {
                    if (board.allGems[x, y] != null)
                    {
                        board.allGems[x, y].isMatched = true;
                        currentMatches.Add(board.allGems[x, y]);
                    }
                }
            }
        }

        currentMatches = currentMatches.Distinct().ToList();
    }*/
}
public class Word
{
    public List<Gem> letters;

    public Word(List<Gem> letters)
    {
        this.letters = letters;
    }
    public Vector2Int MiddleGem()
    {
        Vector2 temp = letters[(int)(letters.Count - 1) / 2].posIndex;
        return new Vector2Int((int)temp.x, (int)temp.y);
    }
}
