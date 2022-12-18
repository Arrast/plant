using System.Collections.Generic;

namespace versoft.plant.game_logic
{
    public class PlayerSaveData
    {
        public List<PlantSaveData> PlantStates = new List<PlantSaveData>();
        public int SoftCurrency = 0;
        public bool HasCompletedTutorial = false;
        public int MaxNumberOfPlants = 2;
        public string EquippedPlantLayout = "";
        public PlantLayoutSaveData PlantLayout;
        public PurchasesSaveData PurchasesSaveData;
    }
}
