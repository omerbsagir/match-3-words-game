using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchFinder : MonoBehaviour
{
    private Board board;
    public List<Gem> currentMatches = new List<Gem>();
    private List<string> wordDatabase = new List<string>();
    public int letterCountForMatch;

    private WordDatabase wd;

    private void Awake()
    {
        wd = FindObjectOfType<WordDatabase>();
        letterCountForMatch--;
        board = FindObjectOfType<Board>();
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
                    string reverseHorizontalWordR = new string(horizontalWordR.Reverse().ToArray());
                    string reverseHorizontalWordL = new string(horizontalWordL.Reverse().ToArray());

                    if (IsValidWord(horizontalWordR) || IsValidWord(reverseHorizontalWordR))
                    {
                        MarkGemsAsMatched(horizontalGemsRight);
                    }
                    if (IsValidWord(horizontalWordL) || IsValidWord(reverseHorizontalWordL))
                    {
                        MarkGemsAsMatched(horizontalGemsLeft);
                    }

                    // Dikey kelime kontrolü
                    List<Gem> verticalGemsAbove = GetVerticalGemsAbove(x, y);
                    List<Gem> verticalGemsUnder = GetVerticalGemsUnder(x, y);

                    string verticalWordA = GetWordFromGems(verticalGemsAbove);
                    string verticalWordU = GetWordFromGems(verticalGemsUnder);
                    string reverseVerticalWordA = new string(verticalWordA.Reverse().ToArray());
                    string reverseVerticalWordU = new string(verticalWordU.Reverse().ToArray());

                    if (IsValidWord(verticalWordA) || IsValidWord(reverseVerticalWordA))
                    {
                        MarkGemsAsMatched(verticalGemsAbove);
                    }
                    if (IsValidWord(verticalWordU) || IsValidWord(reverseVerticalWordU))
                    {
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
            int x = gem.posIndex.x;
            int y = gem.posIndex.y;


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
            int x = gem.posIndex.x;
            int y = gem.posIndex.y;


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
