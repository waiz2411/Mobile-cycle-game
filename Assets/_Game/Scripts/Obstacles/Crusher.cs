using UnityEngine;

namespace WheelMasterRun.Obstacles
{
    public class Crusher : Obstacle
    {
        [Header("Crusher Settings")]
        public float peakHeight = 4f;
        public float groundHeight = 0.5f;
        public float speed = 3f;

        private float timer;

        private void Update()
        {
            timer += Time.deltaTime * speed;
            // Ping-pong style vertical movement
            float targetY = Mathf.Lerp(groundHeight, peakHeight, (Mathf.Sin(timer) + 1f) / 2f);
            
            Vector3 pos = transform.position;
            pos.y = targetY;
            transform.position = pos;
        }
    }
}
