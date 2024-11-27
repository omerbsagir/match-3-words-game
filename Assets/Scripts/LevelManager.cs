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
        return allLevels[level - 1].letterCountFM;
    }

}
