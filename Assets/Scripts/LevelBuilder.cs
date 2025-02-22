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
    public float scale = 4;
    public static float voxel_scale = 4;
    public float floor_noise_scale = 1.2f;
    public static float noise_scale = 1.2f;

    public GameObject black_plane;

    public GameObject medieval_wall_skeleton;
    public GameObject medieval_floor_skeleton;
    public GameObject medieval_ceiling_skeleton;

    public int medieval_level_height = 5;
    public int medieval_wall_width = 5;
    public int medieval_wall_variance = 2;
    public int medieval_avg_room_width = 5;
    public int medieval_avg_room_height = 5;
    public int medieval_avg_room_variance = 2;

    private System.Random rng = new System.Random();

    public void Init()
    {
        voxel_scale = scale;
    }

    public Level Medieval(Maze maze, Transform parent)
    {
        Level level = new Level(maze, medieval_level_height, parent);

        for (int x = 0; x < maze.width; x++)
            level.column_widths[x] = medieval_avg_room_width + (rng.Next() % medieval_avg_room_variance) * (rng.Next() % 2 == 0 ? 1 : -1);
        for (int y = 0; y < maze.height; y++)
            level.row_heights[y]  = medieval_avg_room_height  + (rng.Next() % medieval_avg_room_variance) * (rng.Next() % 2 == 0 ? 1 : -1);

        level.InitVoxels();
        level.BuildMesh();

        List<GameObject> floors = new List<GameObject>();
        List<GameObject> walls = new List<GameObject>();
        List<GameObject> ceilings = new List<GameObject>();

        for (int i = 0; i < level.floor_vertices.Count; i++) {
            GameObject floor = Instantiate(medieval_floor_skeleton, Vector3.zero, Quaternion.identity, parent);
            SetMesh(floor, level.floor_vertices[i], level.floor_triangles[i], level.floor_uvs[i]);
            floors.Add(floor);
        }
        for (int i = 0; i < level.wall_vertices.Count; i++) {
            GameObject wall = Instantiate(medieval_wall_skeleton, Vector3.zero, Quaternion.identity, parent);
            SetMesh(wall, level.wall_vertices[i], level.wall_triangles[i], level.wall_uvs[i]);
            walls.Add(wall);
        }
        for (int i = 0; i < level.ceiling_vertices.Count; i++) {
            GameObject ceiling = Instantiate(medieval_ceiling_skeleton, Vector3.zero, Quaternion.identity, parent);
            SetMesh(ceiling, level.ceiling_vertices[i], level.ceiling_triangles[i], level.ceiling_uvs[i]);
            ceilings.Add(ceiling);
        }

        return level;
    }

    void SetMesh(GameObject skeleton, List<Vector3> vertices, List<int> triangles, List<Vector2> uvs)
    {
        MeshFilter mesh_filter = skeleton.GetComponent<MeshFilter>();
        Mesh mesh = skeleton.GetComponent<MeshFilter>().mesh;
        MeshCollider mesh_collider = skeleton.GetComponent<MeshCollider>();
        NavMeshSurface nav_mesh_surface = skeleton.GetComponent<NavMeshSurface>();

        mesh.Clear();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        Mesh mesh_for_collider = new Mesh();
        mesh_for_collider.vertices = vertices.ToArray();
        mesh_for_collider.triangles = triangles.ToArray();
        mesh_for_collider.RecalculateBounds();
        mesh_for_collider.RecalculateNormals();
        mesh_collider.sharedMesh = mesh_for_collider;
    }
}

public enum Voxel { None, Air, Wall, Floor, Ceiling }

public class Level
{
    public Transform parent_trf;

    public Maze maze;
    public Voxel[,,] voxels;
    public Voxel[,,] voxels_with_boundary;
    public int[] column_widths, row_heights;
    public int voxel_width = 0, voxel_height = 0, level_voxel_height = 0;
    public float world_width = 0, world_height = 0;
    public float[,] noise_levels;
    public bool[,] occupied;

    public List<List<Vector3>> floor_vertices = new List<List<Vector3>>();
    public List<List<int>> floor_triangles = new List<List<int>>();
    public List<List<Vector2>> floor_uvs = new List<List<Vector2>>();

    public List<List<Vector3>> wall_vertices = new List<List<Vector3>>();
    public List<List<int>> wall_triangles = new List<List<int>>();
    public List<List<Vector2>> wall_uvs = new List<List<Vector2>>();

    public List<List<Vector3>> ceiling_vertices = new List<List<Vector3>>();
    public List<List<int>> ceiling_triangles = new List<List<int>>();
    public List<List<Vector2>> ceiling_uvs = new List<List<Vector2>>();

    int boundary_width = 3;
    int max_vertex_data = (1 << 16) - 1;
    int[] indices = { 0, 1, 2, 0, 2, 3 };
    Vector2[] uv = { new Vector2Int(0, 0), new Vector2Int(0, 1), new Vector2Int(1, 1), new Vector2Int(1, 0) };

    public Level(Maze maze, int level_voxel_height, Transform parent)
    {
        parent_trf = parent;
        this.maze = maze;
        row_heights   = new int[maze.height];
        column_widths = new int[maze.width];
        this.level_voxel_height = level_voxel_height;
    }

    public void InitVoxels()
    {
        for (int x = 0; x < maze.width; x++)
            voxel_width += column_widths[x];
        for (int y = 0; y < maze.height; y++)
            voxel_height += row_heights[y];

        world_width = voxel_width * LevelBuilder.voxel_scale;
        world_height = voxel_height * LevelBuilder.voxel_scale;

        voxels = new Voxel[voxel_width, level_voxel_height, voxel_height];
        voxels_with_boundary = new Voxel[voxel_width + boundary_width * 2, level_voxel_height, voxel_height + boundary_width * 2];
        noise_levels = new float[voxel_width, voxel_height];
        occupied = new bool[voxel_width, voxel_height];

        for (int x = -boundary_width; x < voxel_width + boundary_width; x++) {
            for (int y = 0; y < level_voxel_height; y++) {
                for (int z = -boundary_width; z < voxel_height + boundary_width; z++) {
                    if (x < 0 || x >= voxel_width || z < 0 || z >= voxel_height) {
                        if (y == level_voxel_height - 1)
                            voxels_with_boundary[x + boundary_width, y, z + boundary_width] = Voxel.Ceiling;
                        else
                            voxels_with_boundary[x + boundary_width, y, z + boundary_width] = Voxel.Wall;
                        continue;
                    }

                    if (noise_levels[x, z] == 0)
                        noise_levels[x, z] = Utils.MultiLayerNoise(x + 0.015f, z + 0.013f) * LevelBuilder.noise_scale;

                    if (!IsWall(x, z) && y == 0) {
                        voxels[x, y, z] = Voxel.Floor;
                        voxels_with_boundary[x + boundary_width, y, z + boundary_width] = Voxel.Floor;
                    }
                    else if (IsWall(x, z)) {
                        occupied[x, z] = true;
                        if (y == level_voxel_height - 1) {
                            voxels_with_boundary[x + boundary_width, y, z + boundary_width] = Voxel.Ceiling;
                            voxels[x, y, z] = Voxel.Ceiling;
                        }
                        else {
                            voxels_with_boundary[x + boundary_width, y, z + boundary_width] = Voxel.Wall;
                            voxels[x, y, z] = Voxel.Wall;
                        }
                    } else {
                        voxels[x, y, z] = Voxel.Air;
                        voxels_with_boundary[x + boundary_width, y, z + boundary_width] = Voxel.Air;
                    }
                }
            }
        }
    }

    public void MarkOccupiedPosition(int voxel_x, int voxel_z) { occupied[voxel_x, voxel_z] = true; }
    public void UnMarkOccupiedPosition(int voxel_x, int voxel_z) { occupied[voxel_x, voxel_z] = false; }

    public Vector3 GetRandomUnoccupiedPosition()
    {
        int count = 0;
        while (true) {
            int x = Utils.rng.Next() % voxel_width;
            int z = Utils.rng.Next() % voxel_height;
            if (count++ > 1000 || !occupied[x, z]) {
                if (count > 1000)
                    Debug.Log("GetRandomUnoccupiedPosition over 1000 iterations");
                return GetVoxelWorldCenter(x, z);
            }
        }
        return Vector3.zero;
    }

    public Vector3 GetVoxelWorldCenter(int x, int z)
    {
        return new Vector3(x * LevelBuilder.voxel_scale + LevelBuilder.voxel_scale / 2.0f,
                noise_levels[x, z], z * LevelBuilder.voxel_scale + LevelBuilder.voxel_scale / 2.0f);
    }

    public Vector2Int MapWorldPosToVoxel(float x, float z)
    {
        return new Vector2Int((int)Math.Floor(x / LevelBuilder.voxel_scale), (int)Math.Floor(z / LevelBuilder.voxel_scale));
    }

    public Vector3 GetStartPosition()
    {
        Vector2Int start_cell_voxel_off = MapCellToVoxelOffset(maze.start.x, maze.start.y);
        return MapCellVoxelToWorld(maze.start.x, maze.start.y, column_widths[maze.start.x] / 2, row_heights[maze.start.y] / 2);
    }

    public Vector3 GetFinishPosition()
    {
        Vector2Int finish_cell_voxel_off = MapCellToVoxelOffset(maze.finish.x, maze.finish.y);
        return MapCellVoxelToWorld(maze.finish.x, maze.finish.y, column_widths[maze.finish.x] / 2, row_heights[maze.finish.y] / 2);
    }

    public bool IsWall(int x, int z)
    {
        if (x >= voxel_width || z >= voxel_height)
            return false;

        int x_idx = 0, z_idx = 0;
        int x_sum = column_widths[0], z_sum = row_heights[0];
        while (x >= x_sum && x_idx < maze.width - 1) x_sum += column_widths[++x_idx];
        while (z >= z_sum && z_idx < maze.height - 1) z_sum += row_heights[++z_idx];

        if (x < 0 || z < 0 || x_idx >= maze.width || z_idx >= maze.height)
            return false;
        return maze.walls[z_idx, x_idx];
    }

    public void GetMazeIdx(int x, int z, out int maze_x, out int maze_z)
    {
        int x_idx = 0, z_idx = 0;
        int x_sum = column_widths[0], z_sum = row_heights[0];
        while (x >= x_sum && x_idx < maze.width - 1) x_sum += column_widths[++x_idx];
        while (z >= z_sum && z_idx < maze.height - 1) z_sum += row_heights[++z_idx];
        maze_x = x_idx; maze_z = z_idx;
    }

    public Vector2Int MapCellToVoxelOffset(int x, int z)
    {
        int x_off = 0, z_off = 0;
        for (int i = 0; i < x; i++) x_off += column_widths[i];
        for (int i = 0; i < z; i++) z_off += row_heights[i];
        return new Vector2Int(x_off, z_off);
    }

    public Vector2 MapCellToWorldOffset(int x, int z)
    {
        int x_off = 0, z_off = 0;
        for (int i = 0; i < x; i++) x_off += column_widths[i];
        for (int i = 0; i < z; i++) z_off += row_heights[i];
        return new Vector2(x_off * LevelBuilder.voxel_scale + LevelBuilder.voxel_scale / 2.0f, z_off * LevelBuilder.voxel_scale + LevelBuilder.voxel_scale / 2.0f);
    }

    public Vector3 MapCellVoxelToWorld(int cell_x, int cell_z, int voxel_x, int voxel_z)
    {
        Vector2 cell_pos = MapCellToWorldOffset(cell_x, cell_z);
        return new Vector3(cell_pos.x + voxel_x * LevelBuilder.voxel_scale,
                noise_levels[cell_x + voxel_x, cell_z + voxel_z] + LevelBuilder.voxel_scale / 2.0f,
                cell_pos.y + voxel_z * LevelBuilder.voxel_scale);
    }

    public void BuildMesh()
    {
        for (int z = -boundary_width; z < voxel_height + boundary_width; z++) {
            for (int y = 0; y < level_voxel_height; y++) {
                for (int x = -boundary_width; x < voxel_width + boundary_width; x++) {
                    if (voxels_with_boundary[x + boundary_width, y, z + boundary_width] == Voxel.None || voxels_with_boundary[x + boundary_width, y, z + boundary_width] == Voxel.Air)
                        continue;
                    if (voxels_with_boundary[x + boundary_width, y, z + boundary_width] == Voxel.Floor) {
                        AddCubeQuads(x, y + noise_levels[x + boundary_width, z + boundary_width], z, 1, 1 / LevelBuilder.voxel_scale, floor_vertices, floor_triangles, floor_uvs, true);
                    }
                    if (voxels_with_boundary[x + boundary_width, y, z + boundary_width] == Voxel.Wall) {
                        if (IsCubeVisible(x + boundary_width, y, z + boundary_width))
                            AddCubeQuads(x, y, z, 1, 1, wall_vertices, wall_triangles, wall_uvs, false);
                    }
                    if (voxels_with_boundary[x + boundary_width, y, z + boundary_width] == Voxel.Ceiling) {
                        AddCubeQuads(x, y, z, 1, 0.01f / LevelBuilder.voxel_scale, ceiling_vertices, ceiling_triangles, ceiling_uvs, false);
                    }
                }
            }
        }
        Debug.Log("wall counts: " + wall_vertices.Count);
        Debug.Log("floor counts: " + floor_vertices.Count);
    }

    bool IsCubeVisible(int x, int y, int z)
    {
        Vector3Int p = new Vector3Int(x, y, z);
        foreach (Direction3D dir in Direction3D.GetValues(typeof(Direction3D))) {
            Vector3Int p_n = p + Utils.DirVec3D(dir);
            if (p_n.x < 0 || p_n.x >= voxel_width + boundary_width * 2 || p_n.y < 0 || p_n.y >= level_voxel_height || p_n.z < 0 || p_n.z >= voxel_height + boundary_width * 2) {
                return true;
            }
            if (voxels_with_boundary[p_n.x, p_n.y, p_n.z] == Voxel.Air || voxels_with_boundary[p_n.x, p_n.y, p_n.z] == Voxel.None) {
                return true;
            }
        }
        return false;
    }

    bool IsQuadVisible(int x, int y, int z, Direction3D dir)
    {
        Vector3Int p = new Vector3Int(x, y, z);
        Vector3Int p_n = p + Utils.DirVec3D(dir);
        if (p_n.x < -boundary_width || p_n.x >= voxel_width + 1 * boundary_width || p_n.y < 0 || p_n.y >= level_voxel_height || p_n.z < -boundary_width || p_n.z >= voxel_height + 1 * boundary_width) {
            return true;
        }
        // Debug.Log(p_n.ToString() + " / " + (voxel_width + 2 * boundary_width).ToString() + "," + (voxel_height + 2 * boundary_width).ToString());
        if (voxels_with_boundary[p_n.x + boundary_width, p_n.y, p_n.z + boundary_width] == Voxel.Air
                || voxels_with_boundary[p_n.x + boundary_width, p_n.y, p_n.z + boundary_width] == Voxel.None
                || voxels_with_boundary[p_n.x + boundary_width, p_n.y, p_n.z + boundary_width] == Voxel.Floor) {
            return true;
        }
        return false;
    }

    void AddCubeQuads(float x, float y, float z, float w, float h,
            List<List<Vector3>> vertices, List<List<int>> triangles, List<List<Vector2>> uvs,
            bool force_sides)
    {
        if (vertices.Count < 1 || vertices[vertices.Count - 1].Count >= max_vertex_data - 72) {
            vertices.Add(new List<Vector3>());
            triangles.Add(new List<int>());
            uvs.Add(new List<Vector2>());
        }

        w *= LevelBuilder.voxel_scale;
        h *= LevelBuilder.voxel_scale;

        float x_scaled = x * LevelBuilder.voxel_scale;
        float y_scaled = y * LevelBuilder.voxel_scale;
        float z_scaled = z * LevelBuilder.voxel_scale;

        Vector3[] top = {
            new Vector3(x_scaled, y_scaled + h, z_scaled),
            new Vector3(x_scaled, y_scaled + h, z_scaled + w),
            new Vector3(x_scaled + w, y_scaled + h, z_scaled + w),
            new Vector3(x_scaled + w, y_scaled + h, z_scaled),
        };

        Vector3[] left = {
            new Vector3(x_scaled, y_scaled, z_scaled + w),
            new Vector3(x_scaled, y_scaled + h, z_scaled + w),
            new Vector3(x_scaled, y_scaled + h, z_scaled),
            new Vector3(x_scaled, y_scaled, z_scaled),
        };

        Vector3[] right = {
            new Vector3(x_scaled + w, y_scaled, z_scaled),
            new Vector3(x_scaled + w, y_scaled + h, z_scaled),
            new Vector3(x_scaled + w, y_scaled + h, z_scaled + w),
            new Vector3(x_scaled + w, y_scaled, z_scaled + w),
        };

        Vector3[] front = {
            new Vector3(x_scaled, y_scaled, z_scaled),
            new Vector3(x_scaled, y_scaled + h, z_scaled),
            new Vector3(x_scaled + w, y_scaled + h, z_scaled),
            new Vector3(x_scaled + w, y_scaled, z_scaled),
        };

        Vector3[] back = {
            new Vector3(x_scaled + w, y_scaled, z_scaled + w),
            new Vector3(x_scaled + w, y_scaled + h, z_scaled + w),
            new Vector3(x_scaled, y_scaled + h, z_scaled + w),
            new Vector3(x_scaled, y_scaled, z_scaled + w),
        };

        if (IsQuadVisible((int)Math.Floor(x), (int)Math.Floor(y), (int)Math.Floor(z), Direction3D.Up)) {
            triangles[triangles.Count - 1].InsertRange(0, indices.Select(i => i + vertices[vertices.Count - 1].Count));
            vertices[vertices.Count - 1].InsertRange(0, top);
            uvs[uvs.Count - 1].InsertRange(0, uv);
        }
        if (force_sides || IsQuadVisible((int)Math.Floor(x), (int)Math.Floor(y), (int)Math.Floor(z), Direction3D.North)) {
            triangles[triangles.Count - 1].InsertRange(0, indices.Select(i => i + vertices[vertices.Count - 1].Count));
            vertices[vertices.Count - 1].InsertRange(0, back);
            uvs[uvs.Count - 1].InsertRange(0, uv);
        }
        if (force_sides || IsQuadVisible((int)Math.Floor(x), (int)Math.Floor(y), (int)Math.Floor(z), Direction3D.South)) {
            triangles[triangles.Count - 1].InsertRange(0, indices.Select(i => i + vertices[vertices.Count - 1].Count));
            vertices[vertices.Count - 1].InsertRange(0, front);
            uvs[uvs.Count - 1].InsertRange(0, uv);
        }
        if (force_sides || IsQuadVisible((int)Math.Floor(x), (int)Math.Floor(y), (int)Math.Floor(z), Direction3D.West)) {
            triangles[triangles.Count - 1].InsertRange(0, indices.Select(i => i + vertices[vertices.Count - 1].Count));
            vertices[vertices.Count - 1].InsertRange(0, left);
            uvs[uvs.Count - 1].InsertRange(0, uv);
        }
        if (force_sides || IsQuadVisible((int)Math.Floor(x), (int)Math.Floor(y), (int)Math.Floor(z), Direction3D.East)) {
            triangles[triangles.Count - 1].InsertRange(0, indices.Select(i => i + vertices[vertices.Count - 1].Count));
            vertices[vertices.Count - 1].InsertRange(0, right);
            uvs[uvs.Count - 1].InsertRange(0, uv);
        }
    }
}
















