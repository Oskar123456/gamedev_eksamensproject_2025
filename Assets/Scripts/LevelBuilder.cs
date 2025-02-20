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

public class LevelBuilder : MonoBehaviour
{
    public List<GameObject> medieval_wall_prefabs;
    public List<GameObject> medieval_tile_prefabs;
    public List<GameObject> medieval_lamp_prefabs;

    public int medieval_level_height;
    public int medieval_wall_width;
    public int medieval_wall_variance;
    public int medieval_avg_room_width;
    public int medieval_avg_room_height;
    public int medieval_avg_room_variance;

    private System.Random rng = new System.Random();

    public void Medieval(Maze maze, Transform parent)
    {
        Level level = new Level(maze, medieval_level_height, parent);

        for (int x = 0; x < maze.width; x++)
            level.row_heights[x] = medieval_avg_room_height + (rng.Next() % medieval_avg_room_variance) * (rng.Next() % 2 == 0 ? 1 : -1);
        for (int y = 0; y < maze.height; y++)
            level.column_widths[y]  = medieval_avg_room_width  + (rng.Next() % medieval_avg_room_variance) * (rng.Next() % 2 == 0 ? 1 : -1);

        level.InitVoxels();

        for (int x = 0; x < level.voxel_width; x++) {
            for (int y = 0; y < level.level_voxel_height; y++) {
                for (int z = 0; z < level.voxel_height; z++) {
                    int x_idx = 0, z_idx = 0;
                    int x_sum = level.column_widths[0], z_sum = level.row_heights[0];
                    while (x > x_sum) x_sum += level.column_widths[++x_idx];
                    while (z > z_sum) z_sum += level.row_heights[++z_idx];
                    if (level.maze.walls[z_idx, x_idx]) {
                        level.voxels[x, y, z] = Voxel.Wall;
                        Instantiate(medieval_wall_prefabs[0], new Vector3(x, y, z), Quaternion.identity, parent);
                    } else if (y <= level.level_voxel_height / 2) {
                        level.voxels[x, y, z] = Voxel.Floor;
                        Instantiate(medieval_tile_prefabs[0], new Vector3(x, y, z), Quaternion.identity, parent);
                    } else {
                        level.voxels[x, y, z] = Voxel.Air;
                    }
                }
            }
        }
    }

    private static float MultiLayerNoise(float x, float z)
    {
        float total = 0;
        float freq = 0.04f;
        float amp = 1.0f;
        float amp_sum = 0;
        float per = 0.5f;
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

public enum Voxel { None, Air, Wall, Floor }

public class Level
{
    public Transform parent_trf;

    public Maze maze;
    public Voxel[,,] voxels;
    public int[] column_widths, row_heights;
    public int voxel_width = 0, voxel_height = 0, level_voxel_height = 0;

    public void InitVoxels()
    {
        for (int x = 0; x < maze.width; x++)
            voxel_width += column_widths[x];
        for (int y = 0; y < maze.height; y++)
            voxel_height += row_heights[y];

        voxels = new Voxel[voxel_width, level_voxel_height, voxel_height];
    }

    public Level(Maze maze, int level_voxel_height, Transform parent)
    {
        parent_trf = parent;
        this.maze = maze;
        row_heights   = new int[maze.height];
        column_widths = new int[maze.width];
        this.level_voxel_height = level_voxel_height;
    }
}































