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

using System;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

namespace PCG
{
    public class PCGScript : MonoBehaviour
    {
        public GameObject level_container_prefab;
        public List<GameObject> medieval_pillar_prefabs;
        public List<GameObject> medieval_deco_prefabs;
        public List<GameObject> medieval_deco_large_prefabs;
        public List<GameObject> medieval_light_prefabs;
        public List<GameObject> medieval_enemy_prefabs;
        public GameObject water_volume;
        public GameObject water_tile;
        public GameObject water_arch;
        public GameObject water_wall;

        public int maze_width, maze_height;

        LevelBuilder level_builder;

        List<GameObject> walls = new List<GameObject>();
        List<GameObject> tiles = new List<GameObject>();
        List<GameObject> decos = new List<GameObject>();
        GameObject level_container;

        GameObject player;
        Transform player_trf;
        PlayerStats player_stats;

        int max_lights = 128, light_count = 0;
        System.Random rng = new System.Random();
        public int level, difficulty;
        Maze maze;

        void Awake()
        {
            level_builder = GetComponent<LevelBuilder>();
        }

        void Start() { }

        void Update() { }

        void Clean()
        {
            Destroy(level_container);
        }

        public Level New(LevelType level_type)
        {
            player = GameObject.Find("Player");
            player_trf = player.GetComponent<Transform>();

            Clean();
            level_container = Instantiate(level_container_prefab, Vector3.zero, Quaternion.identity, transform);
            maze = new Maze(maze_width + (GameState.rng.Next() % (int)MathF.Sqrt(GameState.level + 1)),
                    maze_height + (GameState.rng.Next() % (int)MathF.Sqrt(GameState.level + 1)));
            Level level = level_builder.Medieval(maze, level_container.GetComponent<Transform>(), level_type);

            level_container.GetComponent<NavMeshSurface>().BuildNavMesh();

            if (level_type == LevelType.Water) {
                AddWater(level, level_type);
            }
            if (level_type != LevelType.Water) {
                DecorateLarge(level, level_type);
            }
            AddPillars(level, level_type);
            Decorate(level, level_type);
            AddEnemies(level, level_type);

            return level;
        }

        void AddWater(Level level, LevelType level_type)
        {
            for (int x = 0; x < (level.voxel_width * LevelBuilder.voxel_scale + 49) / 50; x++) {
                for (int z = 0; z < (level.voxel_height * LevelBuilder.voxel_scale + 49) / 50; z++) {
                    GameObject wv = Instantiate(water_volume, new Vector3(x * 50, LevelBuilder.voxel_scale - 1, z * 50),
                        Quaternion.identity, level_container.GetComponent<Transform>());

                    for (int i = 0; i < 25; i++) {
                        wv = Instantiate(water_arch, new Vector3((x + 1) * 50 - 0.5f, LevelBuilder.voxel_scale - 3, z * 50 + 2 * i),
                            Quaternion.Euler(0, 90, 0), level_container.GetComponent<Transform>());
                        wv = Instantiate(water_arch, new Vector3(x * 50 + 2 * i, LevelBuilder.voxel_scale - 3, (z + 1) * 50 - 0.5f),
                            Quaternion.identity, level_container.GetComponent<Transform>());
                    }
                }
            }
        }

        void AddPillars(Level level, LevelType level_type)
        {
            for (int x = 0; x < level.width; x++) {
                for (int z = 0; z < level.height; z++) {
                    Vector2Int voxel_offs = level.MapCellToVoxelOffset(x, z);
                    int n_enemies = Utils.rng.Next() % ((int)Math.Sqrt(level.column_widths[x] * level.row_heights[z]));

                    Vector3 pos = Vector3.zero;
                    RaycastHit hit_info;

                    if (level_type == LevelType.Water && level.maze.walls[z, x]) {
                        int cell_x = (level.column_widths[x] / 2);
                        int cell_z = (level.row_heights[z] / 2);

                        level.occupied[cell_x + voxel_offs.x, cell_z + voxel_offs.y] = true;
                        pos = level.MapCellVoxelToWorld(x, z, cell_x, cell_z);
                        pos.y -= LevelBuilder.voxel_scale * 2;

                        GameObject pillar  = medieval_pillar_prefabs[Utils.rng.Next() % medieval_pillar_prefabs.Count];
                        GameObject new_pillar = Instantiate(pillar, pos, Quaternion.identity, level_container.transform);

                        new_pillar.transform.localScale = new Vector3(LevelBuilder.voxel_scale * 1.66f,
                            LevelBuilder.voxel_scale * 1.66f, LevelBuilder.voxel_scale * 1.66f);

                        Vector3 poss = new Vector3(pos.x - 1, pos.y + (level.level_voxel_height - 1) * LevelBuilder.voxel_scale / 2, pos.z);
                        Vector3 posw = new Vector3(pos.x, pos.y + (level.level_voxel_height - 1) * LevelBuilder.voxel_scale / 2, pos.z - 1);

                        if (light_count < max_lights && Utils.rng.Next() % 2 == 0) {
                            light_count++;
                            if (Utils.rng.Next() % 2 == 0) {
                                Instantiate(medieval_light_prefabs[Utils.rng.Next() % medieval_light_prefabs.Count],
                                    posw, Quaternion.Euler(0, 180, 0), level_container.transform);
                            } else {
                                Instantiate(medieval_light_prefabs[Utils.rng.Next() % medieval_light_prefabs.Count],
                                    poss, Quaternion.Euler(0, 270, 0), level_container.transform);
                            }
                        }
                    }

                    if (level.maze.walls[z, x])
                        continue;

                    for (int i = 0; i < 2; i++) {
                        for (int j = 0; j < 2; j++) {
                            int cell_x = (level.column_widths[x] / 4) * (1 + 2 * i);
                            int cell_z = (level.row_heights[z] / 4) * (1 + 2 * j);

                            level.occupied[cell_x + voxel_offs.x, cell_z + voxel_offs.y] = true;
                            pos = level.MapCellVoxelToWorld(x, z, cell_x, cell_z);

                            if (Physics.Raycast(pos + Vector3.up * 100, Vector3.down, out hit_info, 200)) {
                                pos = hit_info.point;

                                GameObject pillar  = medieval_pillar_prefabs[Utils.rng.Next() % medieval_pillar_prefabs.Count];
                                GameObject new_pillar = Instantiate(pillar, pos, Quaternion.identity, level_container.transform);

                                new_pillar.transform.localScale = new Vector3(LevelBuilder.voxel_scale * 1.66f,
                                    LevelBuilder.voxel_scale * 1.66f, LevelBuilder.voxel_scale * 1.66f);
                            }
                        }
                    }

                }
            }
        }

        void AddEnemies(Level level, LevelType level_type)
        {
            int enemy_target_num = (int)(MathF.Sqrt(GameState.level) * 100.0f);

            Debug.Log("enemy_target_num : " + enemy_target_num);

            Vector2Int noise_seeds = new Vector2Int(GameState.rng.Next(1000), GameState.rng.Next(1000));
            float[,] noise_array = new float[level.voxel_width, level.voxel_height];
            float[] noise_levels = new float[level.voxel_width * level.voxel_height];
            for (int x = 0; x < level.voxel_width; x++) {
                for (int z = 0; z < level.voxel_height; z++) {
                    if (level.IsWall(x, z)) {
                        noise_array[x, z] = 0;
                    } else {
                        noise_array[x, z] = Utils.MultiLayerNoise((x + noise_seeds.x) * 1.87f, (z + noise_seeds.y) * 1.87f);
                    }
                    noise_levels[x * level.voxel_height + z] = noise_array[x, z];
                }
            }

            Array.Sort(noise_levels);
            float noise_min = noise_levels[noise_levels.Length - enemy_target_num];
            int enemy_num = 0;

            for (int x = 0; x < level.width; x++) {
                for (int z = 0; z < level.height; z++) {
                    if (level.maze.walls[z, x])
                        continue;
                    if ((x == maze.start.x && z == maze.start.y) || (x == maze.finish.x && z == maze.finish.y))
                        continue;

                    Vector2Int voxel_offs = level.MapCellToVoxelOffset(x, z);
                    int n_enemies = Utils.rng.Next() % ((int)Math.Sqrt(level.column_widths[x] * level.row_heights[z])) + GameState.level;

                    Vector3 enemy_pos = Vector3.zero;
                    RaycastHit hit_info;

                    for (int cell_x = 0; cell_x < level.column_widths[x]; ++cell_x) {
                        for (int cell_z = 0; cell_z < level.row_heights[z]; ++cell_z) {
                            if (level.occupied[voxel_offs.x + cell_x, voxel_offs.y + cell_z]) {
                                continue;
                            }
                            if (noise_array[voxel_offs.x + cell_x, voxel_offs.y + cell_z] < noise_min) {
                                continue;
                            }

                            enemy_num++;

                            enemy_pos = level.MapCellVoxelToWorld(x, z, cell_x, cell_z);
                            if (Physics.Raycast(enemy_pos + Vector3.up * 100, Vector3.down, out hit_info, 200)) {
                                enemy_pos = hit_info.point;

                                GameObject enemy  = medieval_enemy_prefabs[Utils.rng.Next() % medieval_enemy_prefabs.Count];
                                GameObject new_enemy = Instantiate(enemy, enemy_pos, Quaternion.identity, level_container.transform);

                                level.occupied[voxel_offs.x + cell_x, voxel_offs.y + cell_z] = true;
                            }
                        }
                    }
                }
            }
            Debug.Log("Spawned " + enemy_num + " enemies");
        }

        void DecorateLarge(Level level, LevelType level_type)
        {
            int n = 0;
            for (int x = 0; x < level.width; x++) {
                for (int z = 0; z < level.height; z++) {
                    if (level.maze.walls[x, z]) {
                        continue;
                    }
                    if (Utils.rng.Next() % 4 == 0) {
                        continue;
                    }

                    Vector2Int cell_offs = level.MapCellToVoxelOffset(x, z);
                    Vector2Int voxel_coord = new Vector2Int((Utils.rng.Next() % (level.column_widths[x] - 2)) + 1, (Utils.rng.Next() % (level.row_heights[z] - 2)) + 1);
                    Vector3 pos = level.MapCellVoxelToWorld(x, z, voxel_coord.x, voxel_coord.y);
                    pos.x += LevelBuilder.voxel_scale / 2;
                    pos.y = LevelBuilder.voxel_scale * 2;
                    pos.z += LevelBuilder.voxel_scale / 2;

                    RaycastHit hit_info;
                    for (int dir = 0; dir < 4; dir++) {
                        // Vector2Int dir_vec = Utils.DirVec2D(dir);
                        float ray_length = (dir == 0 || dir == 2) ? level.row_heights[z] : level.column_widths[x];
                        Vector3 dir_vec = Utils.DirVec3DF(dir);
                        Vector3Int dir_vec_perp_i = Utils.DirVec3D((dir + 1) % 4);
                        if (Physics.Raycast(pos, dir_vec, out hit_info, ray_length * 1.1f * LevelBuilder.voxel_scale)) {
                            Debug.DrawRay(pos, dir_vec, Color.red, 1000);

                            int r = Utils.rng.Next(medieval_deco_large_prefabs.Count);
                            GameObject new_deco = Instantiate(medieval_deco_large_prefabs[r], hit_info.point,
                                    Quaternion.Euler(0, (dir + 1) * 90, 0), level_container.transform);

                            Vector3 p = hit_info.point;
                            new_deco.transform.position = new_deco.transform.position - dir_vec * (new_deco.transform.lossyScale.x * 1.5f);

                            Vector2Int point_voxel_coord = level.MapWorldPosToVoxel(new_deco.transform.position.x, new_deco.transform.position.z);
                            Vector2Int point_voxel_neighbor_coord = new Vector2Int(point_voxel_coord.x + dir_vec_perp_i.x, point_voxel_coord.y + dir_vec_perp_i.z);

                            float min_noise = MathF.Min(level.noise_levels[point_voxel_coord.x, point_voxel_coord.y],
                                    level.noise_levels[point_voxel_neighbor_coord.x, point_voxel_neighbor_coord.y]);

                            new_deco.transform.position = new Vector3(new_deco.transform.position.x,
                                    min_noise * LevelBuilder.voxel_scale + LevelBuilder.voxel_scale, new_deco.transform.position.z);

                            Debug.DrawRay(new Vector3(point_voxel_coord.x * LevelBuilder.voxel_scale, 0,
                                        point_voxel_coord.y * LevelBuilder.voxel_scale), Vector3.up * 10, Color.red, 1000);
                            Debug.DrawRay(new Vector3(point_voxel_neighbor_coord.x * LevelBuilder.voxel_scale, 0,
                                        point_voxel_neighbor_coord.y * LevelBuilder.voxel_scale), Vector3.up * 10, Color.red, 1000);

                            n++;

                            level.occupied[point_voxel_coord.x, point_voxel_coord.y] = true;
                            level.occupied[point_voxel_neighbor_coord.x, point_voxel_neighbor_coord.y] = true;

                            Debug.Log("New Large Deco at: " + hit_info.point.ToString() + " and marking: " + point_voxel_coord + " and " + point_voxel_neighbor_coord + " as occupied");
                            break;
                        }
                    }
                }
            }
            Debug.Log("Added " + n + " large decos");
        }

        void Decorate(Level level, LevelType level_type)
        {
            int deco_target_num = (int)MathF.Sqrt(level.voxel_width * level.voxel_height) * 2;

            Vector2Int noise_seeds = new Vector2Int(GameState.rng.Next(1000), GameState.rng.Next(1000));
            float[,] noise_array = new float[level.voxel_width, level.voxel_height];
            float[] noise_levels = new float[level.voxel_width * level.voxel_height];
            for (int x = 0; x < level.voxel_width; x++) {
                for (int z = 0; z < level.voxel_height; z++) {
                    if (level.IsWall(x, z)) {
                        noise_array[x, z] = 0;
                    } else {
                        noise_array[x, z] = Utils.MultiLayerNoise((x + noise_seeds.x) * 3.577f, (z + noise_seeds.y) * 3.577f);
                    }
                    noise_levels[x * level.voxel_height + z] = noise_array[x, z];
                }
            }

            Array.Sort(noise_levels);
            float noise_min = noise_levels[noise_levels.Length - deco_target_num];

            int deco_num = 0;
            for (int x = 0; x < level.width; x++) {
                for (int z = 0; z < level.height; z++) {
                    if (level.maze.walls[z, x])
                        continue;

                    Vector2Int voxel_offs = level.MapCellToVoxelOffset(x, z);
                    int n_decos = Utils.rng.Next() % ((int)Math.Sqrt(level.column_widths[x] * level.row_heights[z])) + 2;

                    Vector3 deco_pos = Vector3.zero;
                    RaycastHit hit_info;


                    if (light_count < max_lights && Utils.rng.Next() % 2 == 0 && z < level.height - 1 && level.maze.walls[z + 1, x]) {
                        Vector3 pos = level.MapCellVoxelToWorld(x, z, level.column_widths[x] / 2, level.row_heights[z] / 2);
                        pos.y = level.level_voxel_height * (LevelBuilder.voxel_scale / 2.0f) * 0.65f;
                        // Debug.DrawRay(pos, Vector3.forward * 3f * level.row_heights[z] * LevelBuilder.voxel_scale, Color.red, 1000);
                        // Debug.DrawRay(pos, Vector3.right * 3f * level.column_widths[x] * LevelBuilder.voxel_scale, Color.red, 1000);
                        if (Physics.Raycast(pos, Vector3.forward, out hit_info, 10f * level.row_heights[z] * LevelBuilder.voxel_scale)) {
                            deco_pos = hit_info.point;

                            GameObject new_deco = Instantiate(medieval_light_prefabs[Utils.rng.Next() % medieval_light_prefabs.Count],
                                    deco_pos, Quaternion.Euler(0, 180, 0), level_container.transform);

                            light_count++;
                        }
                    }

                    if (light_count < max_lights && Utils.rng.Next() % 2 == 0 && x < level.width - 1 && level.maze.walls[z, x + 1]) {
                        Vector3 pos = level.MapCellVoxelToWorld(x, z, level.column_widths[x] / 2, level.row_heights[z] / 2);
                        pos.y = level.level_voxel_height * (LevelBuilder.voxel_scale / 2.0f) * 0.65f;
                        if (Physics.Raycast(pos, Vector3.right, out hit_info, 10f * level.column_widths[x] * LevelBuilder.voxel_scale)) {
                            deco_pos = hit_info.point;

                            GameObject new_deco = Instantiate(medieval_light_prefabs[Utils.rng.Next() % medieval_light_prefabs.Count],
                                    deco_pos, Quaternion.Euler(0, 270, 0), level_container.transform);
                            light_count++;
                        }
                    }

                    if ((x == maze.start.x && z == maze.start.y) || (x == maze.finish.x && z == maze.finish.y))
                        continue;

                    for (int cell_x = 0; cell_x < level.column_widths[x]; ++cell_x) {
                        for (int cell_z = 0; cell_z < level.row_heights[z]; ++cell_z) {
                            if (level.occupied[cell_x + voxel_offs.x, cell_z + voxel_offs.y]) {
                                continue;
                            }
                            if (noise_array[voxel_offs.x + cell_x, voxel_offs.y + cell_z] < noise_min) {
                                continue;
                            }

                            level.occupied[cell_x + voxel_offs.x, cell_z + voxel_offs.y] = true;
                            deco_pos = level.MapCellVoxelToWorld(x, z, cell_x, cell_z);

                            if (Physics.Raycast(deco_pos + Vector3.up * 100, Vector3.down, out hit_info, 200)) {
                                deco_pos = hit_info.point;

                                GameObject deco  = medieval_deco_prefabs[Utils.rng.Next() % medieval_deco_prefabs.Count];
                                GameObject new_deco = Instantiate(deco, deco_pos + Vector3.up * 0.06f, Quaternion.identity, level_container.transform);

                                new_deco.transform.rotation = Quaternion.Euler(0, Utils.rng.Next(360), 0);
                                new_deco.transform.localScale = new_deco.transform.localScale * LevelBuilder.voxel_scale;

                                deco_num++;
                            }
                        }
                    }
                }
            }
            Debug.Log(deco_num + " decos added");
            Debug.Log(light_count + " lights added");
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
        public static Vector3 DirVec3DF(Direction3D dir) { Vector3Int d = dirs_3D[(int)dir]; return new Vector3(d.x, d.y, d.z); }
        public static Vector3 DirVec3DF(int dir) { Vector3Int d = dirs_3D[dir]; return new Vector3(d.x, d.y, d.z); }

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
}
