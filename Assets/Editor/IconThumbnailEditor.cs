using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class IconThumbnailEditor : EditorWindow
{
    [SerializeField]
    private VisualTreeAsset m_VisualTreeAsset = default;

    [MenuItem("Tool/Icon Editor")]
    public static void ShowExample()
    {
        IconThumbnailEditor wnd = GetWindow<IconThumbnailEditor>();
        wnd.titleContent = new GUIContent("IconThumbnailEditor");
    }

    private ListView m_list;
    private List<ObjectData> m_objects;

    [SerializeField]
    private ObjectData m_selectedObject;

    [SerializeField]
    private Texture2D m_previewTexture;

    //Preview
    private int m_size = 512;
    private Scene m_previewScene;
    private GameObject m_cameraObject;
    private Camera m_sceneCamera;
    private GameObject m_instance;

    private Vector3Field m_cameraRotationField;
    private Vector3Field m_cameraPositionField;
    private Vector3Field m_objectRotationField;

    private Button m_saveButton;

    public void CreateGUI()
    {
        // Each editor window contains a root VisualElement object
        VisualElement root = rootVisualElement;

        // Instantiate UXML
        VisualElement labelFromUXML = m_VisualTreeAsset.Instantiate();
        root.Add(labelFromUXML);

        m_list = rootVisualElement.Q<ListView>("List");
        m_objects = new List<ObjectData>();

        string[] guids = AssetDatabase.FindAssets("t:ObjectData");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            ObjectData obj = AssetDatabase.LoadAssetAtPath<ObjectData>(path);
            m_objects.Add(obj);
        }

        m_list.itemsSource = m_objects;
        m_list.selectionChanged += OnSelectItem;

        rootVisualElement.Q<VisualElement>("Content").dataSource = this;

        m_cameraRotationField = rootVisualElement.Q<Vector3Field>("CameraRotation");
        m_cameraPositionField = rootVisualElement.Q<Vector3Field>("CameraPosition");
        m_objectRotationField = rootVisualElement.Q<Vector3Field>("ObjectRotation");

        m_saveButton = rootVisualElement.Q<Button>("SaveButton");
        m_saveButton.clicked += Export;

        m_cameraRotationField.RegisterValueChangedCallback(OnCameraRotationChanged);
        m_cameraPositionField.RegisterValueChangedCallback(OnCameraPositionChanged);
        m_objectRotationField.RegisterValueChangedCallback(OnObjectRotationChanged);
    }

    private void Export()
    {
        m_sceneCamera.depthTextureMode = DepthTextureMode.Depth;
        m_sceneCamera.backgroundColor = new Color(0, 0, 0, 0);
        UpdateCamera();
        SaveTextureAsPNG(m_previewTexture, m_selectedObject.Name);
        m_sceneCamera.backgroundColor = Color.black;
        UpdateCamera();
    }

    private void SaveTextureAsPNG(Texture2D m_previewTexture, string name)
    {
        if (m_previewTexture == null)
        {
            Debug.LogError("No texture to save.");
            return;
        }

        string path = EditorUtility.SaveFilePanel("Save Texture As PNG", "", name + "_Icon.png", "png");

        if (string.IsNullOrEmpty(path))
        {
            Debug.Log("Save operation canceled.");
            return;
        }

        byte[] pngData = m_previewTexture.EncodeToPNG();
        if (pngData != null)
        {
            System.IO.File.WriteAllBytes(path, pngData);
            Debug.Log("Texture saved to: " + path);
        }
        else
        {
            Debug.LogError("Failed to encode texture to PNG.");
        }
    }

    private void OnObjectRotationChanged(ChangeEvent<Vector3> evt)
    {
        m_instance.transform.eulerAngles = evt.newValue;
        UpdateCamera();
    }

    private void OnCameraRotationChanged(ChangeEvent<Vector3> evt)
    {
        m_cameraObject.transform.eulerAngles = evt.newValue;
        UpdateCamera();
    }

    private void OnCameraPositionChanged(ChangeEvent<Vector3> evt)
    {
        m_cameraObject.transform.position = evt.newValue;
        UpdateCamera();
    }

    private void OnSelectItem(object item)
    {
        m_selectedObject = m_objects[m_list.selectedIndex];

        if (!m_previewScene.IsValid())
        {
            m_previewScene = EditorSceneManager.NewPreviewScene();
        }

        if (m_cameraObject == null)
        {
            m_cameraObject = new GameObject("Camera");
            m_cameraObject.transform.position = new Vector3(0, 0, -10);
            m_cameraObject.transform.eulerAngles = Vector3.zero;

            m_sceneCamera = m_cameraObject.AddComponent<Camera>();
            m_sceneCamera.aspect = 1;
            m_sceneCamera.backgroundColor = Color.black;
            m_sceneCamera.clearFlags = CameraClearFlags.SolidColor;
            m_sceneCamera.targetTexture = new RenderTexture(m_size, m_size, 32, RenderTextureFormat.ARGBFloat);

            SceneManager.MoveGameObjectToScene(m_cameraObject, m_previewScene);

            m_sceneCamera.scene = m_previewScene;

            m_cameraRotationField.value = m_cameraObject.transform.eulerAngles;
            m_cameraPositionField.value = m_cameraObject.transform.position;
        }

        if (m_instance != null)
        {
            DestroyImmediate(m_instance);
        }

        m_instance = (GameObject)PrefabUtility.InstantiatePrefab(m_selectedObject.prefab, m_previewScene);
        m_instance.transform.position = Vector3.zero;
        m_instance.transform.rotation = Quaternion.identity;

        m_objectRotationField.value = m_instance.transform.eulerAngles;

        UpdateCamera();
    }

    private void UpdateCamera()
    {
        m_sceneCamera.Render();

        if (m_previewTexture == null)
        {
            m_previewTexture = new Texture2D(m_size, m_size, TextureFormat.RGBAFloat, false);
        }

        RenderTexture.active = m_sceneCamera.targetTexture;

        m_previewTexture.ReadPixels(new Rect(0, 0, m_size, m_size), 0, 0);
        m_previewTexture.Apply();

        RenderTexture.active = null;
    }
}
