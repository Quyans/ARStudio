using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using TriLib;

namespace TriLibEditor
{
    [CustomEditor(typeof(DefaultAsset))]
    public class TriLibAssetEditor : Editor
    {
        public static TriLibAssetEditor Active { get; private set; }

        private readonly bool[] _groupUnfolded = new bool[AssetAdvancedPropertyMetadata.GroupCount];
        private AssetLoaderOptions _tempAssetLoaderOptions;
        private SerializedObject _tempSerializedObject;
        private AssetImporter _assetImporter;
        private GUIStyle _preBackground;
        private Editor _tempEditor;
        private bool _isAssimpAsset;
        private bool _hasChanged;
        private int _currentTab;

        public string AssetPath { get; private set; }

        public void OnPrefabCreated(GameObject createdPrefab)
        {
            DestroyTempObject();
            LoadUserData();
            _tempEditor = CreateEditor(createdPrefab);
        }

        protected void OnEnable()
        {
            if (!TriLibCheckPlugins.PluginsLoaded)
            {
                return;
            }
            Cleanup();
            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }
            GetAssetPath();
            CheckForAssimpAsset();
            if (!_isAssimpAsset)
            {
                return;
            }
            Active = this;
            LoadUserData();
            SaveChanges();
        }

        protected void OnDisable()
        {
            Cleanup();
        }

        protected void OnDestroy()
        {
            Cleanup();
        }

        private void Cleanup()
        {
            Active = null;
            AssetPath = null;
            _assetImporter = null;
            _preBackground = null;
            _isAssimpAsset = false;
            for (var i = 0; i < _groupUnfolded.Length; i++)
            {
                _groupUnfolded[i] = false;
            }
            DestroyTempObject();
        }

        private void DestroyTempObject()
        {
            if (_tempAssetLoaderOptions != null)
            {
                DestroyImmediate(_tempAssetLoaderOptions);
                _tempAssetLoaderOptions = null;
            }
            if (_tempEditor != null)
            {
                DestroyImmediate(_tempEditor);
                _tempEditor = null;
            }
            _hasChanged = false;
        }

        private void LoadUserData()
        {
            _tempAssetLoaderOptions = AssetLoaderOptions.CreateInstance();
            _assetImporter = AssetImporter.GetAtPath(AssetPath);
            var userData = _assetImporter.userData;
            if (!string.IsNullOrEmpty(userData))
            {
                _tempAssetLoaderOptions.Deserialize(userData);
            }
            _tempSerializedObject = new SerializedObject(_tempAssetLoaderOptions);
            _tempSerializedObject.Update();
        }

        public override bool HasPreviewGUI()
        {
            return _isAssimpAsset || base.HasPreviewGUI();
        }

        public override void OnInspectorGUI()
        {
            if (!_isAssimpAsset)
            {
                base.OnInspectorGUI();
                return;
            }
            GUI.enabled = true;
#pragma warning disable 618
            EditorGUIUtility.LookLikeInspector();
#pragma warning restore 618
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            _currentTab = GUILayout.Toggle(_currentTab == 0, "General", "Button") ? 0 : _currentTab;
            _currentTab = GUILayout.Toggle(_currentTab == 1, "Meshes", "Button") ? 1 : _currentTab;
            _currentTab = GUILayout.Toggle(_currentTab == 2, "Materials", "Button") ? 2 : _currentTab;
            _currentTab = GUILayout.Toggle(_currentTab == 3, "Animations", "Button") ? 3 : _currentTab;
            _currentTab = GUILayout.Toggle(_currentTab == 4, "Misc", "Button") ? 4 : _currentTab;
            _currentTab = GUILayout.Toggle(_currentTab == 5, "Metadata", "Button") ? 5 : _currentTab;
            _currentTab = GUILayout.Toggle(_currentTab == 6, "Advanced", "Button") ? 6 : _currentTab;
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
            EditorGUI.BeginChangeCheck();
            switch (_currentTab)
            {
                case 0:
                    EditorGUILayout.LabelField("General", EditorStyles.boldLabel);
#if UNITY_2017_3_OR_NEWER 
                    _tempAssetLoaderOptions.PostProcessSteps = (AssimpPostProcessSteps)EditorGUILayout.EnumFlagsField("Post Processor Options", _tempAssetLoaderOptions.PostProcessSteps);
#else
                   _tempAssetLoaderOptions.PostProcessSteps = (AssimpPostProcessSteps)EditorGUILayout.EnumMaskField("Post Processor Options", _tempAssetLoaderOptions.PostProcessSteps);
#endif
                    break;
                case 1:
                    {
                        EditorGUILayout.LabelField("Meshes", EditorStyles.boldLabel);
                        var dontLoadMeshesProperty = _tempSerializedObject.FindProperty("DontLoadMeshes");
                        EditorGUILayout.PropertyField(dontLoadMeshesProperty);
                        if (!dontLoadMeshesProperty.boolValue)
                        {
                            EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("CombineMeshes"));
                            EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("Scale"));
                            EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("RotationAngles"));
#if UNITY_2017_3_OR_NEWER
                            EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("Use32BitsIndexFormat"));
#endif
                            var generateMeshCollidersProperty = _tempSerializedObject.FindProperty("GenerateMeshColliders");
                            EditorGUILayout.PropertyField(generateMeshCollidersProperty);
                            if (generateMeshCollidersProperty.boolValue)
                            {
                                EditorGUI.indentLevel = 1;
                                EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("ConvexMeshColliders"));
                                EditorGUI.indentLevel = 0;
                            }
                        }
                    }
                    break;
                case 2:
                    {
                        EditorGUILayout.LabelField("Materials", EditorStyles.boldLabel);
                        var dontLoadMaterialsProperty = _tempSerializedObject.FindProperty("DontLoadMaterials");
                        EditorGUILayout.PropertyField(dontLoadMaterialsProperty);
                        if (!dontLoadMaterialsProperty.boolValue)
                        {
                            EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("ApplyAlphaMaterials"));
                            EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("DisableAlphaMaterials"));
                            EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("UseCutoutMaterials"));
                            EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("UseStandardSpecularMaterial"));
                            EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("TextureCompression"));
                            EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("GenerateMipMaps"));
                        }
                    }
                    break;
                case 3:
                    {
                        EditorGUILayout.LabelField("Animations", EditorStyles.boldLabel);
                        var dontLoadAnimationsProperty = _tempSerializedObject.FindProperty("DontLoadAnimations");
                        EditorGUILayout.PropertyField(dontLoadAnimationsProperty);
                        if (!dontLoadAnimationsProperty.boolValue)
                        {
                            EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("AutoPlayAnimations"));
                            EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("AnimationWrapMode"));
                            EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("EnsureQuaternionContinuity"));
                            var legacyAnimationProperty = _tempSerializedObject.FindProperty("UseLegacyAnimations");
                            EditorGUILayout.PropertyField(legacyAnimationProperty);
                            if (!legacyAnimationProperty.boolValue)
                            {
                                EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("AnimatorController"));
                                EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("Avatar"));
                            }
                        }
                    }
                    break;
                case 4:
                    {
                        EditorGUILayout.LabelField("Misc", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("AddAssetUnloader"));
                        EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("DontLoadCameras"));
                        EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("DontLoadLights"));
                    }
                    break;
                case 5:
                    {
                        EditorGUILayout.LabelField("Metadata", EditorStyles.boldLabel);
                        EditorGUILayout.PropertyField(_tempSerializedObject.FindProperty("DontLoadMetadata"));
                    }
                    break;
                default:
                    {
                        EditorGUILayout.LabelField("Advanced", EditorStyles.boldLabel);
                        var advancedConfigs = _tempSerializedObject.FindProperty("AdvancedConfigs");
                        var lastGroup = string.Empty;
                        var groupIndex = 0;
                        foreach (var configKey in AssetAdvancedPropertyMetadata.ConfigKeys)
                        {
                            AssetAdvancedConfigType assetAdvancedConfigType;
                            string className;
                            string description;
                            string group;
                            bool hasDefaultValue;
                            bool hasMinValue;
                            bool hasMaxValue;
                            object defaultValue;
                            object minValue;
                            object maxValue;
                            AssetAdvancedPropertyMetadata.GetOptionMetadata(configKey, out assetAdvancedConfigType, out className, out description, out group, out defaultValue, out minValue, out maxValue, out hasDefaultValue, out hasMinValue, out hasMaxValue);
                            SerializedProperty elementSerializedProperty = null;
                            var serializedElementIndex = 0;
                            var recentlyCreated = false;
                            for (var i = 0; i < _tempAssetLoaderOptions.AdvancedConfigs.Count; i++)
                            {
                                var advancedProperty = _tempAssetLoaderOptions.AdvancedConfigs[i];
                                if (advancedProperty.Key == configKey)
                                {
                                    serializedElementIndex = i;
                                    elementSerializedProperty = advancedConfigs.GetArrayElementAtIndex(i);
                                    break;
                                }
                            }
                            if (group != lastGroup)
                            {
                                lastGroup = group;
                                _groupUnfolded[groupIndex] = EditorGUILayout.Foldout(_groupUnfolded[groupIndex], lastGroup);
                                groupIndex++;
                            }
                            if (_groupUnfolded[groupIndex - 1])
                            {
                                EditorGUILayout.BeginHorizontal();
                                var enableProperty = EditorGUILayout.BeginToggleGroup(new GUIContent(className, description), elementSerializedProperty != null);
                                if (elementSerializedProperty == null && enableProperty)
                                {
                                    advancedConfigs.InsertArrayElementAtIndex(advancedConfigs.arraySize);
                                    _tempSerializedObject.ApplyModifiedProperties();
                                    var elementIndex = Mathf.Max(0, advancedConfigs.arraySize - 1);
                                    elementSerializedProperty = advancedConfigs.GetArrayElementAtIndex(elementIndex);
                                    elementSerializedProperty.FindPropertyRelative("Key").stringValue = configKey;
                                    _tempSerializedObject.ApplyModifiedProperties();
                                    recentlyCreated = true;
                                }
                                else if (elementSerializedProperty != null && !enableProperty)
                                {
                                    advancedConfigs.DeleteArrayElementAtIndex(serializedElementIndex);
                                    _tempSerializedObject.ApplyModifiedProperties();
                                    return;
                                }
                                SerializedProperty valueSerializedProperty;
                                switch (assetAdvancedConfigType)
                                {
                                    case AssetAdvancedConfigType.Bool:
                                        var boolDefaultValue = hasDefaultValue && (bool)defaultValue;
                                        if (elementSerializedProperty == null)
                                        {
                                            GUI.enabled = false;
                                            EditorGUILayout.Toggle(boolDefaultValue ? "Enabled" : "Disabled", boolDefaultValue);
                                        }
                                        else
                                        {
                                            GUI.enabled = true;
                                            valueSerializedProperty = elementSerializedProperty.FindPropertyRelative("BoolValue");
                                            if (recentlyCreated)
                                            {
                                                valueSerializedProperty.boolValue = boolDefaultValue;
                                            }
                                            EditorGUILayout.PropertyField(valueSerializedProperty, new GUIContent(valueSerializedProperty.boolValue ? "Enabled" : "Disabled"));
                                        }
                                        break;
                                    case AssetAdvancedConfigType.Integer:
                                        var intDefaultValue = hasDefaultValue ? (int)defaultValue : 0;
                                        if (hasMinValue && hasMaxValue)
                                        {
                                            if (elementSerializedProperty == null)
                                            {
                                                GUI.enabled = false;
                                                EditorGUILayout.IntSlider(intDefaultValue, (int)minValue, (int)maxValue);
                                            }
                                            else
                                            {
                                                GUI.enabled = true;
                                                valueSerializedProperty = elementSerializedProperty.FindPropertyRelative("IntValue");
                                                if (recentlyCreated)
                                                {
                                                    valueSerializedProperty.intValue = intDefaultValue;
                                                }
                                                EditorGUILayout.IntSlider(valueSerializedProperty, (int)minValue, (int)maxValue, GUIContent.none);
                                            }
                                        }
                                        else
                                        {
                                            if (elementSerializedProperty == null)
                                            {
                                                GUI.enabled = false;
                                                EditorGUILayout.IntField(intDefaultValue);
                                            }
                                            else
                                            {
                                                GUI.enabled = true;
                                                valueSerializedProperty = elementSerializedProperty.FindPropertyRelative("IntValue");
                                                if (recentlyCreated)
                                                {
                                                    valueSerializedProperty.intValue = intDefaultValue;
                                                }
                                                EditorGUILayout.PropertyField(valueSerializedProperty, GUIContent.none);
                                            }
                                        }
                                        break;
                                    case AssetAdvancedConfigType.Float:
                                        var floatDefaultValue = hasDefaultValue ? (float)defaultValue : 0f;
                                        if (hasMinValue && hasMaxValue)
                                        {
                                            if (elementSerializedProperty == null)
                                            {
                                                GUI.enabled = false;
                                                EditorGUILayout.Slider(floatDefaultValue, (float)minValue, (float)maxValue);
                                            }
                                            else
                                            {
                                                GUI.enabled = true;
                                                valueSerializedProperty = elementSerializedProperty.FindPropertyRelative("FloatValue");
                                                if (recentlyCreated)
                                                {
                                                    valueSerializedProperty.floatValue = floatDefaultValue;
                                                }
                                                EditorGUILayout.Slider(valueSerializedProperty, (float)minValue, (float)maxValue, GUIContent.none);
                                            }
                                        }
                                        else
                                        {
                                            if (elementSerializedProperty == null)
                                            {
                                                GUI.enabled = false;
                                                EditorGUILayout.FloatField(floatDefaultValue);
                                            }
                                            else
                                            {
                                                GUI.enabled = true;
                                                valueSerializedProperty = elementSerializedProperty.FindPropertyRelative("FloatValue");
                                                if (recentlyCreated)
                                                {
                                                    valueSerializedProperty.floatValue = floatDefaultValue;
                                                }
                                                EditorGUILayout.PropertyField(valueSerializedProperty, GUIContent.none);
                                            }
                                        }
                                        break;
                                    case AssetAdvancedConfigType.String:
                                        var stringDefaultValue = hasDefaultValue ? (string)defaultValue : string.Empty;
                                        if (elementSerializedProperty == null)
                                        {
                                            GUI.enabled = false;
                                            EditorGUILayout.TextField(stringDefaultValue);
                                        }
                                        else
                                        {
                                            GUI.enabled = true;
                                            valueSerializedProperty = elementSerializedProperty.FindPropertyRelative("StringValue");
                                            if (recentlyCreated)
                                            {
                                                valueSerializedProperty.stringValue = stringDefaultValue;
                                            }
                                            EditorGUILayout.PropertyField(valueSerializedProperty, GUIContent.none);
                                        }
                                        break;
                                    case AssetAdvancedConfigType.AiComponent:
                                        var aiComponentDefaultValue = hasDefaultValue ? (AiComponent)defaultValue : AiComponent.Animations;
                                        if (elementSerializedProperty == null)
                                        {
                                            GUI.enabled = false;
#if UNITY_2017_3_OR_NEWER
                                            EditorGUILayout.EnumFlagsField(aiComponentDefaultValue);
#else
                                            EditorGUILayout.EnumMaskField(aiComponentDefaultValue);
#endif
                                        }
                                        else
                                        {
                                            GUI.enabled = true;
                                            valueSerializedProperty = elementSerializedProperty.FindPropertyRelative("IntValue");
                                            if (recentlyCreated)
                                            {
                                                valueSerializedProperty.intValue = (int)aiComponentDefaultValue;
                                            }
                                            PropertyEnumMaskField(valueSerializedProperty, assetAdvancedConfigType, GUIContent.none);
                                        }
                                        break;
                                    case AssetAdvancedConfigType.AiPrimitiveType:
                                        var aiPrimitiveTypeDefaultValue = hasDefaultValue ? (AiPrimitiveType)defaultValue : AiPrimitiveType.Line;
                                        if (elementSerializedProperty == null)
                                        {
                                            GUI.enabled = false;
#if UNITY_2017_3_OR_NEWER
                                            EditorGUILayout.EnumFlagsField(aiPrimitiveTypeDefaultValue);
#else
                                            EditorGUILayout.EnumMaskField(aiPrimitiveTypeDefaultValue);
#endif
                                        }
                                        else
                                        {
                                            GUI.enabled = true;
                                            valueSerializedProperty = elementSerializedProperty.FindPropertyRelative("IntValue");
                                            if (recentlyCreated)
                                            {
                                                valueSerializedProperty.intValue = (int)aiPrimitiveTypeDefaultValue;
                                            }
                                            PropertyEnumMaskField(valueSerializedProperty, assetAdvancedConfigType, GUIContent.none);
                                        }
                                        break;
                                    case AssetAdvancedConfigType.AiUVTransform:
                                        var aiUvTransformDefaultValue = hasDefaultValue ? (AiUVTransform)defaultValue : AiUVTransform.Rotation;
                                        if (elementSerializedProperty == null)
                                        {
                                            GUI.enabled = false;
#if UNITY_2017_3_OR_NEWER
                                            EditorGUILayout.EnumFlagsField(aiUvTransformDefaultValue);
#else
                                            EditorGUILayout.EnumMaskField(aiUvTransformDefaultValue);
#endif
                                        }
                                        else
                                        {
                                            GUI.enabled = true;
                                            valueSerializedProperty = elementSerializedProperty.FindPropertyRelative("IntValue");
                                            if (recentlyCreated)
                                            {
                                                valueSerializedProperty.intValue = (int)aiUvTransformDefaultValue;
                                            }
                                            PropertyEnumMaskField(valueSerializedProperty, assetAdvancedConfigType, GUIContent.none);
                                        }
                                        break;
                                    case AssetAdvancedConfigType.AiMatrix:
                                        if (elementSerializedProperty == null)
                                        {
                                            GUI.enabled = false;
                                            GUILayout.BeginVertical();
                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Translation", GUILayout.Width(75));
                                            GUILayout.FlexibleSpace();
                                            EditorGUILayout.Vector3Field(GUIContent.none, Vector3.zero);
                                            GUILayout.EndHorizontal();
                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Rotation", GUILayout.Width(75));
                                            GUILayout.FlexibleSpace();
                                            EditorGUILayout.Vector3Field(GUIContent.none, Vector3.zero);
                                            GUILayout.EndHorizontal();
                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Scale", GUILayout.Width(75));
                                            GUILayout.FlexibleSpace();
                                            EditorGUILayout.Vector3Field(GUIContent.none, Vector3.one);
                                            GUILayout.EndHorizontal();
                                            GUILayout.EndVertical();
                                        }
                                        else
                                        {
                                            GUI.enabled = true;
                                            var valueSerializedProperty1 = elementSerializedProperty.FindPropertyRelative("TranslationValue");
                                            var valueSerializedProperty2 = elementSerializedProperty.FindPropertyRelative("RotationValue");
                                            var valueSerializedProperty3 = elementSerializedProperty.FindPropertyRelative("ScaleValue");
                                            if (recentlyCreated)
                                            {
                                                valueSerializedProperty1.vector3Value = Vector3.zero;
                                                valueSerializedProperty2.vector3Value = Vector3.zero;
                                                valueSerializedProperty3.vector3Value = Vector3.one;
                                            }
                                            GUILayout.BeginVertical();
                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Translation", GUILayout.Width(75));
                                            GUILayout.FlexibleSpace();
                                            EditorGUILayout.PropertyField(valueSerializedProperty1, GUIContent.none, true, GUILayout.MinWidth(100));
                                            GUILayout.EndHorizontal();
                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Rotation", GUILayout.Width(75));
                                            GUILayout.FlexibleSpace();
                                            EditorGUILayout.PropertyField(valueSerializedProperty2, GUIContent.none, true, GUILayout.MinWidth(100));
                                            GUILayout.EndHorizontal();
                                            GUILayout.BeginHorizontal();
                                            GUILayout.Label("Scale", GUILayout.Width(75));
                                            GUILayout.FlexibleSpace();
                                            EditorGUILayout.PropertyField(valueSerializedProperty3, GUIContent.none, true, GUILayout.MinWidth(100));
                                            GUILayout.EndHorizontal();
                                            GUILayout.EndVertical();
                                        }
                                        break;
                                }
                                GUI.enabled = true;
                                EditorGUILayout.EndToggleGroup();
                                EditorGUILayout.EndHorizontal();
                            }
                        }
                    }
                    break;
            }
            if (EditorGUI.EndChangeCheck())
            {
                _hasChanged = true;
            }
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUI.enabled = _hasChanged;
            if (GUILayout.Button("Revert"))
            {
                DestroyTempObject();
                _tempAssetLoaderOptions = AssetLoaderOptions.CreateInstance();
                SaveChanges();
            }
            if (GUILayout.Button("Apply"))
            {
                _tempSerializedObject.ApplyModifiedProperties();
                SaveChanges();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        private void PropertyEnumMaskField(SerializedProperty property, AssetAdvancedConfigType assetAdvancedConfigType, GUIContent guiContent)
        {
            var position = EditorGUILayout.GetControlRect();
            EditorGUI.BeginProperty(position, guiContent, property);
            Enum enumValue;
            switch (assetAdvancedConfigType)
            {
                case AssetAdvancedConfigType.AiComponent:
                    enumValue = (AiComponent)property.intValue;
                    break;
                case AssetAdvancedConfigType.AiPrimitiveType:
                    enumValue = (AiPrimitiveType)property.intValue;
                    break;
                case AssetAdvancedConfigType.AiUVTransform:
                    enumValue = (AiUVTransform)property.intValue;
                    break;
                default:
                    return;
            }
#if UNITY_2017_3_OR_NEWER
            property.intValue = Convert.ToInt32(EditorGUI.EnumFlagsField(position, enumValue));
#else
            property.intValue = Convert.ToInt32(EditorGUI.EnumMaskField(position, enumValue));
#endif
            EditorGUI.EndProperty();
        }

        public override void OnInteractivePreviewGUI(Rect rect, GUIStyle background)
        {
            if (!_isAssimpAsset || _tempEditor == null)
            {
                base.OnInteractivePreviewGUI(rect, background);
                return;
            }
            if (_preBackground == null)
            {
                _preBackground = new GUIStyle("preBackground");
            }
            _tempEditor.OnInteractivePreviewGUI(rect, _preBackground);
        }

        private void SaveChanges()
        {
            _assetImporter.userData = _tempAssetLoaderOptions.Serialize();
            _assetImporter.SaveAndReimport();
            _hasChanged = false;
        }

        private void GetAssetPath()
        {
            AssetPath = AssetDatabase.GetAssetPath(target);
        }

        private void CheckForAssimpAsset()
        {
            var extension = Path.GetExtension(AssetPath);
            if (string.IsNullOrEmpty(extension))
            {
                _isAssimpAsset = false;
            }
            _isAssimpAsset = AssetLoaderBase.IsExtensionSupported(extension);
        }
    }
}
