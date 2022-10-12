
using versoft.asset_manager;
using versoft.plant.game_logic;
using versoft.save_data;

public class PlayerManager
{
    private PlayerSaveData _playerSaveData;

    public void Initialize()
    {
        LoadSave();
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
}
