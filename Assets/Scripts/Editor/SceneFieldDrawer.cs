using Core;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    /// <summary>
    /// Used so we can select the scene for each feature in the editor
    /// </summary>
    [CustomPropertyDrawer(typeof(SceneField))]
    public class SceneFieldPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var sceneAssetProp = property.FindPropertyRelative("_sceneAsset");
            var sceneNameProp = property.FindPropertyRelative("_sceneName");

            EditorGUI.BeginChangeCheck();
            var newAsset = EditorGUI.ObjectField(position, label, sceneAssetProp.objectReferenceValue, typeof(UnityEditor.SceneAsset), false);
            if (EditorGUI.EndChangeCheck())
            {
                sceneAssetProp.objectReferenceValue = newAsset;
                sceneNameProp.stringValue = newAsset != null ? newAsset.name : string.Empty;
            }
        }
    }
}