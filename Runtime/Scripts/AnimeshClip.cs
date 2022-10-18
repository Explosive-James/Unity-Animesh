using UnityEngine;

namespace Animesh
{
    public class AnimeshClip : ScriptableObject
    {
        #region Data
        /// <summary>
        /// Positional offset for each frame of an animation.
        /// </summary>
        public Texture2D positionTex;
        /// <summary>
        /// Normal vectors for each frame of an animation.
        /// </summary>
        public Texture2D normalTex;

        /// <summary>
        /// The playback rate of the animation.
        /// </summary>
        public float framerate = 30;
        #endregion

        #region Public Functions
        /// <summary>
        /// Creates an instance of the animesh clip.
        /// </summary>
        public static AnimeshClip CreateInstance(Texture2D positionTex, Texture2D normalTex, float framerate)
        {
            AnimeshClip output = CreateInstance<AnimeshClip>();
            output.framerate = framerate;

            output.positionTex = positionTex;
            output.normalTex = normalTex;

            return output;
        }
        #endregion
    }
}
