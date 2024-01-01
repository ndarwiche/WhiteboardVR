using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WhiteboardXR.Brushes;

namespace WhiteboardXR.Board
{
    public class BoardBrushCollisionDetector : MonoBehaviour
    {
        [SerializeField] private BoardTextureWriter _board;

        private void OnTriggerEnter(Collider other)
        {
            var brush = other.GetComponent<Brush>();
            if (brush)
            {
                _board.SetBrushActive(brush, true);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            var brush = other.GetComponent<Brush>();
            if (brush)
            {
                _board.SetBrushActive(brush, false);
            }
        }
    }
}