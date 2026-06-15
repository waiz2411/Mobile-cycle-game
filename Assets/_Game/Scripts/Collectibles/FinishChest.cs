using UnityEngine;
using WheelMasterRun.Core;
using WheelMasterRun.Player;

namespace WheelMasterRun.Collectibles
{
    public class FinishChest : MonoBehaviour
    {
        [Header("Chest Rewards")]
        public int baseGemsReward = 10;
        public GameObject openChestVisual;
        public GameObject closedChestVisual;
        public GameObject openEffectPrefab;

        private bool isOpened = false;

        private void Start()
        {
            if (closedChestVisual != null) closedChestVisual.SetActive(true);
            if (openChestVisual != null) openChestVisual.SetActive(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (isOpened) return;

            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                OpenChest(player);
            }
        }

        private void OpenChest(PlayerController player)
        {
            isOpened = true;

            if (closedChestVisual != null) closedChestVisual.SetActive(false);
            if (openChestVisual != null) openChestVisual.SetActive(true);

            // Play particles
            if (openEffectPrefab != null)
            {
                Instantiate(openEffectPrefab, transform.position, Quaternion.identity);
            }

            // Calculate multiplier based on wheel size reached
            WheelSystem wheels = player.GetComponent<WheelSystem>();
            float finalScale = wheels != null ? wheels.currentScale : 1f;

            // Simple scale threshold multiplier mapping
            int multiplier = 1;
            if (finalScale >= 8.0f) multiplier = 10;
            else if (finalScale >= 5.0f) multiplier = 5;
            else if (finalScale >= 3.0f) multiplier = 3;
            else if (finalScale >= 2.0f) multiplier = 2;

            int finalReward = baseGemsReward * multiplier;

            Debug.Log($"Chest opened! Multiplier: {multiplier}x. Gems gained: {finalReward}");

            // Notify GameManager of victory
            if (GameManager.Instance != null)
            {
                GameManager.Instance.CompleteLevel(finalReward, multiplier);
            }
        }
    }
}
