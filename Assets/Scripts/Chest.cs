using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour {

    private GameObject maze;

    // Start is called before the first frame update
    void Start() {
        maze = GameObject.Find("Maze");
    }

    // Update is called once per frame
    void Update() {

    }

    void OnCollisionStay() {
        maze.GetComponent<MazeCarver>().DoMaze();
        print("Game Won!");
    }
}