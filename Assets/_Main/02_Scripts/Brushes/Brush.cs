using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteboardXR.Brushes
{
    public class Brush : MonoBehaviour, IColorable
    {
        [SerializeField] private Color color;
        [SerializeField] private MeshRenderer _meshRenderer;
        public bool IsTouchingBoard { get; set; }


        public Color Color
        {
            get => color;
            set
            {
                color = value;
                _meshRenderer.material.color = color;
            }
        }
    }
}