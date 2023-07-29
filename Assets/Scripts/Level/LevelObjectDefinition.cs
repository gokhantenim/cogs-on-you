using System;
using UnityEngine;

[Serializable]
public class LevelObjectDefinition
{
    /// <summary>
    /// The prefab spawned by this LevelObject.
    /// </summary>
    public GameObject ObjectPrefab;

    /// <summary>
    /// The world position of this LevelObject.
    /// </summary>
    public Vector3 Position = Vector3.zero;

    /// <summary>
    /// The rotational euler angles of this LevelObject.
    /// </summary>
    public Vector3 EulerAngles = Vector3.zero;

    /// <summary>
    /// The world scale of this LevelObject.
    /// </summary>
    public Vector3 Scale = Vector3.one;

    /// <summary>
    /// The health of this LevelObject if it has Damagable.
    /// </summary>
    public float Health = 0;
    /// <summary>
    /// The cogs count this LevelObject if it has Cogs.
    /// </summary>
    public int TotalCogs = 0;
}