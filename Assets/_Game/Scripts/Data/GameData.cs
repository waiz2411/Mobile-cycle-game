using System;
using System.Collections.Generic;

namespace WheelMasterRun.Core
{
    [Serializable]
    public class GameData
    {
        public int currentLevel = 1;
        public int coins = 0;
        public int gems = 0;
        public List<string> unlockedBikes = new List<string> { "BMX" };
        public string selectedBike = "BMX";
    }
}
