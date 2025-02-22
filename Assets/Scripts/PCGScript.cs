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
    public GameObject level_container_prefab;
    public List<GameObject> medieval_deco_prefabs;
    public List<GameObject> medieval_light_prefabs;
    public List<GameObject> medieval_enemy_prefabs;

    public int maze_width, maze_height;

    LevelBuilder level_builder;

    List<GameObject> walls = new List<GameObject>();
    List<GameObject> tiles = new List<GameObject>();
    List<GameObject> decos = new List<GameObject>();
    GameObject level_container;

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
        player_stats = GameState.player_stats;

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
        Destroy(level_container);
    }

    public Level New(LevelType level_type)
    {
        Clean();
        level_container = Instantiate(level_container_prefab, Vector3.zero, Quaternion.identity, transform);
        maze = new Maze(maze_width, maze_height);
        Level level = level_builder.Medieval(maze, level_container.GetComponent<Transform>());

        Decorate(level, level_type);

        level_container.GetComponent<NavMeshSurface>().BuildNavMesh();
        return level;
    }

    void Decorate(Level level, LevelType level_type)
    {
        int count = 0;
        for (int x = 0; x < level.width; x++) {
            for (int z = 0; z < level.height; z++) {
                if (level.maze.walls[z, x])
                    continue;

                Vector2Int voxel_offs = level.MapCellToVoxelOffset(x, z);
                int n_decos = Utils.rng.Next() % ((int)Math.Sqrt(level.column_widths[x] * level.row_heights[z]));

                Vector3 deco_pos = Vector3.zero;
                RaycastHit hit_info;


                if (count < 128 && z < level.height - 1 && level.maze.walls[z + 1, x]) {
                    Vector3 pos = level.MapCellVoxelToWorld(x, z, level.column_widths[x] / 2, level.row_heights[z] / 2);
                    pos.y = level.level_voxel_height * (LevelBuilder.voxel_scale / 2.0f) * 0.75f;
                    if (Physics.Raycast(pos, Vector3.forward, out hit_info, level.row_heights[z] * LevelBuilder.voxel_scale)) {
                        deco_pos = hit_info.point;

                        GameObject new_deco = Instantiate(medieval_light_prefabs[Utils.rng.Next() % medieval_light_prefabs.Count],
                                deco_pos, Quaternion.Euler(0, 180, 0), level_container.transform);

                        count++;
                    }
                }

                if (count < 128 && x < level.width - 1 && level.maze.walls[z, x + 1]) {
                    Vector3 pos = level.MapCellVoxelToWorld(x, z, level.column_widths[x] / 2, level.row_heights[z] / 2);
                    pos.y = level.level_voxel_height * (LevelBuilder.voxel_scale / 2.0f) * 0.75f;
                    if (Physics.Raycast(pos, Vector3.right, out hit_info, level.column_widths[x] * LevelBuilder.voxel_scale)) {
                        deco_pos = hit_info.point;

                        GameObject new_deco = Instantiate(medieval_light_prefabs[Utils.rng.Next() % medieval_light_prefabs.Count],
                                deco_pos, Quaternion.Euler(0, 270, 0), level_container.transform);
                        count++;
                    }
                }

                if ((x == maze.start.x && z == maze.start.y) || (x == maze.finish.x && z == maze.finish.y))
                    continue;

                for (int i = 0; i < n_decos; i++) {
                    int cell_x = Utils.rng.Next() % level.column_widths[x];
                    int cell_z = Utils.rng.Next() % level.row_heights[z];
                    if (level.occupied[cell_x + voxel_offs.x, cell_z + voxel_offs.y])
                        continue;
                    deco_pos = level.MapCellVoxelToWorld(x, z, cell_x, cell_z);

                    if (Physics.Raycast(deco_pos + Vector3.up * 100, Vector3.down, out hit_info, 200)) {
                        deco_pos = hit_info.point;

                        GameObject deco  = medieval_deco_prefabs[Utils.rng.Next() % medieval_deco_prefabs.Count];
                        GameObject new_deco = Instantiate(deco, deco_pos, Quaternion.identity, level_container.transform);

                        new_deco.transform.localScale = new Vector3(LevelBuilder.voxel_scale, LevelBuilder.voxel_scale, LevelBuilder.voxel_scale);
                    }
                }

            }
        }
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
