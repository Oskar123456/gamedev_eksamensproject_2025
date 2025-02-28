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

using UnityEngine;

public class HitInfo
{
    public Vector3 normal;
    public GameObject parent;
    public int damage;
    public DamageType damage_type;

    public HitInfo(Vector3 n, GameObject p, int d, DamageType t)
    {
        normal = n;
        parent = p;
        damage = d;
        damage_type = t;
    }
}
