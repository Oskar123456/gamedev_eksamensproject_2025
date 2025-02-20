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

    List<GameObject> walls = new List<GameObject>();
    List<GameObject> tiles = new List<GameObject>();
    List<GameObject> decos = new List<GameObject>();

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
        foreach (GameObject go in walls) Destroy(go); walls.Clear();
        foreach (GameObject go in tiles) Destroy(go); tiles.Clear();
        foreach (GameObject go in decos) Destroy(go); decos.Clear();
    }

    void GenMaze()
    {
        maze = new Maze(maze_width, maze_height);

        for (int y = 0; y < maze.height; y++) {
            for (int x = 0; x < maze.width; x++) {
                if (maze.walls[y, x])
                    walls.Add(Instantiate(wall_prefabs[rng.Next() % wall_prefabs.Count], new Vector3(x, 0, y), Quaternion.identity, transform));
                else
                    tiles.Add(Instantiate(tile_prefabs[rng.Next() % tile_prefabs.Count], new Vector3(x, 0, y), Quaternion.identity, transform));
            }
        }
    }
}



class MazeCell { public bool[] paths = new bool[4]; }

class Maze
{
    private System.Random rng = new System.Random();

    private MazeCell[,] cells;
    private Vector2Int _start, _finish;
    /* expose dims*/
    public Vector2Int start, finish;
    public int width, height;
    public bool[,] walls;

    public Maze(int width, int height)
    {
        this.width = width * 2 + 1;
        this.height = height * 2 + 1;

        walls = new bool[this.height, this.width];
        cells = new MazeCell[height, width];
        bool[,] vis = new bool[height, width];
        for (int i = 0; i < height; i++)
            for (int j = 0; j < width; j++)
                cells[i, j] = new MazeCell();
        for (int i = 0; i < this.height; i++)
            for (int j = 0; j < this.width; j++)
                walls[i, j] = true;

        Stack<Vector2Int> stack = new Stack<Vector2Int>();

        int start_side = rng.Next() % 4;
        int finish_side = ((rng.Next() % 3) + 1 + start_side) % 4;

        if (start_side == 0) {
            _start = new Vector2Int((rng.Next() % (width - 2)) + 1, 0);
        } if (start_side == 1) {
            _start = new Vector2Int(width - 1, (rng.Next() % (height - 2)) + 1);
        } if (start_side == 2) {
            _start = new Vector2Int((rng.Next() % (width - 2)) + 1, height - 1);
        } if (start_side == 3) {
            _start = new Vector2Int(0, (rng.Next() % (height - 2)) + 1);
        } if (finish_side == 0) {
            _finish = new Vector2Int((rng.Next() % (width - 2)) + 1, 0);
        } if (finish_side == 1) {
            _finish = new Vector2Int(width - 1, (rng.Next() % (height - 2)) + 1);
        } if (finish_side == 2) {
            _finish = new Vector2Int((rng.Next() % (width - 2)) + 1, height - 1);
        } if (finish_side == 3) {
            _finish = new Vector2Int(0, (rng.Next() % (height - 2)) + 1);
        }

        stack.Push(_start);
        int n_vis = 1;
        while (n_vis < width * height) {
            Vector2Int p = stack.Peek();
            vis[p.y, p.x] = true;
            walls[p.y * 2 + 1, p.x * 2 + 1] = false;

            int i, r = rng.Next() % 4;
            for (i = 0; i < 4; ++i) {
                int i_dir = (i + r) % 4;
                Vector2Int dir = Utils.DirVec2D(i_dir);
                Vector2Int p_nxt = p + dir;
                if (p_nxt.x < 0 || p_nxt.x >= width || p_nxt.y < 0 || p_nxt.y >= height || vis[p_nxt.y, p_nxt.x])
                    continue;
                vis[p_nxt.y, p_nxt.x] = true;
                walls[p_nxt.y * 2 + 1, p_nxt.x * 2 + 1] = false;
                walls[p.y * 2 + 1 + dir.y, p.x * 2 + 1 + dir.x] = false;
                cells[p.y, p.x].paths[i_dir] = true;
                cells[p_nxt.y, p_nxt.x].paths[((i_dir) + 2) % 4] = true;
                stack.Push(p_nxt);
                n_vis++;
                break;
            }

            if (i == 4)
                stack.Pop();
        }

        start = new Vector2Int(_start.x * 2 + 1, _start.y * 2 + 1);
        finish = new Vector2Int(_finish.x * 2 + 1, _finish.y * 2 + 1);
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
