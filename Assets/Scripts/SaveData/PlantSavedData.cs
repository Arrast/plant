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
        public float Light = 1.0f;

        [JsonProperty("food")]
        public float Food = 1.0f;

        [JsonProperty("water")]
        public float Water = 1.0f;

        [JsonProperty("sick")]
        public bool Sick = false;

        [JsonProperty("plant_lifetime")]
        public float PlantLifetime = 0.0f;

        [JsonProperty("plant_stage")]
        public PlantStage PlantStage = PlantStage.Seedling;

        [JsonProperty("time_in_bad_conditions")]
        public float TimeInBadConditions = 0.0f;

        [JsonProperty("alive")]
        public bool Alive = true;

        public object Clone()
        {
            var clone = new PlantSavedData()
            {
                PlantId = this.PlantId,
                PlantInstanceId = this.PlantInstanceId,
                LastTimeUpdated = this.LastTimeUpdated,
                Light = this.Light,
                Food = this.Food,
                Water = this.Water,
                Sick = this.Sick,
                PlantStage = this.PlantStage,
                PlantLifetime = this.PlantLifetime,
                TimeInBadConditions = this.TimeInBadConditions,
                Alive = this.Alive
            };
            return clone;
        }

        public void ModifyResource(PlantStat stat, float value)
        {
            switch (stat)
            {
                case PlantStat.Food:
                    Food = Math.Clamp(Food - value, 0, Const.MaxPlantValue);
                    break;
                case PlantStat.Light:
                    Light = Math.Clamp(Light - value, 0, Const.MaxPlantValue);
                    break;
                case PlantStat.Water:
                    Water = Math.Clamp(Water - value, 0, Const.MaxPlantValue);
                    break;
            }
        }

        public override string ToString()
        {
            return $"[{PlantId} - {PlantInstanceId}] Food: {Food.ToString("F2")} | Water: {Water.ToString("F2")} | Light: {Light.ToString("F2")} | Sick {Sick}";
        }
    }
}
