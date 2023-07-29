using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Create Gun Definition", fileName = "GunDefinition")]
public class GunDefinition : ScriptableObject
{
    public string Name;
    public GameObject GunPrefab;
    public GunLevel[] Levels;

    [Serializable]
    public class GunLevel
    {
        public float Damage = 0;
        public float FireRate = 0;
        public float MagazineCapacity = 0;
        public float ReloadTime = 0;
        public int Cost = 0;
    }
}
