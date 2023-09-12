using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class LevelManager : AbstractSingleton<LevelManager>
{
    public LevelDefinition LoadedLevel;
    [SerializeField] NavMeshSurface[] _navigations;
    string _objectsContainerName = "LevelObjectsContainer";
    GameObject _objectsContainer;
    int _totalEnemies = 0;
    int _remainingEnemies = 0;

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
            Vector3 playerRotation = LoadedLevel.PlayerStartRotation != null ? LoadedLevel.PlayerStartRotation : Vector3.zero;
            PlayerController.Instance.transform.position = playerPosition + FlyDistance();
            PlayerController.Instance.transform.rotation = Quaternion.Euler(playerRotation);
        }

        InstantiateLevelObjects();
        foreach (NavMeshSurface navigation in _navigations)
        {
            navigation.BuildNavMesh();
        }
        NavMeshAgent[] navMeshAgents = FindObjectsOfType<NavMeshAgent>();
        foreach (NavMeshAgent agent in navMeshAgents)
        {
            agent.enabled = true;
        }

        StartCoroutine(CountEnemiesAfterDestroys());
    }

    IEnumerator CountEnemiesAfterDestroys()
    {
        yield return new WaitForEndOfFrame();
        CountEnemies();
    }

    void CountEnemies()
    {
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        _totalEnemies = enemies.Length;
        SetRemainingEnemies(_totalEnemies);
    }

    public void SetRemainingEnemies(int amount)
    {
        if (GamePlayUI.Instance == null) return;
        _remainingEnemies = amount;
        GamePlayUI.Instance.SetEnemyCounts(amount, _totalEnemies);
    }

    public void EnemyDied()
    {
        SetRemainingEnemies(_remainingEnemies-1);
        if(_remainingEnemies <= 0) GameManager.Instance.LevelCompleted();
    }

    void InstantiateLevelObjects()
    {
        Transform levelParent = _objectsContainer.transform;
        foreach (LevelObjectDefinition levelObjectDefinition in LoadedLevel.LevelObjects)
        {
#if UNITY_EDITOR
            GameObject levelObjectGameObject = Application.isPlaying ?
                Instantiate(levelObjectDefinition.ObjectPrefab) :
                (GameObject) PrefabUtility.InstantiatePrefab(levelObjectDefinition.ObjectPrefab);
#else
            GameObject levelObjectGameObject = Instantiate(levelObjectDefinition.ObjectPrefab);
#endif
            levelObjectGameObject.transform.SetParent(levelParent);
            levelObjectGameObject.transform.position = levelObjectDefinition.Position;
            levelObjectGameObject.transform.rotation = Quaternion.Euler(levelObjectDefinition.EulerAngles);
            levelObjectGameObject.transform.localScale = levelObjectDefinition.Scale;

            Damagable damagable = levelObjectGameObject.GetComponent<Damagable>();
            if (damagable != null && levelObjectDefinition.Health > 0)
            {
                damagable.SetHealth(levelObjectDefinition.Health, true);
            }
            Cogs cogs = levelObjectGameObject.GetComponent<Cogs>();
            if (cogs != null && levelObjectDefinition.TotalCogs > 0)
            {
                cogs.TotalCogs = levelObjectDefinition.TotalCogs;
            }
        }
    }

    public void ClearLevel()
    {
        DestroyContainer();

        // if exists any level object out of the container
        LevelObject[] levelObjects = FindObjectsOfType<LevelObject>();
        foreach (LevelObject levelObject in levelObjects)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(levelObject.gameObject);
            }
            else
            {
                Destroy(levelObject.gameObject);
            }
        }
    }

    void DestroyContainer()
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
        ClearLevel();
        _objectsContainer = new GameObject();
        _objectsContainer.name = _objectsContainerName;
        _objectsContainer.transform.parent = transform;
    }
}
