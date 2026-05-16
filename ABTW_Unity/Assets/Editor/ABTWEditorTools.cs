using UnityEngine;
using UnityEditor;
using System.IO;
using ABTW.Core;
using ABTW.Systems;
using ABTW.Anomalies;

namespace ABTW.Editor
{
    /// <summary>
    /// ABTW Editor Tools — Unity Editor menu extensions.
    /// Provides quick scene setup, anomaly spawning, and debug utilities.
    /// Access via: Unity menu → ABTW → ...
    /// </summary>
    public class ABTWEditorTools : EditorWindow
    {
        // ── Menu Items ───────────────────────────────────────────────────────

        [MenuItem("ABTW/Setup/Create Chapter 1 Scene Structure")]
        public static void CreateChapter1Structure()
        {
            // Root
            var root = new GameObject("=== ABTW Chapter 1 ===");

            // Environment
            var env = CreateChild(root, "Environment");
            CreateCorridor(env);

            // Systems
            var systems = CreateChild(root, "Systems");
            AddComponent<GameManager>(systems);
            AddComponent<PerceptionSystem>(systems);
            AddComponent<NotebookSystem>(systems);
            AddComponent<GlitchSystem>(systems);
            AddComponent<EchoSystem>(systems);
            AddComponent<MemoryWalkSystem>(systems);
            AddComponent<TrioSystem>(systems);
            AddComponent<UIManager>(systems);
            AddComponent<InputHandler>(systems);
            AddComponent<ChapterOneDirector>(systems);

            // Player
            var player = CreateChild(root, "Player_Astra");
            player.tag = "Player";
            player.AddComponent<CharacterController>();
            player.AddComponent<PlayerMovement>();
            player.AddComponent<PlayerStats>();

            // Camera
            var camGO = new GameObject("Main Camera");
            camGO.tag = "MainCamera";
            camGO.AddComponent<Camera>();
            camGO.AddComponent<AudioListener>();
            camGO.AddComponent<CameraController>();
            camGO.transform.SetParent(root.transform);

            // Companions
            var companions = CreateChild(root, "Companions");
            var lumi = CreateChild(companions, "Lumi");
            lumi.AddComponent<LumiCompanion>();
            var nela = CreateChild(companions, "Nela");
            nela.AddComponent<NelaCompanion>();

            // Anomalies
            var anomalies = CreateChild(root, "Anomalies");
            SpawnAnomaly<ShadowDelayAnomaly>(anomalies, "ShadowDelay_01", new Vector3(3f, 0f, 5f));
            SpawnAnomaly<LightInstabilityAnomaly>(anomalies, "LightInstability_01", new Vector3(-2f, 0f, 8f));
            SpawnAnomaly<SymbolFragmentAnomaly>(anomalies, "SymbolFragment_01", new Vector3(0f, 1.5f, 12f));

            // Lighting
            var lighting = CreateChild(root, "Lighting");
            var dirLight = new GameObject("Directional Light");
            dirLight.transform.SetParent(lighting.transform);
            var light = dirLight.AddComponent<Light>();
            light.type      = LightType.Directional;
            light.intensity = 0.85f;
            light.color     = new Color(0.95f, 0.92f, 0.88f);
            dirLight.transform.rotation = Quaternion.Euler(45f, -30f, 0f);

            Selection.activeGameObject = root;
            Debug.Log("[ABTW] Chapter 1 scene structure created.");
        }

        [MenuItem("ABTW/Setup/Create Folder Structure")]
        public static void CreateFolderStructure()
        {
            string[] folders = {
                "Assets/Scripts/Core",
                "Assets/Scripts/Systems",
                "Assets/Scripts/Anomalies",
                "Assets/Scripts/Companions",
                "Assets/Scripts/UI",
                "Assets/Editor",
                "Assets/Scenes",
                "Assets/Resources/Prefabs",
                "Assets/Resources/Materials",
                "Assets/Resources/Audio",
                "Assets/Resources/Fonts",
                "Assets/Resources/Textures",
                "Assets/Animations",
                "Assets/Settings"
            };

            foreach (var folder in folders)
            {
                if (!AssetDatabase.IsValidFolder(folder))
                {
                    var parts  = folder.Split('/');
                    var parent = string.Join("/", parts, 0, parts.Length - 1);
                    AssetDatabase.CreateFolder(parent, parts[parts.Length - 1]);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log("[ABTW] Folder structure created.");
        }

        [MenuItem("ABTW/Debug/Log All Anomalies in Scene")]
        public static void LogAllAnomalies()
        {
            var anomalies = FindObjectsOfType<AnomalyBase>();
            Debug.Log($"[ABTW] Found {anomalies.Length} anomalies in scene:");
            foreach (var a in anomalies)
                Debug.Log($"  → {a.name} | Type: {a.anomalyType} | Layer: {a.anomalyLayer} | Active: {a.gameObject.activeSelf}");
        }

        [MenuItem("ABTW/Debug/Reset All Anomalies")]
        public static void ResetAllAnomalies()
        {
            var anomalies = FindObjectsOfType<AnomalyBase>(true);
            foreach (var a in anomalies)
                a.gameObject.SetActive(true);
            Debug.Log($"[ABTW] Reset {anomalies.Length} anomalies.");
        }

        [MenuItem("ABTW/Debug/Simulate Trio Activation")]
        public static void SimulateTrioActivation()
        {
            var trio = FindObjectOfType<TrioSystem>();
            if (trio != null)
            {
                trio.OnLumiReady();
                trio.OnNelaReady();
                Debug.Log("[ABTW] Trio activation simulated.");
            }
            else
            {
                Debug.LogWarning("[ABTW] No TrioSystem found in scene.");
            }
        }

        [MenuItem("ABTW/Debug/Add Max EXP to Player")]
        public static void AddMaxEXP()
        {
            var stats = FindObjectOfType<PlayerStats>();
            if (stats != null)
            {
                stats.AddEXP(9999);
                Debug.Log("[ABTW] Added 9999 EXP to player.");
            }
            else
            {
                Debug.LogWarning("[ABTW] No PlayerStats found in scene.");
            }
        }

        [MenuItem("ABTW/Debug/Toggle Perception Mode")]
        public static void TogglePerceptionMode()
        {
            var perception = FindObjectOfType<PerceptionSystem>();
            if (perception != null)
            {
                perception.TogglePerceptionMode();
                Debug.Log("[ABTW] Perception mode toggled.");
            }
        }

        // ── Editor Window ────────────────────────────────────────────────────

        [MenuItem("ABTW/Open ABTW Dashboard")]
        public static void OpenDashboard()
        {
            var window = GetWindow<ABTWEditorTools>("ABTW Dashboard");
            window.minSize = new Vector2(380f, 480f);
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Space(10);
            EditorGUILayout.LabelField("ACADEMY BEYOND THIS WORLD", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("Development Dashboard", EditorStyles.miniLabel);
            GUILayout.Space(10);

            // Scene Setup
            EditorGUILayout.LabelField("── SCENE SETUP ──", EditorStyles.boldLabel);
            if (GUILayout.Button("Create Chapter 1 Scene Structure"))
                CreateChapter1Structure();
            if (GUILayout.Button("Create Folder Structure"))
                CreateFolderStructure();

            GUILayout.Space(8);

            // Debug
            EditorGUILayout.LabelField("── DEBUG TOOLS ──", EditorStyles.boldLabel);
            if (GUILayout.Button("Log All Anomalies"))
                LogAllAnomalies();
            if (GUILayout.Button("Reset All Anomalies"))
                ResetAllAnomalies();
            if (GUILayout.Button("Simulate Trio Activation"))
                SimulateTrioActivation();
            if (GUILayout.Button("Add Max EXP"))
                AddMaxEXP();
            if (GUILayout.Button("Toggle Perception Mode"))
                TogglePerceptionMode();

            GUILayout.Space(8);

            // Info
            EditorGUILayout.LabelField("── PROJECT INFO ──", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "ABTW — Academy Beyond This World\n" +
                "Genre: Narrative 3D Adventure\n" +
                "Core: Perception-based gameplay\n" +
                "Unity Version: 2022.3 LTS+\n\n" +
                "Core Rule:\n" +
                "\"Reality responds to how it is seen.\"",
                MessageType.Info);
        }

        // ── Helpers ──────────────────────────────────────────────────────────

        private static GameObject CreateChild(GameObject parent, string name)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform);
            go.transform.localPosition = Vector3.zero;
            return go;
        }

        private static void AddComponent<T>(GameObject go) where T : Component
        {
            if (go.GetComponent<T>() == null)
                go.AddComponent<T>();
        }

        private static void SpawnAnomaly<T>(GameObject parent, string name, Vector3 pos)
            where T : AnomalyBase
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent.transform);
            go.transform.position = pos;
            go.AddComponent<T>();
        }

        private static void CreateCorridor(GameObject parent)
        {
            // Floor
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            floor.name = "Floor";
            floor.transform.SetParent(parent.transform);
            floor.transform.localScale    = new Vector3(6f, 0.2f, 30f);
            floor.transform.localPosition = new Vector3(0f, -0.1f, 10f);

            // Ceiling
            var ceiling = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ceiling.name = "Ceiling";
            ceiling.transform.SetParent(parent.transform);
            ceiling.transform.localScale    = new Vector3(6f, 0.2f, 30f);
            ceiling.transform.localPosition = new Vector3(0f, 3.6f, 10f);

            // Left Wall
            var wallL = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallL.name = "Wall_Left";
            wallL.transform.SetParent(parent.transform);
            wallL.transform.localScale    = new Vector3(0.2f, 3.8f, 30f);
            wallL.transform.localPosition = new Vector3(-3f, 1.8f, 10f);

            // Right Wall
            var wallR = GameObject.CreatePrimitive(PrimitiveType.Cube);
            wallR.name = "Wall_Right";
            wallR.transform.SetParent(parent.transform);
            wallR.transform.localScale    = new Vector3(0.2f, 3.8f, 30f);
            wallR.transform.localPosition = new Vector3(3f, 1.8f, 10f);
        }
    }
}