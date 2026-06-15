using System;
using UnityEngine;

namespace WheelMasterRun.Player
{
    public class WheelSystem : MonoBehaviour
    {
        [Header("Wheel Configuration")]
        [SerializeField] private Transform[] wheels; // Front and back wheels
        [SerializeField] private Transform bikeBodyVisual; // Entire bike mesh excluding wheels, or visual parent
        
        [Header("Scaling Boundaries")]
        public float currentScale = 1.0f;
        public float minScale = 0.5f;
        public float maxScale = 10.0f;
        
        [Header("Smoothness Settings")]
        public float scaleLerpSpeed = 5f;
        
        // Base measurements
        private float targetScale = 1.0f;
        private float baseWheelRadius = 0.5f; // Approx radius at scale 1.0

        public event Action<float> OnScaleChanged;
        public event Action OnPlayerLost;

        private void Start()
        {
            targetScale = currentScale;
            ApplyWheelScale(currentScale);
        }

        private void Update()
        {
            if (Mathf.Abs(currentScale - targetScale) > 0.001f)
            {
                // Smooth scale interpolation
                currentScale = Mathf.Lerp(currentScale, targetScale, Time.deltaTime * scaleLerpSpeed);
                ApplyWheelScale(currentScale);
            }
        }

        public void AddWheelSize(float amount)
        {
            targetScale = Mathf.Clamp(targetScale + amount, minScale, maxScale);
            OnScaleChanged?.Invoke(targetScale);
        }

        public void DecreaseWheelSize(float amount)
        {
            targetScale = Mathf.Clamp(targetScale - amount, minScale, maxScale);
            OnScaleChanged?.Invoke(targetScale);

            if (targetScale <= minScale)
            {
                OnPlayerLost?.Invoke();
            }
        }

        private void ApplyWheelScale(float scale)
        {
            // Scale individual wheel transforms
            foreach (Transform wheel in wheels)
            {
                if (wheel != null)
                {
                    wheel.localScale = Vector3.one * scale;
                }
            }

            // Raise/lower bike body relative to the wheel scale to keep wheels on the ground
            if (bikeBodyVisual != null)
            {
                // The axle needs to go higher as wheel radius grows
                float heightOffset = baseWheelRadius * (scale - 1.0f);
                Vector3 localPos = bikeBodyVisual.localPosition;
                localPos.y = heightOffset;
                bikeBodyVisual.localPosition = localPos;
            }
        }

        // Used to determine current category level of wheels
        public string GetWheelSizeCategory()
        {
            if (currentScale < 1.2f) return "Small";
            if (currentScale < 2.5f) return "Medium";
            if (currentScale < 4.5f) return "Large";
            if (currentScale < 7.0f) return "Giant";
            return "Ultra Giant";
        }
    }
}
