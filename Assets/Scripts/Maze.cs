using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Uses the Recursive Backtracker method of Maze Generation.

class Maze {
    // Dimensions of the Maze
    public int width;
    public int height;

    // 2D Array of Cells
    private Cell[, ] cells;

    System.Random rnd = new System.Random();

    public Maze(int Width, int Height) {
        width = Width;
        height = Height;

        setupCells();
    }

    public Cell GetCell(int x, int y) {
        Cell c = null;
        if ((x >= 0 && x < width) && (y >= 0 && y < height)) {
            c = cells[x, y];
        } else {
            throw new System.IndexOutOfRangeException("Cannot access Cell at position " + new Vector2Int(x, y) + " as the position is out of bounds.");
        }
        return c;
    }

    void setupCells() {
        cells = new Cell[width, height];
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                var a = new Cell(new Vector2Int(x, y));
                if (x == 0) {
                    a.left = Edge.NoEdge;
                }
                if (y == 0) {
                    a.up = Edge.NoEdge;
                }
                if (x == width - 1) {
                    a.right = Edge.NoEdge;
                }
                if (y == height - 1) {
                    a.down = Edge.NoEdge;
                }
                cells[x, y] = a;
            }
        }
    }

    public void Generate(int x, int y) {
        Stack<Cell> stack = new Stack<Cell>();
        // 1. Choose a starting point on a grid.
        var start = cells[0, 0];
        start.Visit();
        stack.Push(start);

        Cell next = start;
        do {

            // 2. Choose one of its unvisited neighboring nodes randomly
            Cell neighbor = getNeighbor(next);

            // 3. If an unvisited neighbor exists, continue with neighboring node
            if (neighbor != null) {
                neighbor.Visit();
                updateCell(next, neighbor);

                stack.Push(neighbor);
                next = neighbor;
            }
            // 4. If no unvisited neighbor exists, backtrack.
            else {
                stack.Pop();
                next = stack.Peek();
            }

        } while (next != start);

    }

    // Add random connections in the maze
    // load: 0 to 1
    public void Deteriorate(float load) {
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                Cell start = cells[x, y];
                Cell neighbor = getNeighbor(start);
                // Debug.Log(start);
                // Debug.Log(neighbor);
            }
        }
    }

    Cell getNeighbor(Cell cell) {

        Vector2Int p = cell.pos;
        // make a list of all unvisited neighboors
        List<Cell> unvisited = new List<Cell>();
        if (cell.right == Edge.NotExists) {
            if (isCellUnvisited(new Vector2Int(p.x + 1, p.y))) {
                unvisited.Add(cells[p.x + 1, p.y]);
            }
        }
        if (cell.left == Edge.NotExists) {
            if (isCellUnvisited(new Vector2Int(p.x - 1, p.y))) {
                unvisited.Add(cells[p.x - 1, p.y]);
            }
        }
        if (cell.up == Edge.NotExists) {
            if (isCellUnvisited(new Vector2Int(p.x, p.y - 1))) {
                unvisited.Add(cells[p.x, p.y - 1]);
            }
        }
        if (cell.down == Edge.NotExists) {
            if (isCellUnvisited(new Vector2Int(p.x, p.y + 1))) {
                unvisited.Add(cells[p.x, p.y + 1]);
            }
        }

        // if there are no unvisited nodes return null
        if (unvisited.Count == 0) {
            return null;
        }
        // else choose randomly one of the unvisited neighbors
        else {
            int rand = rnd.Next(0, unvisited.Count);
            return unvisited[rand];
        }

    }

    void updateCell(Cell a, Cell b) {
        if (a.pos.x == b.pos.x) {
            if (a.pos.y > b.pos.y) {
                a.up = Edge.Exists;
                b.down = Edge.Exists;
            } else {
                a.down = Edge.Exists;
                b.up = Edge.Exists;
            }
        }

        if (a.pos.y == b.pos.y) {
            if (a.pos.x > b.pos.x) {
                a.left = Edge.Exists;
                b.right = Edge.Exists;
            } else {
                a.right = Edge.Exists;
                b.left = Edge.Exists;
            }
        }
    }

    bool isCellUnvisited(Vector2Int pos) {
        return (cells[pos.x, pos.y].isVisited() == false);
    }

}