using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class LevelManager : MonoBehaviour
{
    public static LevelManager _instance;
    public static LevelManager Instance
    {
        get {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelManager>(true);
            }
            return _instance;
        }
    }
    public LevelDefinition LoadedLevel;
    [SerializeField] NavMeshSurface _navigation;
    string _objectsContainerName = "LevelObjectsContainer";
    GameObject _objectsContainer;
    int _totalEnemies = 0;
    int _remainingEnemies = 0;

    private void Awake()
    {
        
    }

    Vector3 FlyDistance()
    {
#if UNITY_EDITOR
        if (LevelEditorWindow.IsEditScene() && !Application.isPlaying)
        {
            return Vector3.zero;
        }
#endif
        return new Vector3(0, PlayerController.Instance.FlyingDistance);
    }

    public void LoadLevel(LevelDefinition level)
    {
        LoadedLevel = level;
        ResetLevelObjects();

        if(PlayerController.Instance != null)
        {
            Vector3 playerPosition = LoadedLevel.PlayerStartPosition != null ? LoadedLevel.PlayerStartPosition : Vector3.zero;
            PlayerController.Instance.transform.position = playerPosition + FlyDistance();
        }

        InstantiateLevelObjects();
        _navigation.BuildNavMesh();
        NavMeshAgent[] navMeshAgents = FindObjectsOfType<NavMeshAgent>();
        foreach (NavMeshAgent agent in navMeshAgents)
        {
            agent.enabled = true;
        }

        Enemy[] enemies = FindObjectsOfType<Enemy>();
        _totalEnemies = enemies.Length;
        SetRemainingEnemies(_totalEnemies);
    }

    public void SetRemainingEnemies(int amount)
    {
        _remainingEnemies = amount;
        GamePlayUI.Instance.SetEnemyCounts(amount, _totalEnemies);
    }

    public void EnemyDied()
    {
        SetRemainingEnemies(_remainingEnemies-1);
        if(_remainingEnemies == 0) GameManager.Instance.LevelCompleted();
    }

    void InstantiateLevelObjects()
    {
        Transform levelParent = _objectsContainer.transform;
        foreach (LevelObjectDefinition levelObjectDefinition in LoadedLevel.LevelObjects)
        {
            GameObject levelObjectGameObject = Application.isPlaying ?
                Instantiate(levelObjectDefinition.ObjectPrefab) :
                (GameObject) PrefabUtility.InstantiatePrefab(levelObjectDefinition.ObjectPrefab);

            levelObjectGameObject.transform.SetParent(levelParent);
            levelObjectGameObject.transform.position = levelObjectDefinition.Position;
            levelObjectGameObject.transform.rotation = Quaternion.Euler(levelObjectDefinition.EulerAngles);
            levelObjectGameObject.transform.localScale = levelObjectDefinition.Scale;

            Damagable damagable = levelObjectGameObject.GetComponent<Damagable>();
            if (damagable != null && levelObjectDefinition.Health > 0)
            {
                damagable.Health = levelObjectDefinition.Health;
            }
            Cogs cogs = levelObjectGameObject.GetComponent<Cogs>();
            if (cogs != null && levelObjectDefinition.TotalCogs > 0)
            {
                cogs.TotalCogs = levelObjectDefinition.TotalCogs;
            }
        }
    }

    void DestroyLevelObjects()
    {
        Transform objectsContainerTransform = transform.Find(_objectsContainerName);
        if (objectsContainerTransform == null) return;
        _objectsContainer = objectsContainerTransform.gameObject;
        if (_objectsContainer == null) return;

        if (Application.isEditor)
        {
            DestroyImmediate(_objectsContainer);
        }
        else
        {
            Destroy(_objectsContainer);
        }
    }

    void ResetLevelObjects()
    {
        DestroyLevelObjects();
        _objectsContainer = new GameObject();
        _objectsContainer.name = _objectsContainerName;
        _objectsContainer.transform.parent = transform;
    }
}
