using System;
using System.Collections.Generic;
using UnityEngine;

namespace versoft.plant.game_logic
{
    [System.Serializable]
    public class PlantLayoutElementSaveData : ICloneable
    {
        public bool Unlocked;
        public string Skin;
        public List<string> PlantIds;
        public int MaxNumberOfPlants = Const.MaxNumberOfPlantsPerZone;

        public PlantLayoutElementSaveData()
        {
            Unlocked = false;
            Skin = "";
            PlantIds = new List<string>();
            MaxNumberOfPlants = Const.MaxNumberOfPlantsPerZone;
        }

        public PlantLayoutElementSaveData(string defaultSkin)
        {
            Skin = defaultSkin;
            Unlocked = false;
            PlantIds = new List<string>();
            MaxNumberOfPlants = Const.MaxNumberOfPlantsPerZone;
        }

        public PlantLayoutElementSaveData(bool unlocked, int maxNumberOfPlants, string defaultSkin)
        {
            Unlocked = unlocked;
            Skin = defaultSkin;
            PlantIds = new List<string>();
            MaxNumberOfPlants = maxNumberOfPlants;
        }

        public object Clone()
        {
            PlantLayoutElementSaveData clone = new PlantLayoutElementSaveData();
            clone.Unlocked = Unlocked;
            clone.Skin = Skin;
            clone.MaxNumberOfPlants = MaxNumberOfPlants;
            foreach (var plantId in PlantIds)
            {
                clone.PlantIds.Add(plantId);
            }
            return clone;
        }

        public bool IsFull()
        {
            return PlantIds.Count == MaxNumberOfPlants;
        }
    }

    [System.Serializable]
    public class PlantLayoutSaveData : ICloneable
    {
        [SerializeField] private List<string> positionNameList;
        [SerializeField] private List<PlantLayoutElementSaveData> positionSaveDataList;

        private Dictionary<string, PlantLayoutElementSaveData> _positions;

        public Dictionary<string, PlantLayoutElementSaveData> Positions
        {
            get
            {
                if (_positions == null)
                {
                    BuildDictionary();
                }
                return _positions;
            }
        }

        private void BuildDictionary()
        {
            if (positionNameList.Count != positionSaveDataList.Count)
            {
                // Values Mismatch
                return;
            }

            _positions = new Dictionary<string, PlantLayoutElementSaveData>();
            for (int i = 0; i < positionNameList.Count; i++)
            {
                _positions.Add(positionNameList[i], positionSaveDataList[i]);
            }
        }

        public void Add(string positionName, PlantLayoutElementSaveData positionValue)
        {
            Positions.Add(positionName, positionValue);
            this.positionNameList.Add(positionName);
            this.positionSaveDataList.Add(positionValue);
        }

        public object Clone()
        {
            var plantLayoutClone = new PlantLayoutSaveData();
            plantLayoutClone.positionNameList = new List<string>();
            foreach(var (plantId, plantSaveData) in Positions)
            {
                plantLayoutClone.positionNameList.Add(plantId);
                plantLayoutClone.positionSaveDataList.Add((PlantLayoutElementSaveData)plantSaveData.Clone());
            }
            return plantLayoutClone;
        }

        public PlantLayoutSaveData()
        {
            positionSaveDataList = new List<PlantLayoutElementSaveData>();
            positionNameList = new List<string>();
        }
    }
}
