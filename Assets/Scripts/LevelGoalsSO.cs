using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelGoalsSO : ScriptableObject
{
    public int moveLimit;

    public int matchCount;
    public int glassCount;
    public int grassCount;
    public int hiddenCount;
    public Gem hiddenGem;
    public int woodCount;
    public int prizedCount;
    public Gem prizedGem;

}
