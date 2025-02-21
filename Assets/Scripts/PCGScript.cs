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

    public int maze_width, maze_height;

    LevelBuilder level_builder;

    List<GameObject> walls = new List<GameObject>();
    List<GameObject> tiles = new List<GameObject>();
    List<GameObject> decos = new List<GameObject>();
    GameObject level_go;

    NavMeshSurface nms;
    GameObject player;
    Transform player_trf;
    PlayerStats player_stats;

    System.Random rng = new System.Random();
    public int level, difficulty;
    Maze maze;

    void Awake()
    {
        nms = GetComponent<NavMeshSurface>();
        player = GameObject.Find("Player");
        player_trf = player.GetComponent<Transform>();
        player_stats = player.GetComponent<PlayerStats>();

        level_builder = GetComponent<LevelBuilder>();
    }

    void Start()
    {
    }

    void Update()
    {
    }

    void Clean()
    {
        Destroy(level_go);
    }

    public Level New()
    {
        Clean();
        level_go = Instantiate(level_prefab, Vector3.zero, Quaternion.identity, transform);
        maze = new Maze(maze_width, maze_height);
        Level level = level_builder.Medieval(maze, level_go.GetComponent<Transform>());
        level_go.GetComponent<NavMeshSurface>().BuildNavMesh();
        return level;
    }
}

[Flags] public enum Direction2D { North, East, South, West }
[Flags] public enum Direction3D { North, East, South, West, Up, Down }

public class Utils
{
    public static System.Random rng = new System.Random();

    public static readonly Vector3Int[] dirs_3D = {
        new Vector3Int( 0,  0,  1),
        new Vector3Int( 1,  0,  0),
        new Vector3Int( 0,  0,  -1),
        new Vector3Int( -1, 0,  0),
        new Vector3Int( 0,  1,  0),
        new Vector3Int( 0,  -1, 0)
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

    public static float MultiLayerNoise(float x, float z)
    {
        float total = 0;
        float freq = 0.09f;
        float amp = 1.0f;
        float amp_sum = 0;
        float per = 0.6f;
        int oct = 4;

        for (int i = 0; i < oct; ++i) {
            total += (float)(Mathf.PerlinNoise(x * freq, z * freq) * amp);
            amp_sum += amp;
            amp *= per;
            freq *= 2;
        }

        return (float)Math.Clamp(Math.Pow(total / amp_sum, 2), 0, 1);
    }
}
