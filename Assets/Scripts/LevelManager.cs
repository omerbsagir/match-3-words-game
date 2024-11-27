using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public int level = 1;
    public int totalLevelCounts = 10;

    public LevelSO[] allLevels;

    public bool isGameOver = false;
    public bool isLevelPassed = false;

    public void Awake()
    {
        Instance = this;
    }

    public Gem[,] GetLevelLayoutWood()
    {
        return allLevels[level - 1].layout.GetLayoutWood();
    }
    public Gem[,] GetLevelLayoutGlass()
    {
        return allLevels[level - 1].layout.GetLayoutGlass();
    }
    public Gem[,] GetLevelLayoutGems()
    {
        return allLevels[level - 1].layout.GetLayoutGems();
    }
    public Gem[,] GetLevelLayoutGrass()
    {
        return allLevels[level - 1].layout.GetLayoutGrass();
    }
    public Gem[,] GetLevelLayoutHidden()
    {
        return allLevels[level - 1].layout.GetLayoutHidden();
    }
    public bool[,] GetLevelLayoutPoison()
    {
        return allLevels[level - 1].layout.GetLayoutPoison();
    }

    public int[] GetLevelDimensions()
    {
        int[] wAndH = new int[2];

        wAndH[0] = allLevels[level - 1].width;
        wAndH[1] = allLevels[level - 1].height;

        return wAndH;
    }
    public GameObject getTilePrefab()
    {
        return allLevels[level - 1].tilePrefab;
    }
    public float getBombChance()
    {
        return allLevels[level - 1].bombChance;
    }
    public int getLetterCountFM()
    {
        return allLevels[level - 1].letterCountFM-1;
    }

    private void Update()
    {
        if (isGameOver)
        {
            EndGame();
            return;
        }
    }
    private IEnumerator waitBeforeEnd()
    {
        yield return new WaitForSeconds(2f);
        Board.Instance.currentState = Board.BoardState.wait;
    }
    private void EndGame()
    {
        StartCoroutine(waitBeforeEnd());
        Debug.Log("Game over");
    }
}
