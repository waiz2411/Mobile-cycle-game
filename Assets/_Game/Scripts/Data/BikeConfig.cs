using UnityEngine;

namespace WheelMasterRun.Data
{
    [CreateAssetMenu(fileName = "NewBikeConfig", menuName = "Wheel Master Run/Bike Config")]
    public class BikeConfig : ScriptableObject
    {
        public string bikeID;
        public string bikeName;
        public int coinUnlockCost;
        public Sprite bikeIcon;
        public GameObject bikeModelPrefab; // Reference to the 3D model visual prefab
        public Color primaryColor = Color.white;
    }
}
