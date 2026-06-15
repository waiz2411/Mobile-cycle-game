using System.Collections.Generic;
using UnityEngine;
using WheelMasterRun.Core;
using WheelMasterRun.Data;

namespace WheelMasterRun.Environment
{
    public class LevelManager : MonoBehaviour
    {
        public static LevelManager Instance { get; private set; }

        [Header("Prefabs")]
        [SerializeField] private GameObject[] trackSegmentPrefabs;
        [SerializeField] private GameObject finishSegmentPrefab;
        [SerializeField] private GameObject wheelCollectiblePrefab;
        [SerializeField] private GameObject gemCollectiblePrefab;
        [SerializeField] private GameObject[] obstaclePrefabs; // Saw, Spikes, Crusher, etc.

        [Header("Generation Settings")]
        public int segmentsPerLevel = 10;
        public float segmentLength = 30f;

        private List<GameObject> activeSegments = new List<GameObject>();
        private List<GameObject> spawnedItems = new List<GameObject>();
        private Vector3 nextSpawnPoint = Vector3.zero;

        private void Awake()
        {
            if (Instance == null) Instance = this;
        }

        public void GenerateLevel(int levelNumber)
        {
            ClearLevel();

            // Set seed based on level to ensure deterministic generation
            Random.InitState(levelNumber);

            nextSpawnPoint = Vector3.zero;

            // 1. Spawn starting track (safe zone)
            SpawnTrack(0); // Assuming 0 is the simplest straight track

            // 2. Spawn intermediate segments with obstacles & collectibles
            for (int i = 1; i < segmentsPerLevel; i++)
            {
                int prefabIndex = Random.Range(0, trackSegmentPrefabs.Length);
                GameObject segmentObj = SpawnTrack(prefabIndex);
                TrackSegment segment = segmentObj.GetComponent<TrackSegment>();
                
                if (segment != null)
                {
                    PopulateSegment(segment, levelNumber);
                }
            }

            // 3. Spawn Finish line segment
            SpawnFinishSegment();
        }

        private GameObject SpawnTrack(int index)
        {
            GameObject prefab = trackSegmentPrefabs[index];
            GameObject segment = Instantiate(prefab, nextSpawnPoint, Quaternion.identity, transform);
            activeSegments.Add(segment);

            TrackSegment ts = segment.GetComponent<TrackSegment>();
            if (ts != null)
            {
                nextSpawnPoint = ts.GetAnchorPosition();
            }
            else
            {
                nextSpawnPoint += Vector3.forward * segmentLength;
            }

            return segment;
        }

        private void SpawnFinishSegment()
        {
            if (finishSegmentPrefab != null)
            {
                GameObject finishObj = Instantiate(finishSegmentPrefab, nextSpawnPoint, Quaternion.identity, transform);
                activeSegments.Add(finishObj);
            }
        }

        private void PopulateSegment(TrackSegment segment, int levelNumber)
        {
            // Calculate spawn density/difficulty based on level
            float obstacleChance = Mathf.Min(0.3f + (levelNumber * 0.02f), 0.75f);
            float itemChance = 0.5f;

            Transform[][] lanes = new Transform[][]
            {
                segment.lane0SpawnPoints,
                segment.lane1SpawnPoints,
                segment.lane2SpawnPoints
            };

            for (int laneIndex = 0; laneIndex < 3; laneIndex++)
            {
                Transform[] spawnPoints = lanes[laneIndex];
                if (spawnPoints == null) continue;

                foreach (Transform sp in spawnPoints)
                {
                    if (sp == null) continue;

                    float roll = Random.value;

                    if (roll < obstacleChance)
                    {
                        // Spawn Obstacle
                        int obsIndex = Random.Range(0, obstaclePrefabs.Length);
                        GameObject obs = Instantiate(obstaclePrefabs[obsIndex], sp.position, Quaternion.identity, segment.transform);
                        spawnedItems.Add(obs);
                    }
                    else if (roll < obstacleChance + itemChance)
                    {
                        // Spawn Collectibles
                        GameObject itemPrefab = (Random.value > 0.3f) ? wheelCollectiblePrefab : gemCollectiblePrefab;
                        if (itemPrefab != null)
                        {
                            GameObject item = Instantiate(itemPrefab, sp.position + Vector3.up * 0.5f, Quaternion.identity, segment.transform);
                            spawnedItems.Add(item);
                        }
                    }
                }
            }
        }

        public void ClearLevel()
        {
            foreach (GameObject seg in activeSegments)
            {
                if (seg != null) Destroy(seg);
            }
            activeSegments.Clear();

            foreach (GameObject item in spawnedItems)
            {
                if (item != null) Destroy(item);
            }
            spawnedItems.Clear();
        }
    }
}
