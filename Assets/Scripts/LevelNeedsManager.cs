using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelNeedsManager : MonoBehaviour
{
    public Transform goalsParent;
    public TextMeshProUGUI moveCount;
    public GameObject goalPrefab;

    private LevelGoalsSO currentGoals;

    private GameObject matchPrefab;
    private int remainingMatchCount;

    private GameObject glassPrefab;
    private int remainingGlassCount;

    private GameObject grassPrefab;
    private int remainingGrassCount;

    private GameObject hiddenPrefab;
    private int remainingHiddenCount;

    private GameObject woodPrefab;
    private int remainingWoodCount;

    private GameObject prizedPrefab;
    private int remainingPrizedCount;


    public void SetLevelNeeds(int level)
    {
        currentGoals = LevelManager.Instance.allLevelGoals[level - 1];

        moveCount.text = currentGoals.moveLimit.ToString();

        remainingMatchCount = currentGoals.matchCount.count;
        remainingGlassCount = currentGoals.glassCount.count;
        remainingGrassCount = currentGoals.grassCount.count;
        remainingHiddenCount = currentGoals.hiddenCount.count;
        remainingWoodCount = currentGoals.woodCount.count;
        remainingPrizedCount = currentGoals.prizedCount.count;



        if (currentGoals.matchCount.count > 0)
        {
            matchPrefab = Instantiate(goalPrefab, goalsParent);
            matchPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = currentGoals.matchCount.image.texture;
            matchPrefab.transform.GetChild(1).GetComponent<Text>().text = remainingMatchCount.ToString();
        }
        if (currentGoals.glassCount.count > 0)
        {
            glassPrefab = Instantiate(goalPrefab, goalsParent);
            glassPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = currentGoals.glassCount.image.texture;
            glassPrefab.transform.GetChild(1).GetComponent<Text>().text = remainingGlassCount.ToString();
        }
        if (currentGoals.grassCount.count > 0)
        {
            grassPrefab = Instantiate(goalPrefab, goalsParent);
            grassPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = currentGoals.grassCount.image.texture;
            grassPrefab.transform.GetChild(1).GetComponent<Text>().text = remainingGrassCount.ToString();
        }
        if (currentGoals.hiddenCount.count > 0)
        {
            hiddenPrefab = Instantiate(goalPrefab, goalsParent);
            hiddenPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = currentGoals.hiddenCount.image.texture;
            hiddenPrefab.transform.GetChild(1).GetComponent<Text>().text = remainingHiddenCount.ToString();
        }
        if (currentGoals.woodCount.count > 0)
        {
            woodPrefab = Instantiate(goalPrefab, goalsParent);
            woodPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = currentGoals.woodCount.image.texture;
            woodPrefab.transform.GetChild(1).GetComponent<Text>().text = remainingWoodCount.ToString();
        }
        if (currentGoals.prizedCount.count > 0)
        {
            prizedPrefab = Instantiate(goalPrefab, goalsParent);
            prizedPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = currentGoals.prizedCount.image.texture;
            prizedPrefab.transform.GetChild(1).GetComponent<Text>().text = remainingPrizedCount.ToString();
        }
    }
    private void Update()
    {
        // ŞİMDİLİK BÖYLE DAHA SONRA ANİMASYON İLE DÜŞÜRÜCEZ

        if (matchPrefab != null && matchPrefab.transform.GetChild(1).GetComponent<Text>() != null)
        {
            matchPrefab.transform.GetChild(1).GetComponent<Text>().text = remainingMatchCount.ToString();
        }

        if (glassPrefab != null && glassPrefab.transform.GetChild(1).GetComponent<Text>() != null)
        {
            glassPrefab.transform.GetChild(1).GetComponent<Text>().text = remainingGlassCount.ToString();
        }

        if (grassPrefab != null && grassPrefab.transform.GetChild(1).GetComponent<Text>() != null)
        {
            grassPrefab.transform.GetChild(1).GetComponent<Text>().text = remainingGrassCount.ToString();
        }

        if (hiddenPrefab != null && hiddenPrefab.transform.GetChild(1).GetComponent<Text>() != null)
        {
            hiddenPrefab.transform.GetChild(1).GetComponent<Text>().text = remainingHiddenCount.ToString();
        }

        if (woodPrefab != null && woodPrefab.transform.GetChild(1).GetComponent<Text>() != null)
        {
            woodPrefab.transform.GetChild(1).GetComponent<Text>().text = remainingWoodCount.ToString();
        }

        if (prizedPrefab != null && prizedPrefab.transform.GetChild(1).GetComponent<Text>() != null)
        {
            prizedPrefab.transform.GetChild(1).GetComponent<Text>().text = remainingPrizedCount.ToString();
        }
    }

    
    
}
