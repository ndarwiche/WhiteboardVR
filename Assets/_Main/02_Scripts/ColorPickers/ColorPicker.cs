using System;
using System.Collections.Generic;
using UnityEngine;

namespace WhiteboardXR.ColorPicker
{
    [RequireComponent(typeof(Renderer))]
    public class ColorPicker : MonoBehaviour
    {
        private float _pickerSize;
        private Texture2D _texture;
        private Vector2 _textureSize;

        private void Start()
        {
            _pickerSize = transform.localScale.x;
            _texture = GetComponent<Renderer>().sharedMaterial.mainTexture as Texture2D;
            _textureSize = new Vector2(_texture.width, _texture.height);
        }

        private Color GetColor(Vector2 uv)
        {
            int x = Mathf.RoundToInt(uv.x * _textureSize.x);
            int y = Mathf.RoundToInt(uv.y * _textureSize.y);
            return _texture.GetPixel(x, y);
        }

        private void OnCollisionStay(Collision collision)
        {
            var colorable = collision.collider.GetComponent<IColorable>();
            if (colorable == null)
                return;
            Vector3 localPosition = transform.InverseTransformPoint(collision.collider.transform.position);
            Vector2 uv = localPosition + new Vector3(_pickerSize * 0.5f, _pickerSize * 0.5f) / _pickerSize;
            Debug.Log($"Pos = {localPosition} , uv = {uv}");
            colorable.Color = GetColor(uv);
        }
    }
}