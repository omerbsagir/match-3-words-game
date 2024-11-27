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

    [HideInInspector]
    public bool isGameOver = false;
    [HideInInspector]
    public bool isLevelPassed = false;
    [HideInInspector]
    public bool hasGameEnded=false;

    [SerializeField]
    private GameObject gameOverPanel;
    [SerializeField]
    private GameObject levelPassPanel;

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
        if (isGameOver && !hasGameEnded)
        {
            EndGame();
            return;
        }
    }
    private IEnumerator waitBeforeEnd()
    {
        Board.Instance.currentState = Board.BoardState.wait;
        yield return new WaitForSeconds(2f);

        hasGameEnded = true;
        SetGameOverPanel();
    }
    private void EndGame()
    {
        StartCoroutine(waitBeforeEnd());
        
    }
    private void SetGameOverPanel()
    {
        if (isLevelPassed)
        {
            levelPassPanel.SetActive(true);
        }
        else
        {
            gameOverPanel.SetActive(true);
        }
    }
}
