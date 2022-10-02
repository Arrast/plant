using Newtonsoft.Json;
using System;

namespace versoft.plant.game_logic
{
    [System.Serializable]
    public class PlantSavedData : System.ICloneable
    {
        [JsonProperty("plant_id")]
        public string PlantId = "default";

        [JsonProperty("plant_instance_id")]
        public string PlantInstanceId = "default";

        [JsonProperty("last_time_updated")]
        public long LastTimeUpdated;

        [JsonProperty("light")]
        public float SavedLight = 1.0f;

        [JsonProperty("food")]
        public float SavedFood = 1.0f;

        [JsonProperty("water")]
        public float SavedWater = 1.0f;

        [JsonProperty("sick")]
        public bool Sick = false;

        public object Clone()
        {
            var clone = new PlantSavedData()
            {
                PlantId = this.PlantId,
                PlantInstanceId = this.PlantInstanceId,
                LastTimeUpdated = this.LastTimeUpdated,
                SavedLight = this.SavedLight,
                SavedFood = this.SavedFood,
                SavedWater = this.SavedWater,
                Sick = Sick
            };
            return clone;
        }

        public void Tick(float timeElapsed)
        {            
            SavedFood = Math.Clamp(SavedFood - timeElapsed * Const.ResourceDepletionPerSecond, 0, Const.MaxPlantValue);
            SavedLight = Math.Clamp(SavedLight - timeElapsed * Const.ResourceDepletionPerSecond, 0, Const.MaxPlantValue);
            SavedWater = Math.Clamp(SavedWater - timeElapsed * Const.ResourceDepletionPerSecond, 0, Const.MaxPlantValue);
        }

        public override string ToString()
        {
            return $"[{PlantId} - {PlantInstanceId}] Food: {SavedFood.ToString("F2")} | Water: {SavedWater.ToString("F2")} | Light: {SavedLight.ToString("F2")} | Sick {Sick}";
        }
    }
}
