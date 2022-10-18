using AnimeshEditor.Rendering;
using AnimeshEditor.Window;
using UnityEngine;

namespace AnimeshEditor.Exporting
{
    public static class ExportManager
    {
        #region Data
        /// <summary>
        /// The renderer currently being used to convert the animations.
        /// </summary>
        private static RenderManager renderer;
        #endregion

        #region Properties
        /// <summary>
        /// Are animations currently being converted and exported.
        /// </summary>
        public static bool CurrentlyExporting =>
            renderer != null && renderer.CurrentlyRendering;
        #endregion

        #region Public Functions
        /// <summary>
        /// Starts rendering the animations in the settings, 
        /// aborting any current rendering process run by the manager.
        /// </summary>
        public static void RenderAnimations(WindowSettings settings)
        {
            if (renderer != null) AbortRendering();

            renderer = new RenderManager(settings);
            renderer.RenderAnimations();

            renderer.OnTexturesRendered += Renderer_OnTexturesRendered;
        }

        /// <summary>
        /// Force aborts any rendering process being run by the manager.
        /// </summary>
        public static void AbortRendering()
        {
            if (renderer != null) {

                renderer.OnTexturesRendered -= Renderer_OnTexturesRendered;

                renderer.AbortRendering();
                renderer = null;
            }
        }

        /// <summary>
        /// Exports the mesh from a skinned mesh renderer to it's own asset containing just the mesh.
        /// </summary>
        public static void ExportMesh(SkinnedMeshRenderer skinnedMesh, string directory)
        {
            string outputDirectory = DatabaseHelper.GetValidDirectory(directory);

            Mesh clonedMesh = Object.Instantiate(skinnedMesh.sharedMesh);
            clonedMesh.name = DatabaseHelper.GetValidFilename(clonedMesh.name);

            DatabaseHelper.CreateUnityAssets(outputDirectory, clonedMesh);
        }
        #endregion

        #region Private Functions
        /// <summary>
        /// Exports textures passed from the render manager.
        /// </summary>
        private static void Renderer_OnTexturesRendered(Object[] objs)
        {
            string outputDirectory = DatabaseHelper.GetValidDirectory(
                renderer.CurrentSetting.directory);

            foreach(Object o in objs) {
                o.name = DatabaseHelper.GetValidFilename(o.name);
            }

            DatabaseHelper.CreateUnityAssets(outputDirectory, objs);
        }
        #endregion
    }
}
