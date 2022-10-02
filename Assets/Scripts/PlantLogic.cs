

using Newtonsoft.Json.Schema;
using System;
using System.Reflection;

namespace versoft.plant.game_logic
{
    public enum PlantStat
    {
        None,
        Water,
        Light,
        Food
    }

    public class PlantLogic
    {
        private PlantSavedData _plantSavedData;

        public void Init(PlantSavedData plantSavedData)
        {
            _plantSavedData = (PlantSavedData)plantSavedData.Clone();
        }

        public void Tick(float timeElapsed)
        {
            _plantSavedData.Tick(timeElapsed);
        }

        public PlantSavedData GetPlantSaveData()
        {
            _plantSavedData.LastTimeUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return (PlantSavedData) _plantSavedData.Clone();
        }

        public float GetStatValue(PlantStat stat)
        {
            float value = 0;
            var field = GetFieldForPlantStat(stat);
            if (field != null)
            {
                value = (float)field.GetValue(_plantSavedData);
            }

            return value;
        }

        private FieldInfo GetFieldForPlantStat(PlantStat stat)
        {
            if (stat == PlantStat.None)
            { return null; }

            var plantType = typeof(PlantSavedData);
            return plantType.GetField($"Saved{stat}");
        }

        public void IncreaseStat(PlantStat stat, float increment)
        {
            var field = GetFieldForPlantStat(stat);
            if(field != null)
            {
                float value = (float) field.GetValue(_plantSavedData);
                value += increment;
                if(value > Const.MaxPlantValue) 
                { return; }
                
                field.SetValue(_plantSavedData, value += increment);
            }
        }
    }
}
