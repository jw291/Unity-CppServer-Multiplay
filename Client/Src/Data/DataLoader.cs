using UnityEngine;

public static class DataLoader
{
    public static T[] LoadTable<T>(string path)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(path);
        if (textAsset == null)
        {
            Debug.LogError($"[DataLoader] Failed to load: {path}");
            return new T[0];
        }

        string wrapped = "{\"items\":" + textAsset.text + "}";
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(wrapped);
        return wrapper.items;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] items;
    }
}