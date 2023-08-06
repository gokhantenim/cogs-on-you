using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Create Level Definition")]
public partial class LevelDefinition : ScriptableObject
{
    /// <summary>
    /// The start position of the player for this level
    /// </summary>
    public Vector3 PlayerStartPosition = Vector3.zero;
    public Vector3 PlayerStartRotation = Vector3.zero;
    /// <summary>
    /// An array of all SpawnableObjects that exist in this level.
    /// </summary>
    public LevelObjectDefinition[] LevelObjects;
}
