using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void LoadGameScene()
    {
        SceneManager.LoadScene("Game");
        StartCoroutine(SetupLevelAfterSceneLoad());
    }
    public void LoadMenuScene()
    {
        SceneManager.LoadScene("MainMenu");
    }
    private IEnumerator SetupLevelAfterSceneLoad()
    {
        yield return null; // Yeni sahne yüklenmesini bekler
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.SetEverythingForLevel();
        }
    }
}
