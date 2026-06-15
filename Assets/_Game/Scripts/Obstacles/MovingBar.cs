using UnityEngine;

namespace WheelMasterRun.Obstacles
{
    public class MovingBar : Obstacle
    {
        [Header("Bar Settings")]
        public float moveRange = 3f;
        public float speed = 1.5f;

        private Vector3 startPos;

        private void Start()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            float newX = startPos.x + Mathf.PingPong(Time.time * speed, moveRange * 2) - moveRange;
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
    }
}
