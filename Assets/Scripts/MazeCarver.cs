using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteInEditMode]
public class MazeCarver : MonoBehaviour {
    // Dimensions of the Maze
    public int width = 3;
    public int height = 3;

    // Speed at which the Maze grows: smaller is faster.
    public float delay = 0.05f;

    // Prefab for the wall.
    public GameObject wall;
    // Center the camera over the Maze.
    public GameObject cam;

    System.Random rnd = new System.Random();

    TileManager tiles;

    GameObject stair;
    GameObject chest;
    Vector2Int goalPos;

    // Maze Node-based representation.
    ImperfectMaze maze;

    // Start is called before the first frame update
    void Start() {

        // cam.transform.position = new Vector3(width / 2 * 6, cam.transform.position.y, height / 2 * 6);

        tiles = new TileManager("Tiles");
        // 0bLURD
        tiles.Add(0b1111, "4-connector");
        tiles.Add(0b0110, "bottom-left-curve");
        tiles.Add(0b1100, "bottom-right-curve");
        tiles.Add(0b0011, "top-left-curve");
        tiles.Add(0b1001, "top-right-curve");

        tiles.Add(0b1010, "left-right");
        tiles.Add(0b0101, "up-down");

        // T connectors are hallways or rooms with fewer doors
        tiles.Add(0b1011, "bottom-T", 2);
        tiles.Add(0b1101, "left-T", 2);
        tiles.Add(0b0111, "right-T", 2);
        tiles.Add(0b1110, "top-T", 2);

        tiles.Add(0b1011, "4-connector");
        tiles.Add(0b1101, "4-connector");
        tiles.Add(0b0111, "4-connector");
        tiles.Add(0b1110, "4-connector");

        // End connectors are rooms with one door or cliffs/fallofs/traps
        tiles.Add(0b0010, "left-opening", 2);
        tiles.Add(0b1000, "right-opening", 2);
        tiles.Add(0b0100, "bottom-opening", 2);
        tiles.Add(0b0001, "top-opening", 2);

        tiles.Add(0b0010, "4-connector");
        tiles.Add(0b1000, "4-connector");
        tiles.Add(0b0100, "4-connector");
        tiles.Add(0b0001, "4-connector");

        stair = Resources.Load("Tiles/stair", typeof(GameObject))as GameObject;
        chest = GameObject.Find("Chest");

        DoMaze();

    }

    public void DoMaze() {
        // Calculate the maze
        maze = new ImperfectMaze(width, height);
        maze.Generate();

        // Delete the current maze
        foreach (Transform child in transform) {
            Destroy(child.gameObject);
        }

        // Find goal position
        // Goal should never be close to the start point (0, 0)
        int rWidth = rnd.Next(width / 2, width);
        int rHeight = rnd.Next(height / 2, width);
        goalPos = new Vector2Int(rWidth, rHeight);

        // Render maze
        carveMaze();

    }

    void carveMaze() {
        // choose a starting point
        Cell start = maze.GetCell(0, 0);
        Stack<Cell> stack = new Stack<Cell>();
        stack.Push(start);

        Cell current = start;
        Cell last = current;

        int safety = 1000;

        // add max level of probably 4

        do {
            var mask = getWallMask(current);

            GameObject s = null;
            current.level = last.level;

            if (!current.built && tiles.Contains(mask)) {
                var tile = tiles.GetRandom(mask);
                current.built = true;

                int r = rnd.Next(0, 10);

                // If mask is straight, half the time it should be a stair
                if (r < 5 && (mask == 0b0101 || mask == 0b1010)) {
                    s = placeStair(mask, current, last, tile.transform.rotation, r);
                } // else proceed normally
                else {
                    Vector3 place = current.Vector() * 6;
                    place.y = current.level * 3;
                    s = Instantiate(tile, place, tile.transform.rotation);
                }

                // Place the chest
                if (current.pos == goalPos) {
                    print(goalPos);
                    Vector3 pos = new Vector3(current.pos.x * 6, current.level * 3, current.pos.y * 6);
                    chest.transform.position = pos;
                }
            }

            if (s) {
                s.name += "_" + current.pos + "_" + current.level + "_" + System.Convert.ToString(mask, 2);
                s.transform.parent = transform;
            }
            last = current;
            Cell next = findNext(current);
            if (next != null) {
                current = next;
                stack.Push(next);
            } else {
                last = stack.Pop();
                current = stack.Peek();
                last.level = current.level;
            }
            if (safety <= 0) {
                break;
            }
            safety--;
        }
        while (current != start);
    }

    GameObject placeStair(int mask, Cell current, Cell last, Quaternion rot, int r) {
        GameObject s = null;

        Vector2Int movingDirection = new Vector2Int();

        // Create a stair
        Vector3 place = current.Vector() * 6;
        place.y = current.level * 3;
        s = Instantiate(stair, place, rot);

        movingDirection = current.pos - last.pos;

        if (movingDirection == new Vector2Int(0, 1)) {
            s.transform.rotation = Quaternion.Euler(0, 0f, 0);
        } else if (movingDirection == new Vector2Int(0, -1)) {
            s.transform.rotation = Quaternion.Euler(0, 180f, 0);
        } else if (movingDirection == new Vector2Int(1, 0)) {
            s.transform.rotation = Quaternion.Euler(0, 90f, 0);
        } else if (movingDirection == new Vector2Int(-1, 0)) {
            s.transform.rotation = Quaternion.Euler(0, 270f, 0);
        }

        // The rest of the time the stair goes down
        if (r >= 3 && current.level > 0) {
            // need to bump down
            current.level -= 1;
            s.transform.Translate(0, -3f, 0);

            s.transform.Rotate(0, 180f, 0);

        }
        // Part of the time the stair goes up a level
        else {
            // increment level
            current.level += 1;

        }

        return s;
    }

    Cell findNext(Cell cell) {
        Vector2Int p = cell.pos;
        Cell result = null;
        if (cell.right == Edge.Exists || cell.right == Edge.Weave) {
            if (maze.GetCell(p.x + 1, p.y) != null) {
                if (maze.GetCell(p.x + 1, p.y).built == false) {
                    result = maze.GetCell(p.x + 1, p.y);
                }
            }
        }
        if (cell.left == Edge.Exists || cell.left == Edge.Weave) {
            if (maze.GetCell(p.x - 1, p.y) != null) {
                if (maze.GetCell(p.x - 1, p.y).built == false) {
                    result = maze.GetCell(p.x - 1, p.y);
                }
            }
        }
        if (cell.up == Edge.Exists || cell.up == Edge.Weave) {
            if (maze.GetCell(p.x, p.y - 1) != null) {
                if (maze.GetCell(p.x, p.y - 1).built == false) {
                    result = maze.GetCell(p.x, p.y - 1);
                }
            }
        }
        if (cell.down == Edge.Exists || cell.down == Edge.Weave) {
            if (maze.GetCell(p.x, p.y + 1) != null) {
                if (maze.GetCell(p.x, p.y + 1).built == false) {
                    result = maze.GetCell(p.x, p.y + 1);
                }
            }
        }
        return result;
    }

    int getWallMask(Cell a) {
        // if there is a connection, add to the bitmask
        // In order: 0bLURD
        var mask = 0b0000;
        if (a.left == Edge.Exists) {
            mask += 0b1000;
        }
        if (a.right == Edge.Exists) {
            mask += 0b0010;
        }
        if (a.up == Edge.Exists) {
            mask += 0b0100;
        }
        if (a.down == Edge.Exists) {
            mask += 0b0001;
        }
        return mask;

    }

}