using UnityEngine;
using WheelMasterRun.Player;

namespace WheelMasterRun.Obstacles
{
    public class SizeGate : MonoBehaviour
    {
        [Header("Gate Settings")]
        public float requiredScale = 2.0f;
        public GameObject successVisual;
        public GameObject failureVisual;

        private void Start()
        {
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (successVisual != null && failureVisual != null)
            {
                successVisual.SetActive(true);
                failureVisual.SetActive(false);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            PlayerController player = other.GetComponentInParent<PlayerController>();
            if (player != null)
            {
                WheelSystem wheels = player.GetComponent<WheelSystem>();
                if (wheels != null)
                {
                    if (wheels.currentScale >= requiredScale)
                    {
                        // Player successfully breaks through the gate
                        Debug.Log("Pass size gate success!");
                        // Play a shatter or success effect
                        Destroy(gameObject);
                    }
                    else
                    {
                        // Failure: Damage or stop player
                        Debug.Log("Failed size gate: scale too small!");
                        wheels.DecreaseWheelSize(0.3f);
                        
                        // Bounce player back
                        Rigidbody rb = player.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.AddForce((Vector3.back * 8f) + (Vector3.up * 3f), ForceMode.Impulse);
                        }
                    }
                }
            }
        }
    }
}
