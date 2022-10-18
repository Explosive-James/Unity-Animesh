using System.Collections.Generic;
using UnityEngine;

namespace AnimeshEditor.Rendering
{
    public class AnimationRenderer
    {
        #region Data
        /// <summary>
        /// Animator that plays the animation.
        /// </summary>
        private readonly Animator animator;
        /// <summary>
        /// The skinned mesh that is used to bake the animated mesh.
        /// </summary>
        private readonly SkinnedMeshRenderer skinnedMesh;
        #endregion

        #region Constructor
        public AnimationRenderer(Animator animator, SkinnedMeshRenderer skinnedMesh)
        {
            this.animator = animator;
            this.skinnedMesh = skinnedMesh;
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Converts an animation to textures.
        /// </summary>
        public IEnumerable<YieldInstruction> RenderAnimation(AnimationClip animation, float framerate,
            RenderTexture positionTex, RenderTexture normalTex)
        {
            Mesh animatedMesh = new Mesh();

            animator.Play(animation.name);
            animator.speed = 0;

            using (ComputeHandler compute = new ComputeHandler(skinnedMesh.sharedMesh.vertices, positionTex, normalTex)) {

                for(int i = 0; i < positionTex.height; i++) {

                    // Updating the animator.
                    animator.Play(animation.name, 0, i / (positionTex.height - 1f));
                    animator.Update(1f / framerate);

                    // Waiting one frame for the skinned mesh and animator to update the mesh.
                    yield return new WaitForEndOfFrame();

                    // Converting the mesh data to the output textures.
                    skinnedMesh.BakeMesh(animatedMesh, true);
                    compute.ConvertAnimationFrame(animatedMesh, animator.rootPosition, i);
                }
            }
        }
        #endregion
    }
}
