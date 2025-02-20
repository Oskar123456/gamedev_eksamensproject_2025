/*
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 *
 * */

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System;
using TMPro;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class PCGScript : MonoBehaviour
{
    public List<GameObject> wall_prefabs;
    public List<GameObject> tile_prefabs;
    public List<GameObject> deco_prefabs;
    public GameObject level_prefab;

    LevelBuilder level_builder;

    List<GameObject> walls = new List<GameObject>();
    List<GameObject> tiles = new List<GameObject>();
    List<GameObject> decos = new List<GameObject>();
    GameObject level_go;

    GameObject player;
    Transform player_trf;
    PlayerStats player_stats;

    System.Random rng = new System.Random();
    int level, difficulty;
    public int maze_width, maze_height;
    Maze maze;

    void Start()
    {
        player = GameObject.Find("Player");
        player_trf = player.GetComponent<Transform>();
        player_stats = player.GetComponent<PlayerStats>();
        level_builder = GetComponent<LevelBuilder>();

        GenMaze();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) {
            Clean();
            GenMaze();
        }
    }

    void Clean()
    {
        Destroy(level_go);
        // foreach (GameObject go in walls) Destroy(go); walls.Clear();
        // foreach (GameObject go in tiles) Destroy(go); tiles.Clear();
        // foreach (GameObject go in decos) Destroy(go); decos.Clear();
    }

    void GenMaze()
    {
        level_go = Instantiate(level_prefab, Vector3.zero, Quaternion.identity, transform);
        maze = new Maze(maze_width, maze_height);
        level_builder.Medieval(maze, level_go.GetComponent<Transform>());

        // for (int y = 0; y < maze.height; y++) {
        //     for (int x = 0; x < maze.width; x++) {
        //         if (maze.walls[y, x])
        //             walls.Add(Instantiate(wall_prefabs[rng.Next() % wall_prefabs.Count], new Vector3(x, 0, y), Quaternion.identity, transform));
        //         else
        //             tiles.Add(Instantiate(tile_prefabs[rng.Next() % tile_prefabs.Count], new Vector3(x, 0, y), Quaternion.identity, transform));
        //     }
        // }
    }
}

[Flags] public enum Direction2D { North, East, South, West }
[Flags] public enum Direction3D { Up_x, Down_x, Up_y, Down_y, Up_z, Down_z }

public class Utils
{
    public static readonly Vector3Int[] dirs_3D = {
        new Vector3Int( 1,  0,  0),
        new Vector3Int(-1,  0,  0),
        new Vector3Int( 0,  1,  0),
        new Vector3Int( 0, -1,  0),
        new Vector3Int( 0,  0,  1),
        new Vector3Int( 0,  0, -1)
    };

    public static readonly Vector2Int[] dirs_2D = {
        new Vector2Int( 1,  0),
        new Vector2Int( 0,  1),
        new Vector2Int(-1,  0),
        new Vector2Int( 0, -1)
    };

    public static Vector2Int DirVec2D(Direction2D dir) { return dirs_2D[(int)dir]; }
    public static Vector2Int DirVec2D(int dir) { return dirs_2D[dir]; }
    public static Vector3Int DirVec3D(Direction3D dir) { return dirs_3D[(int)dir]; }
    public static Vector3Int DirVec3D(int dir) { return dirs_3D[dir]; }
}
