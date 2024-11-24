using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public int level = 1;
    public int totalLevelCounts = 10;

    public LevelSize[] widthAndHeights;
    public LayoutSO[] allLayouts;
    public LevelGoalsSO[] allLevelGoals;

    public void Start()
    {
        Instance = this;
    }

    public Gem[,] GetLevelLayoutWood()
    {
        return allLayouts[level - 1].GetLayoutWood();
    }
    public Gem[,] GetLevelLayoutGlass()
    {
        return allLayouts[level - 1].GetLayoutGlass();
    }
    public Gem[,] GetLevelLayoutGems()
    {
        return allLayouts[level - 1].GetLayoutGems();
    }
    public Gem[,] GetLevelLayoutGrass()
    {
        return allLayouts[level - 1].GetLayoutGrass();
    }
    public Gem[,] GetLevelLayoutHidden()
    {
        return allLayouts[level - 1].GetLayoutHidden();
    }

    public int[] GetLevelDimensions()
    {
        int[] wAndH = new int[2];

        wAndH[0] = widthAndHeights[level - 1].w;
        wAndH[1] = widthAndHeights[level - 1].h;

        return wAndH;
    }


    [System.Serializable]
    public class LevelSize
    {
        public int w;
        public int h;

        public LevelSize(int width, int height)
        {
            this.w = width;
            this.h = height;
        }
    }
}
