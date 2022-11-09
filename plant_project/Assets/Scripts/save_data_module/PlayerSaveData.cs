using System.Collections.Generic;

namespace versoft.plant.game_logic
{
    public class PlayerSaveData
    {
        public List<PlantSavedData> PlantStates = new List<PlantSavedData>();
        public int SoftCurrency = 0;
        public bool HasCompletedTutorial = false;
        public int MaxNumberOfPlants = 2;
    }
}
