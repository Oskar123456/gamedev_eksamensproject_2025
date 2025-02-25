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

public class MazeCell { public bool[] paths = new bool[4]; }

public class Maze
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

        if (_start.x == height - 1) {
            start.x += 1;
        }
        if (_start.x == 0) {
            start.x -= 1;
        }
        if (_finish.x == height - 1) {
            finish.x += 1;
        }
        if (_finish.x == 0) {
            finish.x -= 1;
        }

        if (_start.y == width - 1) {
            start.y += 1;
        }
        if (_start.y == 0) {
            start.y -= 1;
        }
        if (_finish.y == width - 1) {
            finish.y += 1;
        }
        if (_finish.y == 0) {
            finish.y -= 1;
        }

        walls[start.y, start.x] = false;
        walls[finish.y, finish.x] = false;

        Debug.Log(_start.ToString() + " --> " + _finish.ToString());
        Debug.Log(start.ToString() + " --> " + finish.ToString());

    }
}


