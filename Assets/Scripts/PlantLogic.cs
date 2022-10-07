using System;
using System.Collections.Generic;
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
        private PlantModel _plantModel;
        private Dictionary<PlantStat, Func<float>> _plantStatModifiers = new Dictionary<PlantStat, Func<float>>();

        public void Init(PlantSavedData plantSavedData, PlantModel plantModel)
        {
            _plantSavedData = (PlantSavedData)plantSavedData.Clone();
            _plantModel = plantModel;
        }

        private float GetStatChangeForPlantStat(PlantStat stat, float timeElapsed)
        {
            float multiplier = 1;
            if (_plantStatModifiers.TryGetValue(stat, out var multiplierFunc))
            {
                multiplier = multiplierFunc.Invoke();
            }

            float value = timeElapsed * multiplier;

            switch (stat)
            {
                case PlantStat.Light:
                    // Here we will check the Day / Night cycle
                    
                    break;
                case PlantStat.Water:
                case PlantStat.Food:
                    value *= -1;
                    break;
            }
            return value;
        }

        public void Tick(float timeElapsed)
        {
            TickPlantStat(PlantStat.Light, timeElapsed * Const.ResourceDepletionPerSecond);
            TickPlantStat(PlantStat.Water, timeElapsed * Const.ResourceDepletionPerSecond);
            TickPlantStat(PlantStat.Food,  timeElapsed * Const.ResourceDepletionPerSecond);
        }

        private void TickPlantStat(PlantStat stat, float timeElapsed)
        {
            float increment = GetStatChangeForPlantStat(stat, timeElapsed);
            ModifyStat(stat, increment);
        }

        public PlantSavedData GetPlantSaveData()
        {
            _plantSavedData.LastTimeUpdated = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return (PlantSavedData) _plantSavedData.Clone();
        }

        public float GetStatValue(PlantStat stat)
        {
            return GetStatValue(stat, _plantSavedData);
        }

        private float GetStatValue<T>(PlantStat stat, T objectFromClass)
        {
            float value = 0;
            var field = GetFieldForPlantStat<T>(stat);
            if (field != null)
            {
                value = (float)field.GetValue(objectFromClass);
            }

            return value;
        }

        private FieldInfo GetFieldForPlantStat<T>(PlantStat stat)
        {
            if (stat == PlantStat.None)
            { return null; }

            var plantType = typeof(T);
            return plantType.GetField(stat.ToString());
        }

        public void ModifyStat(PlantStat stat, float increment)
        {
            float plantMaxValue = GetStatValue(stat, _plantModel);
            float maxStatValue = plantMaxValue * Const.MaxPlantPercentage;

            var field = GetFieldForPlantStat<PlantSavedData>(stat);
            if(field != null)
            {
                float value = (float) field.GetValue(_plantSavedData);
                value += increment;
                value = Math.Clamp(value, 0, maxStatValue);

                field.SetValue(_plantSavedData, value);
            }
        }
    }
}
