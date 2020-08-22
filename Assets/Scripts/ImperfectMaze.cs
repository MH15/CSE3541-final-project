using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// Uses the Recursive Backtracker method of Maze Generation.

class ImperfectMaze {
    // Dimensions of the Maze
    public int width;
    public int height;

    // 2D Array of Cells
    private Cell[, ] cells;

    System.Random rnd = new System.Random();

    public ImperfectMaze(int Width, int Height) {
        width = Width;
        height = Height;

        setupCells();
    }

    public Cell GetCell(int x, int y) {
        Cell c = null;
        if ((x >= 0 && x < width) && (y >= 0 && y < height)) {
            c = cells[x, y];
        } else {
            Debug.LogError("Cannot access Cell at position " + new Vector2Int(x, y) + " as the position is out of bounds.");
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

    public void Generate() {
        Stack<Cell> stack = new Stack<Cell>();
        // 1. Choose a starting point on a grid.
        var start = cells[0, 0];
        start.Visit();
        stack.Push(start);

        Cell current = start;
        Cell last = start;

        Vector2Int movingDirection = new Vector2Int();

        int safety = 1000;
        do {
            // Debug.Log(last);
            // Debug.Log(current);
            movingDirection = current.pos - last.pos;
            last = current;
            // Debug.Log(movingDirection);

            // 2. Choose one of its unvisited neighboring nodes randomly
            Cell nextNeighbor = getNeighbor(current);

            // 3. If an unvisited neighbor exists, continue with neighboring node
            if (nextNeighbor != null) {
                nextNeighbor.Visit();
                updateCell(current, nextNeighbor);

                stack.Push(nextNeighbor);
                current = nextNeighbor;
            }
            // 4. If no unvisited neighbor exists, choose whether to connect or to backtrack.
            else {
                int r = rnd.Next(0, 10);
                // We know no univisited neighbors exist, now check if any visited neighbors exist that we can connect with.
                Vector2Int testPos = current.pos + movingDirection;
                if (r < 0 && (testPos.x >= 0 && testPos.x < width) && (testPos.y >= 0 && testPos.y < height)) {
                    // Connect
                    Cell testCell = GetCell(testPos.x, testPos.y);
                    if (movingDirection == new Vector2Int(0, 1)) {
                        current.down = Edge.Exists;
                        testCell.up = Edge.Exists;
                    } else if (movingDirection == new Vector2Int(0, -1)) {
                        current.up = Edge.Exists;
                        testCell.down = Edge.Exists;
                    } else if (movingDirection == new Vector2Int(1, 0)) {
                        current.right = Edge.Exists;
                        testCell.left = Edge.Exists;
                    } else if (movingDirection == new Vector2Int(-1, 0)) {
                        current.left = Edge.Exists;
                        testCell.right = Edge.Exists;
                    }

                } else {
                    // Backtrack
                    last = stack.Pop();
                    current = stack.Peek();

                }
            }

            if (safety <= 0) {
                break;
            }
            safety--;

        } while (current != start);

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