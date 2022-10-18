using Animesh;
using AnimeshEditor.Window;
using System;
using System.Collections.Generic;
using Unity.EditorCoroutines.Editor;
using UnityEngine;

namespace AnimeshEditor.Rendering
{
    public class RenderManager
    {
        #region Data
        private WindowSettings settings;
        private AnimationRenderer renderer;

        /// <summary>
        /// The coroutine that converts the animations to textures.
        /// </summary>
        private EditorCoroutine currentRoutine;
        #endregion

        #region Properties
        /// <summary>
        /// Is the manager currently converting animations to textures.
        /// </summary>
        public bool CurrentlyRendering => currentRoutine != null;

        /// <summary>
        /// Settings of the render manager.
        /// </summary>
        public WindowSettings CurrentSetting => settings;
        #endregion

        #region Events
        /// <summary>
        /// Invoked when a texture is converted to textures.
        /// Contains position and normal textures and animation clip if specified.
        /// </summary>
        public event Action<UnityEngine.Object[]> OnTexturesRendered;
        #endregion

        #region Constructor
        public RenderManager(WindowSettings originalSettings)
        {
            // Creating a duplicate to prevent the user from altering the settings while rendering.
            settings = UnityEngine.Object.Instantiate(originalSettings);

            renderer = new AnimationRenderer(settings.animator, settings.skinnedMesh);
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Starts the rendering process for the manager, 
        /// aborts current render if one is already running.
        /// </summary>
        public void RenderAnimations()
        {
            if (CurrentlyRendering) {

                EditorCoroutineUtility.StopCoroutine(currentRoutine);
            }

            currentRoutine = EditorCoroutineUtility.StartCoroutine(InternalRenderAnimations(), this);
        }
        /// <summary>
        /// Aborts the rendering process if it#s running.
        /// </summary>
        public void AbortRendering()
        {
            if (CurrentlyRendering) {

                EditorCoroutineUtility.StopCoroutine(currentRoutine);
                currentRoutine = null;
            }
        }
        #endregion

        #region Private Functions
        private IEnumerator<YieldInstruction> InternalRenderAnimations()
        {
            string modelName = settings.skinnedMesh.sharedMesh.name + "_";

            foreach (AnimationClip animation in settings.animations) {

                List<UnityEngine.Object> renderedAssets = new List<UnityEngine.Object>();

                // Calculating the size for the output textures.
                int textureHeight = Mathf.CeilToInt(animation.length * settings.framerate);
                int textureWidth = settings.skinnedMesh.sharedMesh.vertexCount;

                // Creating textures that will store the position and normal information.
                RenderTexture positionTex = new RenderTexture(textureWidth, textureHeight, 32, RenderTextureFormat.ARGBFloat) {
                    enableRandomWrite = true,
                    name = modelName + animation.name + "_position",
                };
                RenderTexture normalTex = new RenderTexture(textureWidth, textureHeight, 32, RenderTextureFormat.ARGBFloat) {
                    enableRandomWrite = true,
                    name = modelName + animation.name + "_normals",
                };

                // Converting the animation to a texture using the renderer.
                foreach (YieldInstruction inst in renderer.RenderAnimation(
                    animation, settings.framerate, positionTex, normalTex)) {

                    yield return inst;
                }

                // Converting render textures to texture2ds.
                Texture2D finalPositionTex = positionTex.ToTexture2D();
                Texture2D finalNormalTex = normalTex.ToTexture2D();

                renderedAssets.AddRange(new Texture2D[] { finalPositionTex, finalNormalTex });

                if (settings.generateClips) {

                    AnimeshClip clip = AnimeshClip.CreateInstance(
                        finalPositionTex, finalNormalTex, settings.framerate);

                    clip.name = modelName + animation.name;
                    renderedAssets.Add(clip);
                }

                OnTexturesRendered?.Invoke(renderedAssets.ToArray());
            }

            currentRoutine = null;
        }
        #endregion
    }
}
