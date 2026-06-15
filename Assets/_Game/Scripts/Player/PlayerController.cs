using System;
using UnityEngine;

namespace WheelMasterRun.Player
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(WheelSystem))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController Instance { get; private set; }

        [Header("Movement Settings")]
        public float forwardSpeed = 8f;
        public float laneChangeSpeed = 10f;
        public float laneWidth = 2.0f; // Distance between lanes

        [Header("Lane Setup")]
        private int currentLane = 1; // 0 = Left, 1 = Center, 2 = Right
        private float targetX = 0f;

        [Header("Inputs")]
        private Vector2 touchStartPos;
        private bool isSwiping = false;
        private readonly float minSwipeDistance = 50f;

        private Rigidbody rb;
        private WheelSystem wheelSystem;
        private bool isControlActive = false;
        private bool isMultiplierClimb = false;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            rb = GetComponent<Rigidbody>();
            wheelSystem = GetComponent<WheelSystem>();
        }

        private void Start()
        {
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.FreezeRotation;
        }

        public void StartRun()
        {
            isControlActive = true;
            isMultiplierClimb = false;
            currentLane = 1;
            targetX = 0f;
        }

        public void StopRun()
        {
            isControlActive = false;
            rb.velocity = Vector3.zero;
        }

        private void Update()
        {
            if (!isControlActive) return;

            HandlePCInput();
            HandleMobileInput();
        }

        private void FixedUpdate()
        {
            if (!isControlActive) return;

            Vector3 currentPos = rb.position;
            
            // Calculate target position
            float newX = Mathf.Lerp(currentPos.x, targetX, Time.fixedDeltaTime * laneChangeSpeed);
            float newZ = currentPos.z + (forwardSpeed * Time.fixedDeltaTime);
            
            // Allow physical gravity for Y position (so bike stays on ramps/ground)
            Vector3 targetPosition = new Vector3(newX, currentPos.y, newZ);
            
            rb.MovePosition(targetPosition);
        }

        private void HandlePCInput()
        {
            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                MoveLane(false);
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                MoveLane(true);
            }
        }

        private void HandleMobileInput()
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchStartPos = touch.position;
                        isSwiping = true;
                        break;

                    case TouchPhase.Moved:
                        if (isSwiping)
                        {
                            Vector2 diff = touch.position - touchStartPos;
                            if (diff.magnitude > minSwipeDistance)
                            {
                                // Check swipe direction
                                if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
                                {
                                    if (diff.x > 0) MoveLane(true); // Swipe Right
                                    else MoveLane(false); // Swipe Left
                                }
                                isSwiping = false; // Prevent multi-triggers
                            }
                        }
                        break;

                    case TouchPhase.Ended:
                        isSwiping = false;
                        break;
                }
            }
        }

        private void MoveLane(bool right)
        {
            if (right && currentLane < 2)
            {
                currentLane++;
            }
            else if (!right && currentLane > 0)
            {
                currentLane--;
            }

            // Map lane index to X coordinate
            targetX = (currentLane - 1) * laneWidth;
        }

        public void EnterMultiplierZone(float rampForce)
        {
            isMultiplierClimb = true;
            rb.AddForce(Vector3.up * rampForce + Vector3.forward * (forwardSpeed * 1.5f), ForceMode.VelocityChange);
        }
    }
}
