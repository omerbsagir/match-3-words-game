using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelNeedsManager : MonoBehaviour
{
    public static LevelNeedsManager Instance;

    public Transform goalsParent;
    public TextMeshProUGUI moveCount;
    public GameObject goalPrefab;

    private LevelGoalsSO currentGoals;

    public int remainingMoveCount;

    private GameObject letterPrefab;
    public int remainingLetterCount;

    private GameObject glassPrefab;
    public int remainingGlassCount;

    private GameObject grassPrefab;
    public int remainingGrassCount;

    private GameObject hiddenPrefab;
    public int remainingHiddenCount;

    private GameObject woodPrefab;
    public int remainingWoodCount;

    private GameObject prizedPrefab;
    public int remainingPrizedCount;

    private bool isGameOver = false; // Oyun durumunu takip eden bayrak

    public void Awake()
    {
        Instance = this;
    }

    public void SetLevelNeeds(int level)
    {
        currentGoals = LevelManager.Instance.allLevelGoals[level - 1];

        moveCount.text = currentGoals.moveLimit.ToString();
        remainingMoveCount = currentGoals.moveLimit;

        remainingLetterCount = currentGoals.matchCount.count;
        remainingGlassCount = currentGoals.glassCount.count;
        remainingGrassCount = currentGoals.grassCount.count;
        remainingHiddenCount = currentGoals.hiddenCount.count;
        remainingWoodCount = currentGoals.woodCount.count;
        remainingPrizedCount = currentGoals.prizedCount.count;

        if (currentGoals.matchCount.count > 0)
        {
            letterPrefab = Instantiate(goalPrefab, goalsParent);
            letterPrefab.transform.GetChild(0).GetComponent<RawImage>().texture = currentGoals.matchCount.image.texture;
            letterPrefab.transform.GetChild(1).GetComponent<Text>().text = remainingLetterCount.ToString();
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
        if (isGameOver)
            return; // Oyun bitmişse işlemleri durdur

        // Kalan hamle sayısını güncelle
        moveCount.text = remainingMoveCount.ToString();

        // Gereksinimlerden biri tamamlandıysa ilgili prefab'ı yok et
        UpdateGoalUI();

        // Oyun durumu kontrolü
        if (remainingMoveCount <= 0 || AreAllGoalsCompleted())
        {
            EndGame();
        }
    }

    private void UpdateGoalUI()
    {
        UpdateGoalPrefab(letterPrefab, ref remainingLetterCount);
        UpdateGoalPrefab(glassPrefab, ref remainingGlassCount);
        UpdateGoalPrefab(grassPrefab, ref remainingGrassCount);
        UpdateGoalPrefab(hiddenPrefab, ref remainingHiddenCount);
        UpdateGoalPrefab(woodPrefab, ref remainingWoodCount);
        UpdateGoalPrefab(prizedPrefab, ref remainingPrizedCount);
    }

    private void UpdateGoalPrefab(GameObject prefab, ref int remainingCount)
    {
        if (prefab != null && prefab.transform.GetChild(1).GetComponent<Text>() != null)
        {
            if (remainingCount <= 0)
            {
                Destroy(prefab);
            }
            else
            {
                prefab.transform.GetChild(1).GetComponent<Text>().text = remainingCount.ToString();
            }
        }
    }

    private bool AreAllGoalsCompleted()
    {
        return remainingLetterCount <= 0 &&
               remainingGlassCount <= 0 &&
               remainingGrassCount <= 0 &&
               remainingHiddenCount <= 0 &&
               remainingWoodCount <= 0 &&
               remainingPrizedCount <= 0;
    }

    private void EndGame()
    {
        isGameOver = true; // Oyun durumu güncellendi
        Debug.Log("Oyun Bitti!"); // Konsola mesaj yazdır

        // Oyun bitiş animasyonu veya durumu burada işlenebilir
        Time.timeScale = 0; // Oyun duraklatıldı
    }
}
