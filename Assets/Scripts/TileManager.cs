using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TileManager {
    private string root;
    Dictionary<int, List<GameObject>> tileMap = new Dictionary<int, List<GameObject>>();

    System.Random rnd = new System.Random();

    public TileManager(string basePath) {
        root = basePath;
    }

    public void Add(int mask, string resourcePath) {
        string combinedPath = System.IO.Path.Combine(root, resourcePath);
        GameObject o = Resources.Load(combinedPath, typeof(GameObject))as GameObject;

        if (!tileMap.ContainsKey(mask)) {
            tileMap[mask] = new List<GameObject>();
        }

        tileMap[mask].Add(o);
    }
    public void Add(int mask, string resourcePath, int weight) {
        for (int i = 0; i < weight; i++) {
            Add(mask, resourcePath);
        }
    }

    public bool Contains(int mask) {
        return tileMap.ContainsKey(mask);
    }

    public GameObject GetRandom(int mask) {
        GameObject retrieved = null;
        if (tileMap.ContainsKey(mask)) {
            retrieved = tileMap[mask][0];
            int count = tileMap[mask].Count;
            int r = rnd.Next(0, count);
            retrieved = tileMap[mask][r];
        }

        return retrieved;
    }
}