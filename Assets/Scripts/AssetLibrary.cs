using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AssetLibrary<TKey, TValue> : MonoBehaviour
{
    private Dictionary<TKey, TValue> library;
    [SerializeField] TKey[] keys = default;
    [SerializeField] TValue[] values = default;

    private void Awake()
    {
        library = new Dictionary<TKey, TValue>();

        for (int i = 0; i < keys.Length; i++)
        {
            if (values[i] != null)
                library.Add(keys[i], values[i]);
        }
    }

    public TValue GetValue(TKey key)
    {
        if (library.ContainsKey(key))
            return library[key];
        else
        {
            Debug.LogWarning($"{key} not found in {name}");
            return default;
        }
    }

    public bool KeyFound(TKey key)
    {
        if (library.ContainsKey(key))
            return true;
        else
            return false;
    }
}
