using UnityEngine;

namespace AnimeshEditor
{
    public static class RenderTextureExtentions
    {
        #region Public Functions
        /// <summary>
        /// Converts a Render Texture to a Texture2D
        /// </summary>
        public static Texture2D ToTexture2D(this RenderTexture texture)
        {
            RenderTexture.active = texture;
            Texture2D output = new Texture2D(texture.width, texture.height, TextureFormat.RGBAFloat, true) {

                name = texture.name,
                filterMode = texture.filterMode,
            };

            output.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            output.Apply();

            RenderTexture.active = null;
            return output;
        }
        #endregion
    }
}
