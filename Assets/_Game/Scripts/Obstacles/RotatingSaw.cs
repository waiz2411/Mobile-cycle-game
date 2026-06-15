using UnityEngine;

namespace WheelMasterRun.Obstacles
{
    public class RotatingSaw : Obstacle
    {
        [Header("Saw Movement & Rotation")]
        public float rotationSpeed = 360f;
        public float moveRange = 2f;
        public float moveSpeed = 2f;
        public bool moveHorizontally = true;

        private Vector3 startPos;

        private void Start()
        {
            startPos = transform.position;
        }

        private void Update()
        {
            // Spin saw blade
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);

            // Move left and right
            if (moveHorizontally)
            {
                float newX = startPos.x + Mathf.Sin(Time.time * moveSpeed) * moveRange;
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
            }
        }
    }
}
