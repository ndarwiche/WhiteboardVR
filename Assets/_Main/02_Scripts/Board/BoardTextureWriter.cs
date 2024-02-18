using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using WhiteboardXR.Brushes;

namespace WhiteboardXR.Board
{
    /// <summary>
    /// Uses a compute shader to write to the board texture with brushes
    /// </summary>
    public class BoardTextureWriter : MonoBehaviour
    {
        private static readonly int BaseMap = Shader.PropertyToID("_BaseMap");

        [SerializeField] private ComputeShader _shader;
        [SerializeField] private int _textureSize = 1024;
        [SerializeField] private Material _boardMaterial;
        [SerializeField] private List<Brush> _brushes;
        public RenderTexture BoardTexture { get; private set; }

        private struct Particle
        {
            public Vector4 color;
            public Vector3 previousPosition;
            public Vector3 position;
            public float isActive;
        };

        private Particle[] _particleArray;
        private int _kernel;
        private ComputeBuffer _particlesComputeBuffer;

        private void Start()
        {
            _kernel = _shader.FindKernel("CSMain");

            //Quad texture
            BoardTexture = new RenderTexture(_textureSize, _textureSize, 0, GraphicsFormat.R32G32B32A32_SFloat);
            BoardTexture.enableRandomWrite = true;
            BoardTexture.Create();
            ClearToWhite();
            
            _boardMaterial.SetTexture(BaseMap, BoardTexture);
            _shader.SetTexture(_kernel, "Result", BoardTexture);
            _shader.SetInt("size", _textureSize);
            _shader.SetFloat("quadSize", transform.localScale.x);
            
            _particleArray = new Particle[_brushes.Count];
            _particlesComputeBuffer = new ComputeBuffer(_particleArray.Length, 12 + 12 + 16 + 4);
            _particlesComputeBuffer.SetData(_particleArray);
            _shader.SetBuffer(_kernel, "particleBuffer", _particlesComputeBuffer);
            _shader.SetInt("particleCount", _particleArray.Length);


            for (int i = 0; i < _brushes.Count; ++i)
            {
                Vector3 particlePos = _brushes[i].transform.position;
                particlePos = transform.InverseTransformPoint(particlePos);
                _particleArray[i].position = particlePos;
            }
        }

        private void ClearToWhite()
        {
            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = BoardTexture;
            GL.Clear(true, true, Color.white);
            RenderTexture.active = rt;
        }

        private void Update()
        {
            UpdateParticles();

            _particlesComputeBuffer.SetData(_particleArray);
            _shader.Dispatch(_kernel, Mathf.CeilToInt(_textureSize / 8f), Mathf.CeilToInt(_textureSize / 8f), 1);
        }

        private void UpdateParticles()
        {
            for (int i = 0; i < _brushes.Count; ++i)
            {
                Vector3 particlePos = _brushes[i].transform.position;
                particlePos = transform.InverseTransformPoint(particlePos);
                _particleArray[i].previousPosition = _particleArray[i].position;
                _particleArray[i].position = particlePos;
                _particleArray[i].color = _brushes[i].Color;
            }
        }

        private void OnDestroy()
        {
            _particlesComputeBuffer.Release();
        }

        public void SetBrushActive(Brush brush, bool active)
        {
            int index = _brushes.IndexOf(brush);
            _particleArray[index].isActive = active ? 1 : 0;
        }
    }
}