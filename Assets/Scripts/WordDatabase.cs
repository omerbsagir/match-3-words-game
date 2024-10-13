using System.Collections.Generic;
using UnityEngine;

public class WordDatabase : MonoBehaviour
{
    [HideInInspector]
    public List<string> wordList;

    void Awake()
    {
        TextAsset wordFile = Resources.Load<TextAsset>("words");
        if (wordFile != null)
        {
            wordList = new List<string>(wordFile.text.Split(new char[] { '\n' }));
        }
        else
        {
            Debug.LogError("Kelime dosyası bulunamadı: words.txt");
        }
    }
}
