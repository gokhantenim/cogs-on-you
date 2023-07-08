using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public Transform TargetPosition;
    Cogs _cogs;

    [SerializeField] EnemyUI ui;

    void Awake()
    {
        _cogs = GetComponent<Cogs>();
    }

    public void Die()
    {
        Destroy(gameObject);
        if (_cogs == null) return;
        _cogs.Spill();
    }
}
