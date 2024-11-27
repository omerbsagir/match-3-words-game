using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class LevelSO : ScriptableObject
{
    // her level için kamera değerleri belirle ona göre ayarla

    public int width, height;   
    public GameObject tilePrefab; 
    public float bombChance = 2f; 
    public int letterCountFM;

    public LayoutSO layout;
    public LevelGoalsSO goals;

    public CameraDetails cameraDetails;
    
}

[System.Serializable]
public class CameraDetails
{
    public float x, y,ortoSize;
}