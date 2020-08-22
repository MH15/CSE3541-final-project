using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Cell {
    private bool visited;
    public Vector2Int pos;

    // edges
    public Edge left;
    public Edge right;
    public Edge up;
    public Edge down;

    public bool weave = false;

    public bool built = false;

    public int level = 0;

    public Cell(Vector2Int p) {
        visited = false;
        pos = p;
    }

    public override string ToString() {
        return "Cell{" + visited + ", pos:" + pos + ", level: " + level + "}";
    }

    public void Visit() {
        visited = true;
    }

    public bool isVisited() {
        return visited;
    }

    public Vector3 Vector() {
        return new Vector3(pos.x, 0, pos.y);
    }

}

enum Direction {
    North = 0,
    South,
    East,
    West
}

public enum Edge {
    Exists = 1,
    NotExists = 0,
    NoEdge = -1,
    Weave = 2,
}