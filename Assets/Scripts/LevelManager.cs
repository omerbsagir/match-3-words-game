using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    public int level = 1;
    public int totalLevelCounts = 10;

    public LevelSize[] widthAndHeights;
    public LayoutSO[] allLayouts;


    public Gem[,] GetLevelLayout()
    {
        return allLayouts[level - 1].GetLayout();
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
