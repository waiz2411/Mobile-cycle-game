using UnityEngine;
using WheelMasterRun.Player;
using WheelMasterRun.Data;

namespace WheelMasterRun.Collectibles
{
    public class CollectibleItem : MonoBehaviour
    {
        public enum ItemType { Wheel, Gem }
        
        [Header("Item Config")]
        public ItemType type;
        public float value = 0.1f; // For wheels: scaling increment. For gems: amount.
        public float rotationSpeed = 90f;

        [Header("FX")]
        [SerializeField] private GameObject collectionEffectPrefab;

        private void Update()
        {
            // Spin visual in place
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                Collect(player);
            }
        }

        private void Collect(PlayerController player)
        {
            if (type == ItemType.Wheel)
            {
                WheelSystem wheels = player.GetComponent<WheelSystem>();
                if (wheels != null)
                {
                    wheels.AddWheelSize(value);
                }
                
                // Play collection sound
                Debug.Log("+Wheel Size collected");
            }
            else if (type == ItemType.Gem)
            {
                if (CurrencyManager.Instance != null)
                {
                    CurrencyManager.Instance.AddGems((int)value);
                }
                
                // Floating text indicator
                Debug.Log("+1 Gem collected");
            }

            // Play particles
            if (collectionEffectPrefab != null)
            {
                Instantiate(collectionEffectPrefab, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
