using UnityEngine;

namespace WheelMasterRun.Environment
{
    public class TrackSegment : MonoBehaviour
    {
        [Header("Sockets")]
        public Transform connectionAnchor; // The transform at the end of the road segment

        [Header("Spawn Points")]
        public Transform[] lane0SpawnPoints; // Left Lane (-2)
        public Transform[] lane1SpawnPoints; // Center Lane (0)
        public Transform[] lane2SpawnPoints; // Right Lane (+2)
        
        public Vector3 GetAnchorPosition()
        {
            return connectionAnchor != null ? connectionAnchor.position : transform.position + new Vector3(0, 0, 30f); // Fallback length
        }
    }
}
