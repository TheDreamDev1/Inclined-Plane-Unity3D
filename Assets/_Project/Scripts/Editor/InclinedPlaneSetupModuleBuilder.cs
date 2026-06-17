using System.IO;
using OOPLab.Modules.InclinedPlaneSetup;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OOPLab.EditorTools
{
    public static class InclinedPlaneSetupModuleBuilder
    {
        private const string PrefabFolder = "Assets/_Project/Prefabs/Modules";
        private const string MaterialFolder = "Assets/_Project/Materials/InclinedPlaneSetup";

        [MenuItem("OOP Lab/01 Build Inclined Plane Setup Module")]
        public static void BuildModule()
        {
            EnsureFolders();
            DestroyIfExists("InclinedPlaneSetupModule");

            GameObject module = new GameObject("InclinedPlaneSetupModule");
            Transform rampPivot = CreateRamp(module.transform);
            Transform support = CreateBaseAndSupports(module.transform);
            CreateDistanceMarkers(module.transform);

            Canvas canvas = CreateCanvas(module.transform);
            UiRefs ui = CreatePopupUi(canvas.transform);

            InclinedPlaneController controller = module.AddComponent<InclinedPlaneController>();
            SerializedObject serialized = new SerializedObject(controller);
            Set(serialized, "rampPivot", rampPivot);
            Set(serialized, "heightSupport", support);
            Set(serialized, "controlPanel", ui.ControlPanel);
            Set(serialized, "closeButton", ui.CloseButton);
            Set(serialized, "angleSlider", ui.AngleSlider);
            Set(serialized, "angleInput", ui.AngleInput);
            Set(serialized, "angleMinusButton", ui.AngleMinusButton);
            Set(serialized, "anglePlusButton", ui.AnglePlusButton);
            Set(serialized, "resetButton", ui.ResetButton);
            Set(serialized, "angleText", ui.AngleText);
            Set(serialized, "heightText", ui.HeightText);
            Set(serialized, "statusText", ui.StatusText);
            serialized.ApplyModifiedProperties();

            foreach (InclinedPlaneClickTarget clickTarget in module.GetComponentsInChildren<InclinedPlaneClickTarget>(true))
            {
                clickTarget.SetController(controller);
                EditorUtility.SetDirty(clickTarget);
            }

            SetupCamera();
            SetupEventSystem();
            SavePrefab(module);
            Selection.activeGameObject = module;
            Debug.Log("Inclined Plane Setup module rebuilt. Click the ramp in Play Mode to open the UI.");
        }

        private static void EnsureFolders()
        {
            Directory.CreateDirectory("Assets/_Project");
            Directory.CreateDirectory(PrefabFolder);
            Directory.CreateDirectory(MaterialFolder);
            AssetDatabase.Refresh();
        }

        private static void DestroyIfExists(string objectName)
        {
            GameObject existing = GameObject.Find(objectName);
            if (existing != null)
            {
                Object.DestroyImmediate(existing);
            }
        }

        private static Transform CreateRamp(Transform parent)
        {
            GameObject pivot = new GameObject("RampPivot");
            pivot.transform.SetParent(parent, false);
            pivot.transform.position = new Vector3(-2.2f, 1.85f, 0f);

            GameObject ramp = GameObject.CreatePrimitive(PrimitiveType.Cube);
            ramp.name = "ClickableRampSurface";
            ramp.transform.SetParent(pivot.transform, false);
            ramp.transform.localPosition = new Vector3(2.25f, 0f, 0f);
            ramp.transform.localScale = new Vector3(4.5f, 0.18f, 1.35f);
            ramp.GetComponent<Renderer>().sharedMaterial = CreateMaterial("RampSurfaceMat", new Color(0.11f, 0.14f, 0.16f));
            ramp.AddComponent<InclinedPlaneClickTarget>();

            GameObject topStrip = GameObject.CreatePrimitive(PrimitiveType.Cube);
            topStrip.name = "RampBlueSurface";
            topStrip.transform.SetParent(ramp.transform, false);
            topStrip.transform.localPosition = new Vector3(0f, 0.58f, 0f);
            topStrip.transform.localScale = new Vector3(0.94f, 0.04f, 0.82f);
            topStrip.GetComponent<Renderer>().sharedMaterial = CreateMaterial("RampBlueMat", new Color(0.14f, 0.42f, 0.58f));
            Object.DestroyImmediate(topStrip.GetComponent<Collider>());

            CreateRail("LeftRail", ramp.transform, 0.62f);
            CreateRail("RightRail", ramp.transform, -0.62f);

            return pivot.transform;
        }

        private static void CreateRail(string name, Transform parent, float z)
        {
            GameObject rail = GameObject.CreatePrimitive(PrimitiveType.Cube);
            rail.name = name;
            rail.transform.SetParent(parent, false);
            rail.transform.localPosition = new Vector3(0f, 0.78f, z);
            rail.transform.localScale = new Vector3(1f, 0.42f, 0.07f);
            rail.GetComponent<Renderer>().sharedMaterial = CreateMaterial("RailMat", new Color(0.78f, 0.84f, 0.88f));
            Object.DestroyImmediate(rail.GetComponent<Collider>());
        }

        private static Transform CreateBaseAndSupports(Transform parent)
        {
            GameObject basePlate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            basePlate.name = "BasePlate";
            basePlate.transform.SetParent(parent, false);
            basePlate.transform.position = new Vector3(0f, -0.10f, 0f);
            basePlate.transform.localScale = new Vector3(5.8f, 0.06f, 2.35f);
            basePlate.GetComponent<Renderer>().sharedMaterial = CreateMaterial("BaseMat", new Color(0.18f, 0.18f, 0.18f));

            GameObject support = GameObject.CreatePrimitive(PrimitiveType.Cube);
            support.name = "AdjustableHeightSupport";
            support.transform.SetParent(parent, false);
            support.transform.position = new Vector3(-2.2f, 0.82f, -0.82f);
            support.transform.localScale = new Vector3(0.20f, 1.72f, 0.20f);
            support.GetComponent<Renderer>().sharedMaterial = CreateMaterial("SupportMat", new Color(0.92f, 0.65f, 0.18f));
            return support.transform;
        }

        private static void CreateDistanceMarkers(Transform parent)
        {
            for (int i = 0; i <= 5; i++)
            {
                GameObject tick = GameObject.CreatePrimitive(PrimitiveType.Cube);
                tick.name = "DistanceTick_" + i;
                tick.transform.SetParent(parent, false);
                tick.transform.position = new Vector3(-2.2f + i * 0.85f, -0.04f, 0.86f);
                tick.transform.localScale = new Vector3(0.04f, 0.04f, 0.26f);
                tick.GetComponent<Renderer>().sharedMaterial = CreateMaterial("TickMat", new Color(0.90f, 0.94f, 0.94f));
                Object.DestroyImmediate(tick.GetComponent<Collider>());
            }
        }

        private static Canvas CreateCanvas(Transform parent)
        {
            GameObject canvasObject = new GameObject("InclinedPlaneScreenCanvas");
            canvasObject.transform.SetParent(parent, false);

            Canvas canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
            return canvas;
        }

        private static UiRefs CreatePopupUi(Transform parent)
        {
            Font font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf") ?? Resources.GetBuiltinResource<Font>("Arial.ttf");
            GameObject panel = CreatePanel(parent, "InclinedPlanePopup", new Vector2(560f, 0f), new Vector2(560f, 650f), new Color(0.055f, 0.065f, 0.078f, 0.97f));

            Text title = CreateText(panel.transform, "Title", "Inclined Plane Setup", new Vector2(0f, 280f), new Vector2(470f, 42f), 30, TextAnchor.MiddleCenter, font);
            title.fontStyle = FontStyle.Bold;
            Text subtitle = CreateText(panel.transform, "Subtitle", "Click the plane to open. Adjust the ramp angle here.", new Vector2(0f, 244f), new Vector2(470f, 24f), 15, TextAnchor.MiddleCenter, font);
            subtitle.color = new Color(0.62f, 0.73f, 0.80f);

            Button close = CreateButton(panel.transform, "CloseButton", "X", new Vector2(236f, 280f), new Vector2(42f, 38f), new Color(0.62f, 0.14f, 0.20f), 20, font);

            Text angleText = CreateText(panel.transform, "AngleText", "Angle: 25.0 deg", new Vector2(0f, 170f), new Vector2(450f, 30f), 22, TextAnchor.MiddleCenter, font);
            Slider angleSlider = CreateSlider(panel.transform, "AngleSlider", new Vector2(0f, 112f), new Vector2(360f, 30f));
            InputField angleInput = CreateInput(panel.transform, "AngleInput", "25.0", new Vector2(0f, 52f), new Vector2(160f, 46f), font);
            Button angleMinus = CreateButton(panel.transform, "AngleMinusButton", "-", new Vector2(-144f, 52f), new Vector2(58f, 46f), new Color(0.13f, 0.28f, 0.43f), 28, font);
            Button anglePlus = CreateButton(panel.transform, "AnglePlusButton", "+", new Vector2(144f, 52f), new Vector2(58f, 46f), new Color(0.13f, 0.42f, 0.30f), 28, font);

            Text heightText = CreateText(panel.transform, "HeightText", "Raised height: 0.00 m", new Vector2(0f, -24f), new Vector2(450f, 28f), 18, TextAnchor.MiddleCenter, font);
            Text note = CreateText(panel.transform, "NoteText", "Angle range: 0 deg to 45 deg", new Vector2(0f, -66f), new Vector2(450f, 24f), 16, TextAnchor.MiddleCenter, font);
            note.color = new Color(0.62f, 0.73f, 0.80f);

            Button reset = CreateButton(panel.transform, "ResetButton", "Reset Angle", new Vector2(0f, -136f), new Vector2(150f, 48f), new Color(0.62f, 0.14f, 0.20f), 18, font);

            Text status = CreateText(panel.transform, "StatusText", "Ready. Click the plane to edit angle.", new Vector2(0f, -238f), new Vector2(470f, 28f), 17, TextAnchor.MiddleCenter, font);
            status.color = new Color(0.66f, 0.96f, 0.70f);

            return new UiRefs
            {
                ControlPanel = panel,
                CloseButton = close,
                AngleSlider = angleSlider,
                AngleInput = angleInput,
                AngleMinusButton = angleMinus,
                AnglePlusButton = anglePlus,
                ResetButton = reset,
                AngleText = angleText,
                HeightText = heightText,
                StatusText = status
            };
        }

        private static Slider CreateSlider(Transform parent, string name, Vector2 position, Vector2 size)
        {
            GameObject root = new GameObject(name, typeof(RectTransform));
            root.transform.SetParent(parent, false);
            RectTransform rect = root.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            GameObject background = CreatePanel(root.transform, "Background", Vector2.zero, new Vector2(size.x, 10f), new Color(0.15f, 0.18f, 0.20f, 1f));
            GameObject fillArea = new GameObject("Fill Area", typeof(RectTransform));
            fillArea.transform.SetParent(root.transform, false);
            RectTransform fillAreaRect = fillArea.GetComponent<RectTransform>();
            fillAreaRect.anchorMin = Vector2.zero;
            fillAreaRect.anchorMax = Vector2.one;
            fillAreaRect.offsetMin = new Vector2(8f, 0f);
            fillAreaRect.offsetMax = new Vector2(-8f, 0f);

            GameObject fill = CreatePanel(fillArea.transform, "Fill", Vector2.zero, new Vector2(size.x, 10f), new Color(0.04f, 0.58f, 0.78f, 1f));
            RectTransform fillRect = fill.GetComponent<RectTransform>();
            fillRect.anchorMin = Vector2.zero;
            fillRect.anchorMax = Vector2.one;
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;

            GameObject handleArea = new GameObject("Handle Slide Area", typeof(RectTransform));
            handleArea.transform.SetParent(root.transform, false);
            RectTransform handleAreaRect = handleArea.GetComponent<RectTransform>();
            handleAreaRect.anchorMin = Vector2.zero;
            handleAreaRect.anchorMax = Vector2.one;
            handleAreaRect.offsetMin = new Vector2(8f, 0f);
            handleAreaRect.offsetMax = new Vector2(-8f, 0f);

            GameObject handle = CreatePanel(handleArea.transform, "Handle", Vector2.zero, new Vector2(30f, 36f), new Color(0.93f, 0.97f, 0.98f, 1f));

            Slider slider = root.AddComponent<Slider>();
            slider.targetGraphic = handle.GetComponent<Image>();
            slider.fillRect = fillRect;
            slider.handleRect = handle.GetComponent<RectTransform>();
            slider.direction = Slider.Direction.LeftToRight;
            background.transform.SetSiblingIndex(0);
            return slider;
        }

        private static InputField CreateInput(Transform parent, string name, string value, Vector2 position, Vector2 size, Font font)
        {
            GameObject root = CreatePanel(parent, name, position, size, new Color(0.88f, 0.91f, 0.92f, 1f));
            InputField input = root.AddComponent<InputField>();
            input.targetGraphic = root.GetComponent<Image>();

            Text text = CreateText(root.transform, "Text", value, Vector2.zero, new Vector2(size.x - 18f, size.y - 8f), 19, TextAnchor.MiddleCenter, font);
            text.color = new Color(0.08f, 0.10f, 0.12f);

            Text placeholder = CreateText(root.transform, "Placeholder", value, Vector2.zero, new Vector2(size.x - 18f, size.y - 8f), 19, TextAnchor.MiddleCenter, font);
            placeholder.color = new Color(0.42f, 0.47f, 0.50f);
            placeholder.fontStyle = FontStyle.Italic;

            input.textComponent = text;
            input.placeholder = placeholder;
            input.contentType = InputField.ContentType.DecimalNumber;
            input.text = value;
            return input;
        }

        private static Button CreateButton(Transform parent, string name, string text, Vector2 position, Vector2 size, Color color, int fontSize, Font font)
        {
            GameObject buttonObject = CreatePanel(parent, name, position, size, color);
            Button button = buttonObject.AddComponent<Button>();
            button.targetGraphic = buttonObject.GetComponent<Image>();
            Text label = CreateText(buttonObject.transform, "Label", text, Vector2.zero, size, fontSize, TextAnchor.MiddleCenter, font);
            label.fontStyle = FontStyle.Bold;
            return button;
        }

        private static GameObject CreatePanel(Transform parent, string name, Vector2 position, Vector2 size, Color color)
        {
            GameObject panel = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            panel.transform.SetParent(parent, false);
            RectTransform rect = panel.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;
            Image image = panel.GetComponent<Image>();
            image.color = color;
            image.raycastTarget = true;
            return panel;
        }

        private static Text CreateText(Transform parent, string name, string text, Vector2 position, Vector2 size, int fontSize, TextAnchor alignment, Font font)
        {
            GameObject textObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
            textObject.transform.SetParent(parent, false);
            RectTransform rect = textObject.GetComponent<RectTransform>();
            rect.anchoredPosition = position;
            rect.sizeDelta = size;

            Text uiText = textObject.GetComponent<Text>();
            uiText.text = text;
            uiText.font = font;
            uiText.fontSize = fontSize;
            uiText.alignment = alignment;
            uiText.color = new Color(0.86f, 0.91f, 0.94f);
            uiText.raycastTarget = false;
            return uiText;
        }

        private static Material CreateMaterial(string name, Color color)
        {
            string path = MaterialFolder + "/" + name + ".mat";
            Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard"));
                material.color = color;
                AssetDatabase.CreateAsset(material, path);
            }

            return material;
        }

        private static void SetupCamera()
        {
            Camera camera = Camera.main;
            if (camera == null)
            {
                GameObject cameraObject = new GameObject("Main Camera");
                cameraObject.tag = "MainCamera";
                camera = cameraObject.AddComponent<Camera>();
                cameraObject.AddComponent<AudioListener>();
            }

            camera.transform.position = new Vector3(1.6f, 3.3f, -6.2f);
            camera.transform.rotation = Quaternion.Euler(29f, -11f, 0f);
            camera.fieldOfView = 50f;

            if (camera.GetComponent<PhysicsRaycaster>() == null)
            {
                camera.gameObject.AddComponent<PhysicsRaycaster>();
            }

            Light light = Object.FindFirstObjectByType<Light>();
            if (light == null)
            {
                GameObject lightObject = new GameObject("Directional Light");
                light = lightObject.AddComponent<Light>();
                light.type = LightType.Directional;
            }

            light.transform.rotation = Quaternion.Euler(50f, -35f, 0f);
            light.intensity = 1.15f;
        }

        private static void SetupEventSystem()
        {
            EventSystem existing = Object.FindFirstObjectByType<EventSystem>();
            if (existing != null)
            {
                Object.DestroyImmediate(existing.gameObject);
            }

            GameObject eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();

            System.Type inputSystemModule = System.Type.GetType("UnityEngine.InputSystem.UI.InputSystemUIInputModule, Unity.InputSystem");
            if (inputSystemModule != null)
            {
                eventSystemObject.AddComponent(inputSystemModule);
            }
            else
            {
                eventSystemObject.AddComponent<StandaloneInputModule>();
            }
        }

        private static void SavePrefab(GameObject module)
        {
            string prefabPath = PrefabFolder + "/InclinedPlaneSetupModule.prefab";
            PrefabUtility.SaveAsPrefabAssetAndConnect(module, prefabPath, InteractionMode.AutomatedAction);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        private static void Set(SerializedObject serialized, string propertyName, Object value)
        {
            serialized.FindProperty(propertyName).objectReferenceValue = value;
        }

        private struct UiRefs
        {
            public GameObject ControlPanel;
            public Button CloseButton;
            public Slider AngleSlider;
            public InputField AngleInput;
            public Button AngleMinusButton;
            public Button AnglePlusButton;
            public Button ResetButton;
            public Text AngleText;
            public Text HeightText;
            public Text StatusText;
        }
    }
}
