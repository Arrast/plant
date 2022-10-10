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

    public enum PlantStage
    {
        Seedling = 0,
        Bud,
        Grown
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
                    // For now, let's just leave them all decreasing.
                    // break;
                case PlantStat.Water:
                case PlantStat.Food:
                    value *= -1;
                    break;
            }
            return value;
        }

        private void CheckPlantConditions(float timeElapsed)
        {
            bool plantInBadConditions = false;
            if (_plantSavedData.Sick)
            {
                plantInBadConditions = true;
            }

            foreach (PlantStat stat in Enum.GetValues(typeof(PlantStat)))
            {
                if(stat == PlantStat.None) 
                { continue; }

                float baseValue = GetStatValue(stat, _plantModel);
                float value = GetStatValue(stat, _plantSavedData);
                if(value < baseValue * Const.LowResourceThreshold || value > baseValue * Const.HighResourceThreshold)
                {
                    plantInBadConditions = true;
                    UnityEngine.Debug.LogError("Que se muere! Que se muere!");
                }                
            }

            if (plantInBadConditions)
            {
                _plantSavedData.TimeInBadConditions += timeElapsed;
                if(_plantSavedData.TimeInBadConditions > Const.MaxTimeInBadConditions)
                {
                    // Plant Pepsi
                    PlantDead();
                }
            }
        }

        private void PlantDead()
        {
            UnityEngine.Debug.LogError("Se murio");
            _plantSavedData.Alive = false;
        }

        public void Tick(float timeElapsed)
        {
            if (!_plantSavedData.Alive) 
            { 
                return; 
            }

            TickPlantStat(PlantStat.Light, timeElapsed * Const.ResourceDepletionPerSecond);
            TickPlantStat(PlantStat.Water, timeElapsed * Const.ResourceDepletionPerSecond);
            TickPlantStat(PlantStat.Food,  timeElapsed * Const.ResourceDepletionPerSecond);
            IncreaseLifetime(timeElapsed);
            CheckPlantConditions(timeElapsed);
        }

        private void IncreaseLifetime(float timeElapsed)
        {
            if(_plantSavedData.PlantStage != PlantStage.Grown)
            {
                int plantStage = (int)_plantSavedData.PlantStage;
                float previousMinTime = (plantStage == 0) ? 0: _plantModel.LevelUpTimes[0];
                float timeToLevelUp = _plantModel.LevelUpTimes[plantStage] + previousMinTime;
               
                if (_plantSavedData.PlantLifetime < timeToLevelUp && _plantSavedData.PlantLifetime + timeElapsed >= timeToLevelUp)
                {
                    _plantSavedData.PlantStage += 1;
                }
            }

            _plantSavedData.PlantLifetime += timeElapsed;
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
