using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SplineMeshCollider))]
public class SplineMeshColliderEditor : Editor
{
    private SplineMeshCollider _splineMeshCollider;
    private SerializedProperty _splineContainerProperty;
    private SerializedProperty _roadWidthProperty;

    private void OnEnable()
    {
        _splineMeshCollider = (SplineMeshCollider)target;
        _splineContainerProperty = serializedObject.FindProperty("splineContainer");
        _roadWidthProperty = serializedObject.FindProperty("roadWidth");
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        DrawDefaultInspector();
        if (EditorGUI.EndChangeCheck())
        {
            _splineMeshCollider.CreateMeshCollider();
        }
    }

    void OnSceneGUI()
    {
        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.MouseUp)
        {
            _splineMeshCollider.CreateMeshCollider();
        }
    }
}
