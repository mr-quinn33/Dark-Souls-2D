using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;

namespace UnityEditor.AI
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(NavMeshModifierVolume))]
    internal class NavMeshModifierVolumeEditor : Editor
    {
        private SerializedProperty m_AffectedAgents;
        private SerializedProperty m_Area;
        private SerializedProperty m_Center;
        private SerializedProperty m_Size;
        private static Color s_HandleColor = new Color(187f, 138f, 240f, 210f) / 255;
        private static Color s_HandleColorDisabled = new Color(187f * 0.75f, 138f * 0.75f, 240f * 0.75f, 100f) / 255;
        private BoxBoundsHandle m_BoundsHandle = new BoxBoundsHandle();

        private bool EditingCollider => EditMode.editMode == EditMode.SceneViewEditMode.Collider && EditMode.IsOwner(this);

        private void OnEnable()
        {
            m_AffectedAgents = serializedObject.FindProperty("m_AffectedAgents");
            m_Area = serializedObject.FindProperty("m_Area");
            m_Center = serializedObject.FindProperty("m_Center");
            m_Size = serializedObject.FindProperty("m_Size");

            NavMeshVisualizationSettings.showNavigation++;
        }

        private void OnDisable() => NavMeshVisualizationSettings.showNavigation--;

        private Bounds GetBounds()
        {
            NavMeshModifierVolume navModifier = (NavMeshModifierVolume)target;
            return new Bounds(navModifier.transform.position, navModifier.Size);
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditMode.DoEditModeInspectorModeButton(EditMode.SceneViewEditMode.Collider, "Edit Volume",
                EditorGUIUtility.IconContent("EditCollider"), GetBounds, this);

            _ = EditorGUILayout.PropertyField(m_Size);
            _ = EditorGUILayout.PropertyField(m_Center);

            NavMeshComponentsGUIUtility.AreaPopup("Area Type", m_Area);
            NavMeshComponentsGUIUtility.AgentMaskPopup("Affected Agents", m_AffectedAgents);
            EditorGUILayout.Space();

            _ = serializedObject.ApplyModifiedProperties();
        }

        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
#pragma warning disable IDE0051 // 删除未使用的私有成员
        private static void RenderBoxGizmo(NavMeshModifierVolume navModifier, GizmoType _)
#pragma warning restore IDE0051 // 删除未使用的私有成员
        {
            Color color = navModifier.enabled ? s_HandleColor : s_HandleColorDisabled;
            Color colorTrans = new Color(color.r * 0.75f, color.g * 0.75f, color.b * 0.75f, color.a * 0.15f);

            Color oldColor = Gizmos.color;
            Matrix4x4 oldMatrix = Gizmos.matrix;

            Gizmos.matrix = navModifier.transform.localToWorldMatrix;

            Gizmos.color = colorTrans;
            Gizmos.DrawCube(navModifier.Center, navModifier.Size);

            Gizmos.color = color;
            Gizmos.DrawWireCube(navModifier.Center, navModifier.Size);

            Gizmos.matrix = oldMatrix;
            Gizmos.color = oldColor;

            Gizmos.DrawIcon(navModifier.transform.position, "NavMeshModifierVolume Icon", true);
        }

        [DrawGizmo(GizmoType.NotInSelectionHierarchy | GizmoType.Pickable)]
#pragma warning disable IDE0051 // 删除未使用的私有成员
        private static void RenderBoxGizmoNotSelected(NavMeshModifierVolume navModifier, GizmoType _)
#pragma warning restore IDE0051 // 删除未使用的私有成员
        {
            if (NavMeshVisualizationSettings.showNavigation > 0)
            {
                Color color = navModifier.enabled ? s_HandleColor : s_HandleColorDisabled;
                Color oldColor = Gizmos.color;
                Matrix4x4 oldMatrix = Gizmos.matrix;

                Gizmos.matrix = navModifier.transform.localToWorldMatrix;

                Gizmos.color = color;
                Gizmos.DrawWireCube(navModifier.Center, navModifier.Size);

                Gizmos.matrix = oldMatrix;
                Gizmos.color = oldColor;
            }

            Gizmos.DrawIcon(navModifier.transform.position, "NavMeshModifierVolume Icon", true);
        }

        private void OnSceneGUI()
        {
            if (!EditingCollider)
            {
                return;
            }

            NavMeshModifierVolume vol = (NavMeshModifierVolume)target;
            Color color = vol.enabled ? s_HandleColor : s_HandleColorDisabled;
            using (new Handles.DrawingScope(color, vol.transform.localToWorldMatrix))
            {
                m_BoundsHandle.center = vol.Center;
                m_BoundsHandle.size = vol.Size;

                EditorGUI.BeginChangeCheck();
                m_BoundsHandle.DrawHandle();
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(vol, "Modified NavMesh Modifier Volume");
                    Vector3 center = m_BoundsHandle.center;
                    Vector3 size = m_BoundsHandle.size;
                    vol.Center = center;
                    vol.Size = size;
                    EditorUtility.SetDirty(target);
                }
            }
        }

        [MenuItem("GameObject/AI/NavMesh Modifier Volume", false, 2001)]
        public static void CreateNavMeshModifierVolume(MenuCommand menuCommand)
        {
            GameObject parent = menuCommand.context as GameObject;
            GameObject go = NavMeshComponentsGUIUtility.CreateAndSelectGameObject("NavMesh Modifier Volume", parent);
            _ = go.AddComponent<NavMeshModifierVolume>();
            SceneView view = SceneView.lastActiveSceneView;
            if (view != null)
            {
                view.MoveToView(go.transform);
            }
        }
    }
}
