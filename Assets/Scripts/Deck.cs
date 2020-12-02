using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "My Assets/Deck")]
public class Deck : ScriptableObject
{
    [SerializeField] private CardData[] cardDatas;
    public CardData[] CardDatas { get => cardDatas; }
    private List<CardData> library;
    private List<CardData> discardPile;
    private bool isShuffling;
    public bool IsShuffling { get => isShuffling; }

    public void SetupDeck(CardData[] cardDatas)
    {
        this.cardDatas = cardDatas;
        library = new List<CardData>(cardDatas);
        discardPile = new List<CardData>();
        FindObjectOfType<CardManager>().StartCoroutine(Shuffle()); // TODO: cheating
    }

    internal CardData DrawCard()
    {
        if (library.Count < 1)
        {
            Debug.LogWarning($"Player library out of cards. Player can still continue playing.");
            return null;
        }
        else
        {
            var topDeckCard = library[0];
            library.Remove(topDeckCard);
            return topDeckCard;
        }
    }

    internal IEnumerator DrawCardRoutine()
    {
        while (isShuffling)
        {
            yield return null;
        }

        //var topDeckCard = library[library.Count - 1];
        //library.Remove(topDeckCard);
        //return topDeckCard;
    }

    internal IEnumerator Shuffle()
    {
        isShuffling = true;
        //var rng = new System.Random();

        //int n = library.Count;

        //while (n > 1)
        //{
        //    n--;
        //    int k = rng.Next(n - 1);
        //    CardData value = library[k];
        //    Debug.Log($"Trying to place {value.cardName} at library.[{n}] out of [{library.Count}]");
        //    library[k] = library[n];
        //    library[n] = value;
        //    yield return new WaitForSeconds(0.1f);
        //}

        var count = library.Count;
        var last = count;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = library[i];
            Debug.Log($"count: {count} | last: {last} | i: {i} | r: {r} | {tmp}");
            library[i] = library[r];
            library[r] = tmp;
        }
            isShuffling = false;

        for (int i = 0; i < library.Count; i++)
        {
            Debug.Log($"{i} - {library[i]}");
        }
        yield return null;
    }
}
