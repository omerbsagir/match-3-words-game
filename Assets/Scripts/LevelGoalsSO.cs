using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu()]
public class LevelGoalsSO : ScriptableObject
{
    public int moveLimit;

    public Needs matchCount;
    public Needs glassCount;
    public Needs grassCount;
    public Needs hiddenCount;
    public Needs woodCount;
    public Needs prizedCount;


    [System.Serializable]
    public class Needs
    {
        public Sprite image;
        public int count;
    }
}
