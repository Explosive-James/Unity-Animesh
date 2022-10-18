using System;
using UnityEditor;
using UnityEngine;

namespace AnimeshEditor.Rendering
{
    public class ComputeHandler : IDisposable
    {
        #region Data
        /// <summary>
        /// Compute shader that processes vertex information and writes it to the render textures.
        /// </summary>
        private ComputeShader computeShader;
        /// <summary>
        /// The compute buffer controlling the initial vertex positions so the output texture is relative to the original mesh.
        /// </summary>
        private ComputeBuffer initialPositions;
        #endregion

        #region Constructor
        public ComputeHandler(Vector3[] initialPositions, RenderTexture positionTex, RenderTexture normalTex)
        {
            // Finding the compute shader that should be used to render textures.
            computeShader = AssetDatabase.LoadAssetAtPath<ComputeShader>(
                AssetDatabase.GUIDToAssetPath(AssetDatabase.FindAssets("AnimeshConverter")[0]));

            // Sending the initial vertex positions to the compute shader,
            // this makes the output textures relative to their start positions.
            this.initialPositions = new ComputeBuffer(initialPositions.Length, sizeof(float) * 3);
            this.initialPositions.SetData(initialPositions);

            computeShader.SetBuffer(0, "Initial_Positions", this.initialPositions);

            // Sending the output textures to the compute shader.
            computeShader.SetTexture(0, "Position_Tex", positionTex);
            computeShader.SetTexture(0, "Normal_Tex", normalTex);
        }
        #endregion

        #region Public Functions
        /// <summary>
        /// Writes the mesh data to text output textures using a compute shader.
        /// </summary>
        /// <param name="frameData">Mesh that contains the animated vertex positions.</param>
        /// <param name="rootPosition">Animator root position for the current frame.</param>
        /// <param name="frameNumber">The frame number of the animation, on the output texture.</param>
        public void ConvertAnimationFrame(Mesh frameData, Vector3 rootPosition, int frameNumber)
        {
            // Sending the vertex positions to the compute shader.
            ComputeBuffer updatedPositions = new ComputeBuffer(frameData.vertexCount, sizeof(float) * 3);
            updatedPositions.SetData(frameData.vertices);
            computeShader.SetBuffer(0, "Updated_Positions", updatedPositions);

            // Sending the normal directions to the compute shader.
            ComputeBuffer updatedNormals = new ComputeBuffer(frameData.vertexCount, sizeof(float) * 3);
            updatedNormals.SetData(frameData.normals);
            computeShader.SetBuffer(0, "Updated_Normals", updatedNormals);

            // Updating the misc values in the compute shader.
            computeShader.SetVector("Root_Position", rootPosition);
            computeShader.SetInt("Vertical_Offset", frameNumber);

            int threadGroups = (int)Math.Ceiling(frameData.vertexCount / 8.0) * 8;
            computeShader.Dispatch(0, threadGroups, 1, 1);

            // Releasing the compute buffers from unmanaged memory.
            updatedPositions.Release();
            updatedNormals.Release();
        }

        public void Dispose() => initialPositions.Dispose();
        #endregion
    }
}
