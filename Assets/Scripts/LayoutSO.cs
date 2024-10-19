using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LayoutSO : ScriptableObject
{
    public LayoutRow[] allRowsWood;
    public LayoutRow[] allRowsGlass;
    public LayoutRow[] allRowsGems;
    public LayoutRow[] allRowsGrass;
    public LayoutRow[] allRowsHidden;


    public Gem[,] GetLayoutWood()
    {
        Gem[,] theLayout = new Gem[allRowsWood[0].gemsInRow.Length, allRowsWood.Length];

        for (int y = 0; y < allRowsWood.Length; y++)
        {
            for (int x = 0; x < allRowsWood[y].gemsInRow.Length; x++)
            {
                if (x < theLayout.GetLength(0))
                {
                    if (allRowsWood[y].gemsInRow[x] != null)
                    {
                        theLayout[x, allRowsWood.Length - 1 - y] = allRowsWood[y].gemsInRow[x];
                    }
                }
            }
        }



        return theLayout;
    }
    public Gem[,] GetLayoutGlass()
    {
        Gem[,] theLayout = new Gem[allRowsGlass[0].gemsInRow.Length, allRowsGlass.Length];

        for (int y = 0; y < allRowsGlass.Length; y++)
        {
            for (int x = 0; x < allRowsGlass[y].gemsInRow.Length; x++)
            {
                if (x < theLayout.GetLength(0))
                {
                    if (allRowsGlass[y].gemsInRow[x] != null)
                    {
                        theLayout[x, allRowsGlass.Length - 1 - y] = allRowsGlass[y].gemsInRow[x];
                    }
                }
            }
        }



        return theLayout;
    }
    public Gem[,] GetLayoutGems()
    {
        Gem[,] theLayout = new Gem[allRowsGems[0].gemsInRow.Length, allRowsGems.Length];

        for (int y = 0; y < allRowsGems.Length; y++)
        {
            for (int x = 0; x < allRowsGems[y].gemsInRow.Length; x++)
            {
                if (x < theLayout.GetLength(0))
                {
                    if (allRowsGems[y].gemsInRow[x] != null)
                    {
                        theLayout[x, allRowsGems.Length - 1 - y] = allRowsGems[y].gemsInRow[x];
                    }
                }
            }
        }



        return theLayout;
    }
    public Gem[,] GetLayoutGrass()
    {
        Gem[,] theLayout = new Gem[allRowsGrass[0].gemsInRow.Length, allRowsGrass.Length];

        for (int y = 0; y < allRowsGrass.Length; y++)
        {
            for (int x = 0; x < allRowsGrass[y].gemsInRow.Length; x++)
            {
                if (x < theLayout.GetLength(0))
                {
                    if (allRowsGrass[y].gemsInRow[x] != null)
                    {
                        theLayout[x, allRowsGrass.Length - 1 - y] = allRowsGrass[y].gemsInRow[x];
                    }
                }
            }
        }



        return theLayout;
    }
    public Gem[,] GetLayoutHidden()
    {
        Gem[,] theLayout = new Gem[allRowsHidden[0].gemsInRow.Length, allRowsHidden.Length];

        for (int y = 0; y < allRowsHidden.Length; y++)
        {
            for (int x = 0; x < allRowsHidden[y].gemsInRow.Length; x++)
            {
                if (x < theLayout.GetLength(0))
                {
                    if (allRowsHidden[y].gemsInRow[x] != null)
                    {
                        theLayout[x, allRowsHidden.Length - 1 - y] = allRowsHidden[y].gemsInRow[x];
                    }
                }
            }
        }



        return theLayout;
    }
}

[System.Serializable]
public class LayoutRow
{
    public Gem[] gemsInRow;
}
