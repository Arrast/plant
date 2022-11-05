
using System;
using versoft.asset_manager;
using versoft.plant.game_logic;
using versoft.save_data;

public class PlayerManager
{
    private PlayerSaveData _playerSaveData;
    private System.Action<int> _onCurrencyModified;

    public void Initialize()
    {
        LoadSave();
    }

    public void AddCurrencyModifiedListener(System.Action<int> onCurrencyModified)
    {
        _onCurrencyModified += onCurrencyModified;
    }

    public void IncreaseCurrency(int currencyIncrease)
    {
        _playerSaveData.SoftCurrency += currencyIncrease;
        _onCurrencyModified?.Invoke(_playerSaveData.SoftCurrency);
    }

    public void LoadSave()
    {
        var saveDataManager = ServiceLocator.Instance.Get<SaveDataManager>();
        if (saveDataManager == null)
        {
            return;
        }

        _playerSaveData = saveDataManager.LoadData<PlayerSaveData>();
        if (_playerSaveData == null)
        {
            _playerSaveData = new PlayerSaveData();
        }

        var plantManager = ServiceLocator.Instance.Get<PlantManager>();
        plantManager.LoadPlantsFromSaveData(_playerSaveData);
    }

    public void SaveToFile()
    {
        var saveDataManager = ServiceLocator.Instance.Get<SaveDataManager>();
        if (saveDataManager == null)
        {
            return;
        }

        var plantManager = ServiceLocator.Instance.Get<PlantManager>();
        if (plantManager == null)
        {
            return;
        }

        _playerSaveData.PlantStates = plantManager.GetPlants();
        saveDataManager.SaveData(_playerSaveData);
    }

    public int GetSoftCurrency()
    {
        if(_playerSaveData == null) 
        { return 0; }

        return _playerSaveData.SoftCurrency;
    }
}
