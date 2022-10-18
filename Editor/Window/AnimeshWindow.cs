using AnimeshEditor.Exporting;
using UnityEditor;
using UnityEngine;

namespace AnimeshEditor.Window
{
    public class AnimeshWindow : EditorWindow
    {
        #region Data
        /// <summary>
        /// The actual settings of the window.
        /// </summary>
        private WindowSettings settings;
        /// <summary>
        /// The serialized version of the window settings,
        /// this is to use the property drawer in the inspector.
        /// </summary>
        private SerializedObject settingsObj;
        #endregion

        #region Unity Functions
        [MenuItem("Window/Rendering/Animesh Renderer")]
        private static void CreateWindowInstance() => GetWindow<AnimeshWindow>("Animesh Renderer");

        private void OnGUI()
        {
            if (settings == null) settings = CreateInstance<WindowSettings>();
            settingsObj ??= new SerializedObject(settings);

            //----- Object Settings -----//
            EditorGUILayout.LabelField("Object Settings:", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Drawing the animator property field and assigning the skinned mesh if it's null.
            if (DrawSerializedProperties(settingsObj, nameof(settings.animator)) &&
                settings.skinnedMesh == null && settings.animator != null) {

                settings.skinnedMesh = settings.animator.GetComponentInChildren<SkinnedMeshRenderer>();
                settingsObj = new SerializedObject(settings);
            }
            // Drawing the skinned mesh property field and assigning the animator if it's null.
            if (DrawSerializedProperties(settingsObj, nameof(settings.skinnedMesh)) &&
                settings.animator == null && settings.skinnedMesh != null) {

                settings.animator = settings.skinnedMesh.GetComponentInParent<Animator>();
                settingsObj = new SerializedObject(settings);
            }

            //----- Animation Settings -----//
            EditorGUILayout.LabelField("Animations Settings:", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            DrawSerializedProperties(settingsObj, nameof(settings.animations));

            // Gets the animation clips from the animator controller.
            if (GUILayout.Button("Reset Animations")) {

                settings.animations = settings.animator != null ?
                    settings.animator.runtimeAnimatorController.animationClips : new AnimationClip[0];
                settingsObj = new SerializedObject(settings);
            }

            EditorGUILayout.LabelField("Export Settings:", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            // Drawing the property fields for the export settings.
            DrawSerializedProperties(settingsObj, nameof(settings.generateClips),
                nameof(settings.framerate), nameof(settings.directory));

            EditorGUILayout.Space();

            //----- Export Buttons -----//
            EditorGUI.BeginDisabledGroup(!settings.CanRenderAnimations());

            if (!ExportManager.CurrentlyExporting) {

                // Drawing the render button.
                if (GUILayout.Button("Render Animations"))
                    ExportManager.RenderAnimations(settings);
            }
            else {

                // Drawing the abort button.
                if (GUILayout.Button("Abort Exporting"))
                    ExportManager.AbortRendering();
            }
            EditorGUI.EndDisabledGroup();

            // Drawing the export button.
            EditorGUI.BeginDisabledGroup(!settings.CanExportModel());
            if (GUILayout.Button("Export Model")) {

                ExportManager.ExportMesh(settings.skinnedMesh, settings.directory);
            }
            EditorGUI.EndDisabledGroup();
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Draws a collection of properties for a serialized object.
        /// </summary>
        private bool DrawSerializedProperties(SerializedObject obj, params string[] propertyNames)
        {
            for (int i = 0; i < propertyNames.Length; i++) {

                SerializedProperty property = obj.FindProperty(propertyNames[i]);

                if (property != null) {
                    EditorGUILayout.PropertyField(property, true);
                }
            }

            if (obj.hasModifiedProperties) {

                obj.ApplyModifiedProperties();
                return true;
            }

            return false;
        }
        #endregion
    }
}
