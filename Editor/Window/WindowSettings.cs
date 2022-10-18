using UnityEngine;

namespace AnimeshEditor.Window
{
    public class WindowSettings : ScriptableObject
    {
        #region Data
        /// <summary>
        /// Animator that will play the animations.
        /// </summary>
        public Animator animator;
        /// <summary>
        /// The skinned mesh being controlled by the animator.
        /// </summary>
        public SkinnedMeshRenderer skinnedMesh;

        /// <summary>
        /// Animations that will be converted into textures.
        /// </summary>
        public AnimationClip[] animations = new AnimationClip[0];

        /// <summary>
        /// The framerate of the output animation.
        /// </summary>
        public float framerate = 30;
        /// <summary>
        /// Generate Animesh Clips for the textures.
        /// </summary>
        public bool generateClips = true;

        /// <summary>
        /// The output directory for the textures.
        /// </summary>
        public string directory;
        #endregion

        #region Public Functions
        public bool CanRenderAnimations()
        {
            return animator != null && skinnedMesh != null &&
                animations.Length > 0;
        }
        public bool CanExportModel()
        {
            return skinnedMesh != null &&
                skinnedMesh.sharedMesh != null;
        }
        #endregion
    }
}
