using UnityEngine;

namespace Animesh
{
    public sealed class AnimeshController : MonoBehaviour
    {
        #region Data
        /// <summary>
        /// The animation that will be set when the controller starts.
        /// </summary>
        public AnimeshClip initialAnimation;
        /// <summary>
        /// The renderer that contains the animesh material.
        /// </summary>
        public MeshRenderer targetMeshRenderer;
        #endregion

        #region Properties
        /// <summary>
        /// The current clip being played by this controller.
        /// </summary>
        public AnimeshClip CurrentAnimation { get; private set; }
        #endregion

        #region Unity Functions
        private void Awake()
        {
            // Checking if the initial animation has been assigned.
            if (initialAnimation != null) {
                PlayAnimation(initialAnimation);
            }
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Sets the animation playing on an Animesh material.
        /// </summary>
        /// <param name="animation">Animesh clip to play.</param>
        /// <param name="normalizedTime">Animation offset in normalized time (0-1).</param>
        public void PlayAnimation(AnimeshClip animation, float normalizedTime = 0)
        {
            // Since the animation being played by the material is based on time,
            // so if the animation is changed it might not start at the beginning,
            // so first we need to calculate how far through the animation it
            // will be and offset it by that amount.
            float animationLength = animation.positionTex.height / (float)animation.framerate;
            float animationOffset = 1f - ((float)(Time.timeSinceLevelLoadAsDouble / animationLength) % 1f);

            // Creating a property block as this protects the GPU instancing if any.
            MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

            propertyBlock.SetTexture("_Position_Tex", animation.positionTex);
            propertyBlock.SetTexture("_Normal_Tex", animation.normalTex);

            propertyBlock.SetFloat("_Framerate", animation.framerate);
            propertyBlock.SetFloat("_Offset", animationOffset + normalizedTime);

            // Updating the mesh renderer to play the animesh clip.
            targetMeshRenderer.SetPropertyBlock(propertyBlock);
            CurrentAnimation = animation;
        }
        #endregion
    }
}
