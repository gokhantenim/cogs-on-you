using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditorWindow : EditorWindow
{
    public static LevelDefinition SelectedLevel = null;
    LevelDefinition _loadedLevelDefinition = null;
    const string _levelEditorSceneName = "EditLevel";
    const string _levelEditorScenePath = "Assets/Scenes/EditLevel.unity";

    [MenuItem("Window/Level Editor")]
    static void OpenWindow()
    {
        LevelEditorWindow window = (LevelEditorWindow)GetWindow(typeof(LevelEditorWindow), false, "Level Editor");
        window.Show();
    }

    void OnFocus()
    {
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorApplication.playModeStateChanged += OnPlayModeChanged;

        EditorSceneManager.sceneSaved -= OnSceneSaved;
        EditorSceneManager.sceneSaved += OnSceneSaved;
    }

    void OnDestroy()
    {
        //SceneView.duringSceneGui -= OnSceneGUI;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;
        EditorSceneManager.sceneSaved -= OnSceneSaved;
    }

    void OnGUI()
    {
        if (Application.isPlaying)
        {
            GUILayout.Label("Exit play mode to continue editing level.");
            return;
        }

        if (!IsEditScene())
        {
            if (GUILayout.Button("Open Level Editor Scene"))
            {
                EditorSceneManager.OpenScene(_levelEditorScenePath);
                if (SelectedLevel != null)
                {
                    LoadLevel();
                }
            }
            return;
        }

        if (LevelManager.Instance == null)
        {
            GUILayout.Label("Add a LevelManager to begin.");
            return;
        }
        SelectedLevel = (LevelDefinition)EditorGUILayout.ObjectField("Level Definition", SelectedLevel, typeof(LevelDefinition), false, null);

        if (SelectedLevel == null)
        {
            GUILayout.Label("Select a LevelDefinition ScriptableObject to begin.");
            return;
        }

        // auto load level
        if(!SelectedLevel.Equals(_loadedLevelDefinition))
        {
            LoadLevel();
        }

        if (GUILayout.Button("Reload Level"))
        {
            LoadLevel();
        }

        if (GUILayout.Button("Save Level"))
        {
            SaveLevel();
        }
    }

    void SaveLevel()
    {
        LevelObject[] levelObjects = FindObjectsOfType<LevelObject>();
        List<LevelObjectDefinition> levelObjectDefinitions = new();
        foreach (LevelObject levelObject in levelObjects)
        {
            LevelObjectDefinition levelObjectDefinition = new LevelObjectDefinition()
            {
                ObjectPrefab = PrefabUtility.GetCorrespondingObjectFromOriginalSource(levelObject.gameObject),
                Scale = levelObject.transform.lossyScale,
                Position = levelObject.transform.position,
                EulerAngles = levelObject.transform.rotation.eulerAngles,
            };
            Damagable damagable = levelObject.GetComponent<Damagable>();
            if (damagable != null)
            {
                levelObjectDefinition.Health = damagable.Health;
            }
            Cogs cogs = levelObject.GetComponent<Cogs>();
            if (cogs != null)
            {
                levelObjectDefinition.TotalCogs = cogs.TotalCogs;
            }
            levelObjectDefinitions.Add(levelObjectDefinition);
        }
        SelectedLevel.LevelObjects = levelObjectDefinitions.ToArray();
        
        if(PlayerController.Instance != null)
        {
            SelectedLevel.PlayerStartPosition = PlayerController.Instance.transform.position;
            SelectedLevel.PlayerStartRotation = PlayerController.Instance.transform.rotation.eulerAngles;
        }

        // Set level definition dirty so the changes will be written to disk
        EditorUtility.SetDirty(SelectedLevel);

        // Write changes to disk
        AssetDatabase.SaveAssets();
    }

    void LoadLevel()
    {
        LevelManager.Instance.LoadLevel(SelectedLevel);
        _loadedLevelDefinition = SelectedLevel;
    }

    void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (!IsEditScene() || SelectedLevel == null) return;
        if (state != PlayModeStateChange.ExitingEditMode) return;
        SaveLevel();
    }

    void OnSceneSaved(Scene scene)
    {
        if (!IsEditScene() || SelectedLevel == null) return;
        SaveLevel();
    }

    public static bool IsEditScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        return scene.name.Equals(_levelEditorSceneName);
    }
}
