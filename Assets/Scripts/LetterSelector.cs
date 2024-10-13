using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LetterSelector : MonoBehaviour
{
    private Dictionary<char, float> vowelFrequencies = new Dictionary<char, float>()
    {
        { 'A', 8.17f }, { 'E', 12.70f }, { 'I', 6.97f }, { 'O', 7.51f }, { 'U', 2.76f }
    };

    private Dictionary<char, float> consonantFrequencies = new Dictionary<char, float>() 
    {
        { 'B', 1.49f }, { 'C', 2.71f }, { 'D', 4.32f }, { 'F', 2.30f }, { 'G', 2.02f }, { 'H', 6.09f },
        { 'J', 0.10f }, { 'K', 0.69f }, { 'L', 3.98f }, { 'M', 2.61f }, { 'N', 6.75f }, { 'P', 1.92f },
        { 'Q', 0.10f }, { 'R', 5.99f }, { 'S', 6.33f }, { 'T', 9.06f }, { 'V', 1.11f }, { 'W', 2.36f },
        { 'X', 0.15f }, { 'Y', 1.99f }, { 'Z', 0.07f }
    };

    private float vowelProbability = 0.4f;  // Sesli harf oranı (örn: %40)

    public int GetRandomLetter()
    {
        char letter;

        // İlk olarak, sesli veya sessiz harf seçileceğine karar ver
        if (Random.value < vowelProbability)
        {
            // Sesli harf frekanslarına göre sesli harf seç
            letter = GetWeightedRandomVowel();
        }
        else
        {
            // Sessiz harf frekanslarına göre sessiz harf seç
            letter = GetWeightedRandomConsonant();
        }

        // Harfin alfabedeki sırasını döndür (A -> 0, B -> 1, ... Z -> 25)
        return GetLetterIndex(letter);
    }

    private char GetWeightedRandomVowel()
    {
        float totalWeight = vowelFrequencies.Values.Sum();
        float randomWeight = Random.Range(0, totalWeight);

        float cumulativeWeight = 0f;

        foreach (var vowel in vowelFrequencies)
        {
            cumulativeWeight += vowel.Value;
            if (randomWeight <= cumulativeWeight)
            {
                return vowel.Key;
            }
        }

        return vowelFrequencies.Keys.First(); // Default: İlk harf seçilmezse
    }

    private char GetWeightedRandomConsonant()
    {
        // Tüm sessiz harflerin toplam ağırlığını bul
        float totalWeight = consonantFrequencies.Values.Sum();

        // Ağırlığa bağlı rastgele bir sayı üret
        float randomWeight = Random.Range(0, totalWeight);

        float cumulativeWeight = 0f;

        // Sessiz harflerden frekansa göre seçim yap
        foreach (var consonant in consonantFrequencies)
        {
            cumulativeWeight += consonant.Value;
            if (randomWeight <= cumulativeWeight)
            {
                return consonant.Key;
            }
        }

        // Hiçbir şey seçilmediyse (ki bu çok nadiren olur), ilk harfi döndür
        return consonantFrequencies.Keys.First();
    }

    private int GetLetterIndex(char letter)
    {
        return char.ToUpper(letter) - 'A'; // 'A' harfi 0. indekse denk gelir, 'B' 1, vb.
    }
}
