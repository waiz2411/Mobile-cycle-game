using UnityEngine;
using WheelMasterRun.Player;

namespace WheelMasterRun.Obstacles
{
    public abstract class Obstacle : MonoBehaviour
    {
        [Header("Base Obstacle Settings")]
        [SerializeField] protected float damageAmount = 0.2f;
        [SerializeField] protected float pushForce = 5f;

        private float hitCooldown = 1.0f;
        private float lastHitTime = -999f;

        protected virtual void OnTriggerEnter(Collider other)
        {
            if (Time.time < lastHitTime + hitCooldown) return;

            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                lastHitTime = Time.time;
                OnPlayerHit(player);
            }
        }

        protected virtual void OnPlayerHit(PlayerController player)
        {
            WheelSystem wheels = player.GetComponent<WheelSystem>();
            if (wheels != null)
            {
                wheels.DecreaseWheelSize(damageAmount);
            }

            // Apply slight push-back or shake event
            Rigidbody rb = player.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce((Vector3.back * pushForce) + (Vector3.up * 2f), ForceMode.Impulse);
            }
            
            PlayHitEffect();
        }

        protected virtual void PlayHitEffect()
        {
            Debug.Log($"Hit obstacle: {gameObject.name}");
            // Instantiate impact particles or play sounds here
        }
    }
}
